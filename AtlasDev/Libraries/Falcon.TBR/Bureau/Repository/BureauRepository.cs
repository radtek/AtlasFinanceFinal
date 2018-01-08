using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Common.Utils;
using Atlas.Domain.Model;
using Atlas.RabbitMQ.Messages.Credit;
using Atlas.ThirdParty.CompuScan.Enquiry;
using DevExpress.Xpo;
using Falcon.Common.Interfaces.Structures;
using Falcon.TBR.Bureau.Interfaces;
using Falcon.TBR.Bureau.Structures;
using Magnum;
using Magnum.Extensions;
using IAssBureauRepository = Atlas.Ass.Framework.Repository.IAssBureauRepository;
using Product = Atlas.ThirdParty.CompuScan.Enquiry.Product;

namespace Falcon.TBR.Bureau.Repository
{
  public class BureauRepository : IBureauRepository
  {
    private readonly IBureauService _bureauService;
    private readonly IAssBureauRepository _assBureauRepository;

    public BureauRepository(IBureauService bureauService, IAssBureauRepository assBureauRepository)
    {
      _bureauService = bureauService;
      _assBureauRepository = assBureauRepository;
    }

    public List<Interfaces.ICompuscanProducts> GetCompuscanProductsSummary(ICollection<long> branchIds, DateTime date)
    {
      using (var uow = new UnitOfWork())
      {
        var branchProducts = new List<CompuscanProducts>();

        foreach (var branchId in branchIds)
        {
          var branchProduct = new CompuscanProducts
          {
            BranchId = branchId,
            Date = date
          };
          var enquiries =
            new XPQuery<BUR_Enquiry>(uow).Where(
              p =>
                p.EnquiryDate.Date >= date.Date &&
                p.Branch.BranchId == branchId && p.IsSucess)
              .OrderBy(e => e.EnquiryId)
              .ToList();

          foreach (var enq in enquiries)
          {
            var storage = enq.Storage.FirstOrDefault();

            if (storage == null || storage.ResponseMessage == null)
              continue;

            var dbResult =
              ((ResponseResultV2)
                Xml.DeSerialize<ResponseResultV2>(
                  Compression.Decompress(storage.ResponseMessage)));

            if (dbResult == null)
              continue;

            if (!_assBureauRepository.DoesNlrExistsInAss(dbResult.NLREnquiryReferenceNo))
              continue;

            var foundSuccess = false;
            if (dbResult.Products != null && dbResult.Products.Count > 0)
            {
              foundSuccess = IsProductAccepted(dbResult.Products, "12 Month");
              if (foundSuccess)
              {
                branchProduct.TwelveMonths++;
              }
              else
              {
                foundSuccess = IsProductAccepted(dbResult.Products, "5 To 6 Month");
                if (foundSuccess)
                {
                  branchProduct.FiveToSixMonths++;
                }
                else
                {
                  foundSuccess = IsProductAccepted(dbResult.Products, "2 To 4 Month");
                  if (foundSuccess)
                  {
                    branchProduct.TwoToFourMonths++;
                  }
                  else
                  {
                    foundSuccess = IsProductAccepted(dbResult.Products, "1M Capped");
                    if (foundSuccess)
                    {
                      branchProduct.OneMCapped++;
                    }
                    else
                    {
                      foundSuccess = IsProductAccepted(dbResult.Products, "1M Thin");
                      if (foundSuccess)
                      {
                        branchProduct.OneMThin++;
                      }
                      else
                      {
                        foundSuccess = IsProductAccepted(dbResult.Products, "1 Month");
                        if (foundSuccess)
                        {
                          branchProduct.OneMonth++;
                        }
                      }
                    }
                  }
                }
              }
            }

            if (!foundSuccess)
            {
              branchProduct.Declined++;
            }
          }
          branchProducts.Add(branchProduct);
        }

        return branchProducts.Select(p => p.CastAs<Interfaces.ICompuscanProducts>()).ToList();
      }
    }

    private bool IsProductAccepted(List<Product> products, string product)
    {
      if (products.FirstOrDefault(
        p =>
          string.Equals(p.Description.Trim(), product) &&
          string.Equals(p.Outcome, "Y")) != null)
      {
        var prod =
          products.FirstOrDefault(
            p =>
              string.Equals(p.Description.Trim(), product) &&
              string.Equals(p.Outcome, "Y"));

        if (prod != null)
          return true;
      }
      return false;
    }

    public ICreditResponse GetScore(string debtorFirstName, string debtorLastName, string debtorIdNumber,
      IAddress debtorResidentialAddress,
      IContact debtorContactCellNo, IContact debtorContactTelNoHome, IContact debtorContactTelNoWork, long branchId,
      bool newScore)
    {
      var request = new CreditRequestLegacy(CombGuid.Generate());
      using (var uow = new UnitOfWork())
      {
        request.Firstname = debtorFirstName;
        request.Surname = debtorLastName;
        request.IDNumber = debtorIdNumber;
        request.AccountId = null;
        if (debtorResidentialAddress != null)
        {
          request.AddressLine1 = debtorResidentialAddress.Line1;
          request.AddressLine2 = debtorResidentialAddress.Line2;
          request.Suburb = debtorResidentialAddress.Line3;
          request.City = debtorResidentialAddress.Line4;
          request.Province = debtorResidentialAddress.Province != null
            ? debtorResidentialAddress.Province.ShortCode
            : string.Empty;
          request.PostalCode = debtorResidentialAddress.PostalCode;
        }
        var branch = new XPQuery<BRN_Branch>(uow).FirstOrDefault(p => p.BranchId == branchId);
        if (branch != null)
          request.LegacyBranchNo = branch.LegacyBranchNum;

        var idValidation = new IDValidator(debtorIdNumber);

        request.DateOfBirth = idValidation.isValid() ? idValidation.GetDateOfBirthAsDateTime() : new DateTime();

        if (debtorContactCellNo != null)
        {
          request.CellNo = debtorContactCellNo.Value;
        }

        if (debtorContactTelNoHome != null && !string.IsNullOrEmpty(debtorContactTelNoHome.Value))
        {
          var code = debtorContactTelNoHome.Value.Substring(0, 3);
          var no = debtorContactTelNoHome.Value.Substring(3, (debtorContactTelNoHome.Value.Length - 3));
          request.HomeTelCode = code;
          request.HomeTelNo = no;
        }

        if (debtorContactTelNoWork != null && !string.IsNullOrEmpty(debtorContactTelNoWork.Value))
        {
          var code = debtorContactTelNoWork.Value.Substring(0, 3);
          var no = debtorContactTelNoWork.Value.Substring(3, (debtorContactTelNoWork.Value.Length - 3));
          request.WorkTelCode = code;
          request.WorkTelNo = no;
        }

        if (newScore)
        {
          return _bureauService.GetScore(request);
          //if (_streamRepository.DoesBudgetAllow(Stream.Budget.CompuscanEnquiries))
          //{
          //  var result = _bureauService.GetScore(request);
          //  _streamRepository.IncBudget(Stream.Budget.CompuscanEnquiries);
          //  return result;
          //}
          //else
          //{
          //  return _bureauService.RequestScore(request);
          //}
        }
        return _bureauService.RequestScore(request);
      }
    }
  }
}