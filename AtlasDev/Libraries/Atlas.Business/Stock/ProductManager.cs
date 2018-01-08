using Atlas.Business.Security.Role;
using Atlas.Domain.Model;
using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas.Business.StockServer;
using System.Data;
using Atlas.Domain.DTO;
using System.Collections;
using Atlas.Common.Attributes;

namespace Atlas.Business.Stock
{
  public static class ProductManager
  {
    #region Public methods

    /// <summary>
    /// Assign A product to a person
    /// </summary>
    /// <param name="allocatedProduct">Product to assign</param>
    /// <param name="allocatedTo">Person product is being assigned to</param>
    /// <param name="allocatedBy">Person assigning the product to the person</param>
    public static void AllocateProduct(ProductDTO allocatedProduct, PER_PersonDTO allocatedTo, PER_PersonDTO allocatedBy)
    {
      using (var UoW = new UnitOfWork())
      {
        if (allocatedProduct.ReceivedDT == null)
          throw new Exception("Product not yet received. A product needs to be received by branch before allocating");

        if (allocatedProduct.AllocatedDT != null)
          throw new Exception(string.Format("Product has been already allocated on {0}", allocatedProduct.ReceivedDT));

        if (allocatedProduct.ProductBatch.DeliverToBranch.Company.CompanyId != allocatedBy.Branch.Company.CompanyId)
          throw new Exception(string.Format("Product Batch belongs to Branch '{0}' a different branch compared to current receiving person",
            allocatedProduct.ProductBatch.DeliverToBranch.Company.Name));

        if (allocatedBy.PersonId == allocatedTo.PersonId)
          throw new Exception("Product cannot be allocated to the same person allocating the product");

        var product = new XPQuery<PRD_Product>(UoW).FirstOrDefault(p => p.ProductId == allocatedProduct.ProductId);
        product.AllocatedPerson = new XPQuery<PER_Person>(UoW).FirstOrDefault(p => p.PersonId == allocatedTo.PersonId);
        product.AllocatedBy = new XPQuery<PER_Person>(UoW).FirstOrDefault(p => p.PersonId == allocatedBy.PersonId);
        product.AllocatedDT = DateTime.Now;

        UoW.CommitChanges();
      }
    }

    /// <summary>
    /// Creates a batch for the list of new products and imports into DB
    /// </summary>
    /// <param name="productList">List of new products</param>
    /// <param name="productType">Type of Product</param>
    /// <param name="batchStatus">Status of Batch</param>
    /// <param name="courierCompany">Courier company for delivery stock</param>
    /// <param name="captureUser">User who captured the products</param>
    /// <param name="quantity">Number of products in the batch</param>
    /// <param name="comment">Comment made by capturer</param>
    /// <param name="deliveryBranch">Branch of delivery batch is going to</param>
    /// <param name="deliveryDate">Date of Delivery Initiation</param>
    /// <param name="trackingNum">Tracking number from Courier Company</param>
    public static void CaptureProducts(IList productList, ProductTypeDTO productType, Enumerators.General.ProductBatchStatus batchStatus, CPY_CompanyDTO courierCompany,
      PER_PersonDTO captureUser, int quantity, string comment, BranchCompany deliveryBranch = null, DateTime? deliveryDate = null,
      string trackingNum = null)
    {
      using (var UoW = new UnitOfWork())
      {
        var uniqueKeys = Utility.GetUniqueContraints(productList);
        var products = new XPQuery<PRD_Product>(UoW).Where(p => p.ProductBatch.ProductType.ProductTypeId == productType.ProductTypeId); ;
        if (!string.IsNullOrEmpty(uniqueKeys.FirstOrDefault().Item1))
        {
          var n = uniqueKeys.Select(u => u.Item1).ToArray();
          products = products.Where(p => n.Contains(p.SearchValue1));
        }
        if (!string.IsNullOrEmpty(uniqueKeys.FirstOrDefault().Item2))
        {
          var n = uniqueKeys.Select(u => u.Item2).ToArray();
          products = products.Where(p => n.Contains(p.SearchValue2));
        }

        if (products.ToList().Count > 0)
          throw new Exception("Product(s) already exists with another batch");

        var productBatch = new PRD_ProductBatch(UoW);
        productBatch.CapturedBy = UoW.Query<PER_Person>().FirstOrDefault(p => p.PersonId == captureUser.PersonId);
        productBatch.CapturedDT = DateTime.Now;
        productBatch.Comment = comment;
        productBatch.Courier = UoW.Query<CPY_Company>().FirstOrDefault(c => c.CompanyId == courierCompany.CompanyId);
        productBatch.ProductType = UoW.Query<PRD_ProductType>().FirstOrDefault(p => p.ProductTypeId == productType.ProductTypeId);
        productBatch.Quantity = quantity;
        productBatch.Status = batchStatus;

        if (deliveryBranch != null)
        {
          productBatch.DeliverToBranch = UoW.Query<BRN_Branch>().FirstOrDefault(b => b.BranchId == deliveryBranch.BranchId);
          productBatch.DeliveryDT = deliveryDate;
          productBatch.DeliverySetBy = UoW.Query<PER_Person>().FirstOrDefault(p => p.PersonId == captureUser.PersonId);
          productBatch.TrackingNum = trackingNum;
        }

        foreach (var productObj in productList)
        {
          var serializedProduct = Utility.Serialize(productObj.GetType(), productObj, true);
          var searchValues = Utility.GetSearchValues(productObj);

          var product = new PRD_Product(UoW);
          product.CreatedBy = productBatch.CapturedBy;
          product.CreatedDT = DateTime.Now;
          product.ProductBatch = productBatch;
          product.XmlObject = serializedProduct;
          product.SearchValue1 = searchValues[0];
          product.SearchValue2 = searchValues[1];
        }

        UoW.CommitChanges();
      }
    }

    /// <summary>
    /// Updates Product to DB
    /// </summary>
    /// <param name="productUpdate">Product to update</param>
    public static void UpdateProduct(long productId, string xmlObject, string searchValue1, string searchValue2, PER_PersonDTO editingUser)
    {
      using (var UoW = new UnitOfWork())
      {
        var selectedProduct = new XPQuery<PRD_Product>(UoW).FirstOrDefault(p => p.ProductId == productId);
        var lastEditedPerson = new XPQuery<PER_Person>(UoW).FirstOrDefault(p => p.PersonId == editingUser.PersonId);

        if (selectedProduct == null || lastEditedPerson == null)
        {
          throw new Exception("Product/Person does not exist in the Data Base");
        }
        selectedProduct.XmlObject = xmlObject;
        selectedProduct.LastEditedBy = lastEditedPerson;
        selectedProduct.LastEditedDT = DateTime.Now;
        selectedProduct.SearchValue1 = searchValue1;
        selectedProduct.SearchValue2 = searchValue2;

        UoW.CommitChanges();
      }
    }

    /// <summary>
    /// Marks a Liost of products of a batch as Received from branch that the batch was delivered to
    /// If all items of the batch are recevied by the branch, the batch is marked as "Delivered"
    /// </summary>
    /// <param name="productDTOList">List of Products to mark as received</param>
    /// <param name="receivedByPersonDTO">Person who received the product from the branch</param>
    /// <param name="receivedDT">Date the roduct was received</param>
    public static void ReceivedProduct(List<ProductDTO> productDTOList, PER_PersonDTO receivedByPersonDTO, DateTime receivedDT)
    {
      using (var UoW = new UnitOfWork())
      {
        var batchId = new List<long>();
        var productIds =  productDTOList.Select(r => r.ProductId).ToArray();
        var productList = new XPQuery<PRD_Product>(UoW).Where(p => productIds.Contains(p.ProductId));
        var receivedBy = new XPQuery<PER_Person>(UoW).FirstOrDefault(p => p.PersonId == receivedByPersonDTO.PersonId);

        if (receivedBy.Branch == null)
          throw new Exception("Receiving person does not have a company allocated. A company is required to be allocated to the person.");

        if (productList.FirstOrDefault().ProductBatch.DeliverToBranch.Company.CompanyId != receivedBy.Branch.Company.CompanyId)
          throw new Exception(string.Format("Product Batch belongs to Branch '{0}'. A different branch compared to current receiving person",
            productList.FirstOrDefault().ProductBatch.DeliverToBranch.Company.Name));

        foreach (var product in productList)
        {
          if (product.ReceivedDT != null)
            throw new Exception(string.Format("Product already received by branch on {0}", product.ReceivedDT));

          product.ReceivedBy = receivedBy;
          product.ReceivedDT = receivedDT;

          if (!batchId.Contains(product.ProductBatch.ProductBatchId))
            batchId.Add(product.ProductBatch.ProductBatchId);
        }

        var productBatchList = new XPQuery<PRD_ProductBatch>(UoW).Where(b => batchId.Contains(b.ProductBatchId));
        foreach (var batch in productBatchList)
        {
          var productsNotYetReceived = new XPQuery<PRD_Product>(UoW).Where(p => p.ProductBatch == batch && p.ReceivedDT == null).Count();
          if (productsNotYetReceived == 0)
          {
            batch.Status = Enumerators.General.ProductBatchStatus.Delivered;
          }
          else
          {
            if (batch.Status != Enumerators.General.ProductBatchStatus.Partially_Receipted)
              batch.Status = Enumerators.General.ProductBatchStatus.Partially_Receipted;
          }
        }

        UoW.CommitChanges();
      }
    }

    /// <summary>
    /// Basic Search for Batches of products
    /// If paramters are null, filter does not apply for that parameter
    /// </summary>
    /// <param name="productType">Type of product (Can never be null)</param>
    /// <param name="createDateStart">Start Date Range of the Batch Create Date. 
    ///                               If EndDate is Null, the startDate will be the actual Batch Create Date and not a range</param>
    /// <param name="createDateEnd">End Date Range of the Batch Create Date</param>
    /// <param name="deliveryDateStart">Start Date Range of the Batch Delivery Date. 
    ///                               If EndDate is Null, the startDate will be the actual Batch Delivery Date and not a range</param>
    /// <param name="deliveryDateEnd">End Date Range of the Batch Delivery Date</param>
    /// <param name="courier">The Courier company assigned to the batch</param>
    /// <param name="branch">The Branch of which the delivery Destination is linked</param>
    /// <param name="trackingNum">The Tracking Number provided by the Courier Company</param>
    /// <param name="batchStatus">Status of the Batch</param>
    /// <returns></returns>
    public static List<ProductBatchDTO> BatchSearch(ProductTypeDTO productType, DateTime? createDateStart, DateTime? createDateEnd,
      DateTime? deliveryDateStart, DateTime? deliveryDateEnd, CPY_CompanyDTO courier, BranchCompany branch, string trackingNum,
      Enumerators.General.ProductBatchStatus? batchStatus)
    {
      using (var UoW = new UnitOfWork())
      {
        var productBatchQuery = new XPQuery<PRD_ProductBatch>(UoW).Where(b => b.ProductType.ProductTypeId == productType.ProductTypeId);

        if (createDateStart != null)
        {
          if (createDateEnd == null)
          {
            productBatchQuery = productBatchQuery.Where(b => b.CapturedDT.Date == createDateStart.Value.Date);
          }
          else
          {
            productBatchQuery = productBatchQuery.Where(b => b.CapturedDT >= createDateStart);
          }
        }
        if (createDateEnd != null)
        {
          productBatchQuery = productBatchQuery.Where(b => b.CapturedDT <= createDateEnd);
        }
        if (deliveryDateStart != null)
        {
          if (deliveryDateEnd == null)
          {
            productBatchQuery = productBatchQuery.Where(b => b.DeliveryDT.HasValue ? b.DeliveryDT.Value.Date == deliveryDateStart.Value.Date : false);
          }
          else
          {
            productBatchQuery = productBatchQuery.Where(b => b.DeliveryDT >= deliveryDateStart);
          }
        }
        if (deliveryDateEnd != null)
        {
          productBatchQuery = productBatchQuery.Where(b => b.DeliveryDT <= deliveryDateEnd);
        }
        if (courier != null)
        {
          productBatchQuery = productBatchQuery.Where(b => b.Courier.CompanyId == courier.CompanyId);
        }
        if (branch != null)
        {
          productBatchQuery = productBatchQuery.Where(b => b.DeliverToBranch.BranchId == branch.BranchId);
        }
        if (!string.IsNullOrEmpty(trackingNum))
        {
          productBatchQuery = productBatchQuery.Where(b => b.TrackingNum == trackingNum);
        }
        if (batchStatus != null)
        {
          productBatchQuery = productBatchQuery.Where(b => b.Status == batchStatus);
        }

        var result = productBatchQuery.ToList();
        return AutoMapper.Mapper.Map<List<PRD_ProductBatch>, List<ProductBatchDTO>>(result);
      }
    }

    /// <summary>
    /// Updates the Batch according to What Data is passed in the DTO
    /// </summary>
    /// <param name="productBatchUpdate">Product Batch to Update</param>
    public static void UpdateBatch(ProductBatchDTO productBatchUpdate)
    {
      using (var UoW = new UnitOfWork())
      {
        var productBatch = new XPQuery<PRD_ProductBatch>(UoW).FirstOrDefault(b => b.ProductBatchId == productBatchUpdate.ProductBatchId);
        if (productBatch == null)
          throw new Exception(string.Format("ProductBatchId {0} does not exist in DB", productBatch.ProductBatchId));

        productBatch.Status = productBatchUpdate.Status;
        productBatch.TrackingNum = productBatchUpdate.TrackingNum;

        UoW.CommitChanges();
      }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Creates a source request for WCF
    /// </summary>
    /// <param name="branchCode">BranchNum for allocated stock</param>
    /// <param name="appVersion">Calling application version</param>
    /// <returns></returns>
    private static SourceRequest GetSourceRequest(string branchCode, string appVersion)
    {
      return new SourceRequest()
      {
        BranchCode = branchCode,
        AppName = Enumerators.General.ApplicationIdentifiers.AtlasManagement.ToStringEnum(),
        AppVer = appVersion,
        MachineDateTime = DateTime.Now,
        MachineIPAddresses = "127.0.0.1",
        MachineName = Environment.MachineName,
        //MachineUniqueID = RoleContext.LoggedInUser.Security.MachineStore.HardwareKey,
        UserIDOrPassport = RoleContext.LoggedInUser.IdNum
      };
    }

    #endregion
  }
}
