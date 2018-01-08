using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Common.Extensions;
using DevExpress.Xpo;
using Falcon.Common.Interfaces.Services;
using Stream.Domain.Models;
using Stream.Framework.Repository;
using Stream.Framework.Structures;
using Stream.Structures.Models;
using Atlas.Domain.Model;
using Atlas.Enumerators;
using Falcon.Common.Interfaces.Structures;
using Falcon.Common.Structures;
using Falcon.Common.Structures.Branch;
using Serilog;
using Stream.Framework.DataContracts.Requests;
using Action = Stream.Framework.Enumerators.Action;
using CaseStatus = Stream.Framework.Enumerators.CaseStatus;
using ContactType = Atlas.Domain.Model.ContactType;
using Host = Atlas.Domain.Model.Host;

namespace Stream.Repository
{
  // TODO: add host to calling methods
  public class StreamRepository : IStreamRepository
  {
    private ISmsService _smsService;
    private readonly IUserService _userService;
    private readonly IPdfService _pdfService;
    private readonly ILogger _logger;

    public StreamRepository(ISmsService smsService, IUserService userService, IPdfService pdfService, ILogger logger)
    {
      _smsService = smsService;
      _userService = userService;
      _pdfService = pdfService;
      _logger = logger;
    }


    #region Public methods

    /// <summary>
    /// Adds a new debtor to the DB, otherwise, if the debtor exists, it just updates the current record
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public IDebtor AddOrUpdateDebtor(AddOrUpdateDebtorRequest request)
    {
      using (var uow = new UnitOfWork())
      {
        var debtor = new XPQuery<STR_Debtor>(uow).FirstOrDefault(d => request.Debtor.DebtorId == d.DebtorId) ??
                     new STR_Debtor(uow)
                     {
                       CreateDate = DateTime.Now
                     };

        debtor.Title = request.Debtor.Title;
        debtor.FirstName = request.Debtor.FirstName;
        debtor.IdNumber = request.Debtor.IdNumber;
        debtor.LastName = request.Debtor.LastName;
        debtor.OtherName = request.Debtor.OtherName;
        debtor.Reference = request.Debtor.Reference;
        debtor.EmployerCode = request.Debtor.EmployerCode;
        debtor.DateOfBirth = request.Debtor.DateOfBirth;
        debtor.ThirdPartyReferenceNo = request.Debtor.ThirdPartyReferenceNo;

        uow.CommitChanges();

        return new Debtor
        {
          DebtorId = debtor.DebtorId,
          CreateDate = debtor.CreateDate,
          Title = debtor.Title,
          FirstName = debtor.FirstName,
          IdNumber = debtor.IdNumber,
          LastName = debtor.LastName,
          OtherName = debtor.OtherName,
          Reference = debtor.Reference,
          EmployerCode = debtor.EmployerCode,
          DateOfBirth = debtor.DateOfBirth,
          ThirdPartyReferenceNo = debtor.ThirdPartyReferenceNo,
        };
      }
    }

    /// <summary>
    /// Adds a new account to the DB, otherwise, if the debtor exists, it just updates the current record
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public IStreamAccount AddOrUpdateAccount(AddOrUpdateAccountRequest request)
    {
      using (var uow = new UnitOfWork())
      {
        var account = new XPQuery<STR_Account>(uow).FirstOrDefault(d => request.AccountId == d.AccountId);
        if (account == null)
        {
          if (!string.IsNullOrWhiteSpace(request.Reference))
          {
            account = new XPQuery<STR_Account>(uow).FirstOrDefault(d => request.Reference == d.Reference) ??
                      new STR_Account(uow);
          }
          else if (request.Reference2 > 0)
          {
            account = new XPQuery<STR_Account>(uow).FirstOrDefault(d => request.Reference2 == d.Reference2) ??
                      new STR_Account(uow);
          }
          else
          {
            account = new STR_Account(uow);
          }
        }

        if (account.Branch == null || (account.Branch.BranchId != request.BranchId))
        {
          account.Branch = new XPQuery<BRN_Branch>(uow).FirstOrDefault(b => b.BranchId == request.BranchId);
        }
        if (account.Debtor == null || (account.Debtor.DebtorId != request.DebtorId))
        {
          account.Debtor = new XPQuery<STR_Debtor>(uow).FirstOrDefault(d => d.DebtorId == request.DebtorId);
        }
        if (account.Host == null || (account.Host.HostId != request.HostId))
        {
          account.Host = new XPQuery<Host>(uow).FirstOrDefault(h => h.HostId == request.HostId);
        }
        account.LoanAmount = request.LoanAmount;
        account.LoanDate = request.LoanDate;
        account.OpenDate = request.OpenDate;
        account.CloseDate = request.CloseDate;
        account.Reference = request.Reference;
        account.Reference2 = request.Reference2;
        account.LoanTerm = request.LoanTerm;
        if (account.Frequency == null || (account.Frequency.FrequencyId != request.Frequency.ToInt()))
        {
          account.Frequency =
            new XPQuery<STR_Frequency>(uow).FirstOrDefault(f => f.FrequencyId == request.Frequency.ToInt());
        }
        account.LastImportReference = request.LastImportReference;
        account.LastReceiptAmount = request.LastReceiptAmount;
        account.LastReceiptDate = request.LastReceiptDate;
        account.ArrearsAmount = request.ArrearsAmount;
        account.Balance = request.Balance;
        account.InstalmentsOutstanding = request.InstalmentsOutstanding;
        account.RequiredPayment = request.RequiredPayment;

        if (request.UpToDate.HasValue)
        {
          account.UpToDate = request.UpToDate.Value;
        }

        uow.CommitChanges();

        return new StreamAccount
        {
          Branch = new Branch
          {
            BranchId = account.Branch.BranchId,
            Region = account.Branch.Region.Description,
            LegacyBranchNum = account.Branch.LegacyBranchNum,
            Name = account.Branch.Company.Name,
            RegionId = account.Branch.Region.RegionId

          },
          Debtor = new Debtor
          {
            DebtorId = account.Debtor.DebtorId,
            CreateDate = account.Debtor.CreateDate,
            Title = account.Debtor.Title,
            FirstName = account.Debtor.FirstName,
            IdNumber = account.Debtor.IdNumber,
            LastName = account.Debtor.LastName,
            OtherName = account.Debtor.OtherName,
            Reference = account.Debtor.Reference,
            EmployerCode = account.Debtor.EmployerCode,
            DateOfBirth = account.Debtor.DateOfBirth,
            ThirdPartyReferenceNo = account.Debtor.ThirdPartyReferenceNo,
          },
          HostId = account.Host.HostId,
          LoanAmount = account.LoanAmount,
          LoanDate = account.LoanDate,
          OpenDate = account.OpenDate,
          CloseDate = account.CloseDate,
          Reference = account.Reference,
          Reference2 = account.Reference2,
          LoanTerm = account.LoanTerm,
          Frequency = new Frequency
          {
            Description = account.Frequency.Description,
            Type = account.Frequency.Type,
            FrequencyId = account.Frequency.FrequencyId
          },
          AccountId = account.AccountId,
          LastImportReference = account.LastImportReference,
          LastReceiptAmount = account.LastReceiptAmount,
          LastReceiptDate = account.LastReceiptDate,
          ArrearsAmount = account.ArrearsAmount,
          Balance = account.Balance,
          InstalmentsOutstanding = account.InstalmentsOutstanding,
          RequiredPayment = account.RequiredPayment
        };
      }
    }

    public IStreamAccount GetAccount(long accountId)
    {
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        uow.TrackPropertiesModifications = false;
        var account = new XPQuery<STR_Account>(uow).FirstOrDefault(d => accountId == d.AccountId);

        if (account != null)
        {
          return new StreamAccount
          {
            Branch = new Branch
            {
              BranchId = account.Branch.BranchId,
              Region = account.Branch.Region.Description,
              LegacyBranchNum = account.Branch.LegacyBranchNum,
              Name = account.Branch.Company.Name,
              RegionId = account.Branch.Region.RegionId

            },
            Debtor = new Debtor
            {
              DebtorId = account.Debtor.DebtorId,
              CreateDate = account.Debtor.CreateDate,
              Title = account.Debtor.Title,
              FirstName = account.Debtor.FirstName,
              IdNumber = account.Debtor.IdNumber,
              LastName = account.Debtor.LastName,
              OtherName = account.Debtor.OtherName,
              Reference = account.Debtor.Reference,
              EmployerCode = account.Debtor.EmployerCode,
              DateOfBirth = account.Debtor.DateOfBirth,
              ThirdPartyReferenceNo = account.Debtor.ThirdPartyReferenceNo,
            },
            HostId = account.Host.HostId,
            LoanAmount = account.LoanAmount,
            LoanDate = account.LoanDate,
            OpenDate = account.OpenDate,
            CloseDate = account.CloseDate,
            Reference = account.Reference,
            Reference2 = account.Reference2,
            LoanTerm = account.LoanTerm,
            Frequency = new Frequency
            {
              Description = account.Frequency.Description,
              Type = account.Frequency.Type,
              FrequencyId = account.Frequency.FrequencyId
            },
            AccountId = account.AccountId,
            LastImportReference = account.LastImportReference,
            LastReceiptAmount = account.LastReceiptAmount,
            LastReceiptDate = account.LastReceiptDate,
            ArrearsAmount = account.ArrearsAmount,
            Balance = account.Balance,
            InstalmentsOutstanding = account.InstalmentsOutstanding,
            RequiredPayment = account.RequiredPayment
          };
        }
        return null;
      }
    }

    public ICollection<IStreamAccount> GetAccountsByCaseId(long caseId, bool? upToDate = null)
    {
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        uow.TrackPropertiesModifications = false;
        var cases =
          new XPQuery<STR_Case>(uow).Where(
            c => c.CaseId == caseId).ToList();

        if (upToDate.HasValue)
        {
          cases = cases.Where(c => c.Debtor.Accounts.Any(a => a.UpToDate == upToDate)).ToList();
        }

        var accounts = cases
          .SelectMany(d => d.Debtor.Accounts)
          .Distinct()
          .ToList();

        var streamAccounts = new List<IStreamAccount>();

        streamAccounts.AddRange(accounts.Select(account => new StreamAccount
        {
          Branch = new Branch
          {
            BranchId = account.Branch.BranchId,
            Region = account.Branch.Region.Description,
            LegacyBranchNum = account.Branch.LegacyBranchNum,
            Name = account.Branch.Company.Name,
            RegionId = account.Branch.Region.RegionId

          },
          Debtor = new Debtor
          {
            DebtorId = account.Debtor.DebtorId,
            CreateDate = account.Debtor.CreateDate,
            Title = account.Debtor.Title,
            FirstName = account.Debtor.FirstName,
            IdNumber = account.Debtor.IdNumber,
            LastName = account.Debtor.LastName,
            OtherName = account.Debtor.OtherName,
            Reference = account.Debtor.Reference,
            EmployerCode = account.Debtor.EmployerCode,
            DateOfBirth = account.Debtor.DateOfBirth,
            ThirdPartyReferenceNo = account.Debtor.ThirdPartyReferenceNo,
          },
          HostId = account.Host.HostId,
          LoanAmount = account.LoanAmount,
          LoanDate = account.LoanDate,
          OpenDate = account.OpenDate,
          CloseDate = account.CloseDate,
          Reference = account.Reference,
          Reference2 = account.Reference2,
          LoanTerm = account.LoanTerm,
          Frequency = new Frequency
          {
            Description = account.Frequency.Description,
            Type = account.Frequency.Type,
            FrequencyId = account.Frequency.FrequencyId
          },
          AccountId = account.AccountId,
          LastImportReference = account.LastImportReference,
          LastReceiptAmount = account.LastReceiptAmount,
          LastReceiptDate = account.LastReceiptDate,
          ArrearsAmount = account.ArrearsAmount,
          Balance = account.Balance,
          InstalmentsOutstanding = account.InstalmentsOutstanding,
          RequiredPayment = account.RequiredPayment
        }));

        return streamAccounts;
      }
    }

    public ICase GetCase(long caseId)
    {
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        uow.TrackPropertiesModifications = false;
        var streamCase = new XPQuery<STR_Case>(uow).FirstOrDefault(d => d.CaseId == caseId);

        if (streamCase != null)
        {
          return new Case
          {
            Host = streamCase.Host.Type,
            BranchId = streamCase.Branch.BranchId,
            Reference = streamCase.Reference,
            DebtorId = streamCase.Debtor.DebtorId,
            GroupType = streamCase.Group.GroupType,
            CaseId = streamCase.CaseId,
            AllocatedUserId = streamCase.AllocatedUser.PersonId,
            AllocatedUser = streamCase.AllocatedUser.Security.Username,
            CreateDate = streamCase.CreateDate,
            TotalLoanAmount = streamCase.TotalLoanAmount,
            TotalBalance = streamCase.TotalBalance,
            TotalArrearsAmount = streamCase.TotalArrearsAmount,
            CaseStatusType = streamCase.CaseStatus.Status,
            Priority =
              streamCase.Priority != null
                ? streamCase.Priority.PriorityType
                : (Framework.Enumerators.Stream.PriorityType?)null,
            CategoryType = (Framework.Enumerators.Category.Type)streamCase.SubCategory.Category.CategoryId,
            CompleteDate = streamCase.CompleteDate,
            TotalInstalmentsOutstanding = streamCase.TotalInstalmentsOutstanding,
            LastReceiptAmount = streamCase.LastReceiptAmount,
            LastReceiptDate = streamCase.LastReceiptDate,
            LastStatusDate = streamCase.LastStatusDate,
            TotalRequiredPayment = streamCase.TotalRequiredPayment,
            SmsCount = streamCase.SMSCount,
            SubCategoryType = (Framework.Enumerators.Stream.SubCategory)streamCase.SubCategory.SubCategoryId
          };
        }
        return null;
      }
    }

    public ICollection<ICase> GetCasesByDebtorId(long debtorId, Framework.Enumerators.Stream.GroupType? groupType = null, params CaseStatus.Type[] caseStatuses)
    {
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        uow.TrackPropertiesModifications = false;
        var streamCaseQuery = new XPQuery<STR_Case>(uow).Where(d => d.Debtor.DebtorId == debtorId).AsQueryable();
        if (groupType.HasValue)
        {
          streamCaseQuery = streamCaseQuery.Where(c => c.Group.GroupId == groupType.Value.ToInt()).AsQueryable();
        }
        if (caseStatuses.Length > 0)
        {
          var caseStatusIds = caseStatuses.Select(c => c.ToInt());
          streamCaseQuery = streamCaseQuery.Where(c => caseStatusIds.Contains(c.CaseStatus.CaseStatusId)).AsQueryable();
        }

        var streamCases = streamCaseQuery.ToList();

        var cases = streamCases.Select(streamCase =>
          new Case
          {
            Host = streamCase.Host.Type,
            Reference = streamCase.Reference,
            BranchId = streamCase.Branch == null ? 0 : streamCase.Branch.BranchId,
            DebtorId = streamCase.Debtor == null ? 0 : streamCase.Debtor.DebtorId,
            GroupType =
              streamCase.Group == null ? (Framework.Enumerators.Stream.GroupType?)null : streamCase.Group.GroupType,
            CaseId = streamCase.CaseId,
            AllocatedUserId = streamCase.AllocatedUser == null ? 0 : streamCase.AllocatedUser.PersonId,
            AllocatedUser = streamCase.AllocatedUser == null ? "" : streamCase.AllocatedUser.Security.Username,
            CreateDate = streamCase.CreateDate,
            TotalLoanAmount = streamCase.TotalLoanAmount,
            TotalBalance = streamCase.TotalBalance,
            TotalArrearsAmount = streamCase.TotalArrearsAmount,
            CaseStatusType =
              streamCase.CaseStatus == null
                ? (CaseStatus.Type?)null
                : streamCase.CaseStatus.Status,
            Priority = streamCase.Priority != null ? streamCase.Priority.PriorityType : (Framework.Enumerators.Stream.PriorityType?)null,
            CategoryType =
              streamCase.SubCategory == null
                ? (Framework.Enumerators.Category.Type?)null
                : (Framework.Enumerators.Category.Type?)streamCase.SubCategory.Category.CategoryId,
            CompleteDate = streamCase.CompleteDate,
            TotalInstalmentsOutstanding = streamCase.TotalInstalmentsOutstanding,
            LastReceiptAmount = streamCase.LastReceiptAmount,
            LastReceiptDate = streamCase.LastReceiptDate,
            LastStatusDate = streamCase.LastStatusDate,
            TotalRequiredPayment = streamCase.TotalRequiredPayment,
            SmsCount = streamCase.SMSCount,
            SubCategoryType = (Framework.Enumerators.Stream.SubCategory)streamCase.SubCategory.SubCategoryId
          }).ToList();
        return new List<ICase>(cases);
      }
    }

    public ICase GetCaseByAccountId(long accountId)
    {
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        uow.TrackPropertiesModifications = false;
        var streamCase =
          new XPQuery<STR_Case>(uow).FirstOrDefault(d => d.Debtor.Accounts.Any(a => a.AccountId == accountId));

        if (streamCase != null)
        {
          return new Case
          {
            Host = streamCase.Host.Type,
            Reference = streamCase.Reference,
            BranchId = streamCase.Branch == null ? 0 : streamCase.Branch.BranchId,
            DebtorId = streamCase.Debtor == null ? 0 : streamCase.Debtor.DebtorId,
            GroupType =
              streamCase.Group == null ? (Framework.Enumerators.Stream.GroupType?)null : streamCase.Group.GroupType,
            CaseId = streamCase.CaseId,
            AllocatedUserId = streamCase.AllocatedUser == null ? 0 : streamCase.AllocatedUser.PersonId,
            AllocatedUser =
              streamCase.AllocatedUser == null
                ? ""
                : (streamCase.AllocatedUser.Security == null ? "" : streamCase.AllocatedUser.Security.Username),
            CreateDate = streamCase.CreateDate,
            TotalLoanAmount = streamCase.TotalLoanAmount,
            TotalBalance = streamCase.TotalBalance,
            TotalArrearsAmount = streamCase.TotalArrearsAmount,
            CaseStatusType =
              streamCase.CaseStatus == null
                ? (CaseStatus.Type?)null
                : streamCase.CaseStatus.Status,
            Priority = streamCase.Priority != null ? streamCase.Priority.PriorityType : (Framework.Enumerators.Stream.PriorityType?)null,
            CategoryType = (Framework.Enumerators.Category.Type)streamCase.SubCategory.Category.CategoryId,
            CompleteDate = streamCase.CompleteDate,
            TotalInstalmentsOutstanding = streamCase.TotalInstalmentsOutstanding,
            LastReceiptAmount = streamCase.LastReceiptAmount,
            LastReceiptDate = streamCase.LastReceiptDate,
            LastStatusDate = streamCase.LastStatusDate,
            TotalRequiredPayment = streamCase.TotalRequiredPayment,
            SmsCount = streamCase.SMSCount,
            SubCategoryType = (Framework.Enumerators.Stream.SubCategory)streamCase.SubCategory.SubCategoryId
          };
        }
        return null;
      }
    }

    public ICollection<ICase> GetCasesByStatus(Framework.Enumerators.Stream.GroupType groupType,
      params CaseStatus.Type[] caseStatusTypes)
    {
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        uow.TrackPropertiesModifications = false;
        var caseStatusTypeIds = caseStatusTypes.Select(c => c.ToInt());
        var streamCases =
          new XPQuery<STR_Case>(uow).Where(
            d => d.Group.GroupId == groupType.ToInt() && caseStatusTypeIds.Contains(d.CaseStatus.CaseStatusId)).ToList();

        return new List<ICase>(
          streamCases.Select(streamCase => new Case
          {
            Host = streamCase.Host.Type,
            Reference = streamCase.Reference,
            BranchId = streamCase.Branch == null ? 0 : streamCase.Branch.BranchId,
            DebtorId = streamCase.Debtor == null ? 0 : streamCase.Debtor.DebtorId,
            GroupType =
              streamCase.Group == null ? (Framework.Enumerators.Stream.GroupType?)null : streamCase.Group.GroupType,
            CaseId = streamCase.CaseId,
            AllocatedUserId = streamCase.AllocatedUser == null ? 0 : streamCase.AllocatedUser.PersonId,
            AllocatedUser = streamCase.AllocatedUser == null ? "" : streamCase.AllocatedUser.Security.Username,
            CreateDate = streamCase.CreateDate,
            TotalLoanAmount = streamCase.TotalLoanAmount,
            TotalBalance = streamCase.TotalBalance,
            TotalArrearsAmount = streamCase.TotalArrearsAmount,
            CaseStatusType =
              streamCase.CaseStatus == null
                ? (CaseStatus.Type?)null
                : streamCase.CaseStatus.Status,
            Priority = streamCase.Priority != null ? streamCase.Priority.PriorityType : (Framework.Enumerators.Stream.PriorityType?)null,
            CategoryType =
              streamCase.SubCategory == null
                ? (Framework.Enumerators.Category.Type?)null
                : (Framework.Enumerators.Category.Type?)streamCase.SubCategory.Category.CategoryId,
            CompleteDate = streamCase.CompleteDate,
            TotalInstalmentsOutstanding = streamCase.TotalInstalmentsOutstanding,
            LastReceiptAmount = streamCase.LastReceiptAmount,
            LastReceiptDate = streamCase.LastReceiptDate,
            LastStatusDate = streamCase.LastStatusDate,
            TotalRequiredPayment = streamCase.TotalRequiredPayment,
            SmsCount = streamCase.SMSCount,
            SubCategoryType = (Framework.Enumerators.Stream.SubCategory)streamCase.SubCategory.SubCategoryId
          }).ToList()
          );
      }
    }

    /// <summary>
    /// Adds a new case to the DB, otherwise, if the debtor exists, it just updates the current record
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public ICase AddOrUpdateCase(AddOrUpdateCaseRequest request)
    {
      using (var uow = new UnitOfWork())
      {
        var streamCase = new XPQuery<STR_Case>(uow).FirstOrDefault(d => request.CaseId == d.CaseId) ??
                         new STR_Case(uow)
                         {
                           CreateDate = DateTime.Now
                         };

        streamCase.Reference = request.Reference;
        if (streamCase.Host == null || streamCase.Host.HostId != request.Host.ToInt())
        {
          streamCase.Host = new XPQuery<Host>(uow).FirstOrDefault(b => b.HostId == request.Host.ToInt());
        }
        if (streamCase.Branch == null || streamCase.Branch.BranchId != request.BranchId)
        {
          streamCase.Branch = new XPQuery<BRN_Branch>(uow).FirstOrDefault(b => b.BranchId == request.BranchId);
        }
        if (streamCase.Debtor == null || streamCase.Debtor.DebtorId != request.DebtorId)
        {
          streamCase.Debtor = new XPQuery<STR_Debtor>(uow).FirstOrDefault(d => d.DebtorId == request.DebtorId);
        }
        if (streamCase.AllocatedUser == null || streamCase.AllocatedUser.PersonId != request.AllocatedUserId)
        {
          streamCase.AllocatedUser =
            new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == request.AllocatedUserId);
        }

        if (request.WorkableCase.HasValue && !streamCase.WorkableCase.HasValue)
        {
          streamCase.WorkableCase = request.WorkableCase.Value;
        }

        streamCase.TotalBalance = request.TotalBalance;

        if (request.CaseStatus.HasValue && (streamCase.CaseStatus == null || request.CaseStatus.Value.ToInt() != streamCase.CaseStatus.CaseStatusId))
        {
          streamCase.CaseStatus =
            new XPQuery<STR_CaseStatus>(uow).FirstOrDefault(c => c.CaseStatusId == request.CaseStatus.ToInt());
          streamCase.LastStatusDate = DateTime.Now;
          if (streamCase.CaseStatus != null)
          {
            if (request.CaseStatus != null)
            {
              AddAccountNote(General.Person.System.ToInt(),
                string.Format("changed status in AddOrUpdateCase, {0} to {1}", streamCase.CaseStatus.Description,
                  request.CaseStatus.Value.ToStringEnum()), Framework.Enumerators.Stream.AccountNoteType.Normal, uow,
                request.CaseId);
            }
            else
            {

              AddAccountNote(General.Person.System.ToInt(),
                string.Format("changed status in AddOrUpdateCase, from {0}", streamCase.CaseStatus.Description), Framework.Enumerators.Stream.AccountNoteType.Normal, uow,
                request.CaseId);
            }
          }
          else
          {

            AddAccountNote(General.Person.System.ToInt(),
              "changed status in AddOrUpdateCase, from null", Framework.Enumerators.Stream.AccountNoteType.Normal, uow,
              request.CaseId);
          }
        }

        if (streamCase.Group == null || streamCase.Group.GroupId != request.GroupType.ToInt())
        {
          streamCase.Group = new XPQuery<STR_Group>(uow).FirstOrDefault(g => g.GroupId == request.GroupType.ToInt());
        }
        streamCase.TotalArrearsAmount = request.TotalArrearsAmount;
        streamCase.TotalRequiredPayment = request.TotalRequiredPayment;
        streamCase.LastReceiptDate = request.LastReceiptDate;
        streamCase.LastReceiptAmount = request.LastReceiptAmount;
        streamCase.TotalInstalmentsOutstanding = request.TotalInstalmentsOutstanding;
        streamCase.TotalLoanAmount = request.TotalLoanAmount;
        streamCase.SMSCount = request.SmsCount;
        if (streamCase.SubCategory == null || request.SubCategory.HasValue && streamCase.SubCategory.SubCategoryId != request.SubCategory.ToInt())
        {
          streamCase.SubCategory =
            new XPQuery<STR_SubCategory>(uow).FirstOrDefault(s => s.SubCategoryId == request.SubCategory.ToInt());
        }
        if (streamCase.Priority == null ||
            (streamCase.Priority.PriorityId !=
             (request.Priority ?? Framework.Enumerators.Stream.PriorityType.Normal).ToInt()))
        {
          var priorityId = (request.Priority ?? Framework.Enumerators.Stream.PriorityType.Normal).ToInt();
          streamCase.Priority =
            new XPQuery<STR_Priority>(uow).FirstOrDefault(
              s => s.PriorityId == priorityId);
        }

        uow.CommitChanges();

        return new Case
        {
          Host = streamCase.Host == null ? General.Host.ASS : streamCase.Host.Type,
          Reference = streamCase.Reference,
          BranchId = streamCase.Branch == null ? 0 : streamCase.Branch.BranchId,
          DebtorId = streamCase.Debtor == null ? 0 : streamCase.Debtor.DebtorId,
          GroupType =
            streamCase.Group == null ? (Framework.Enumerators.Stream.GroupType?)null : streamCase.Group.GroupType,
          CaseId = streamCase.CaseId,
          AllocatedUserId = streamCase.AllocatedUser == null ? 0 : streamCase.AllocatedUser.PersonId,
          AllocatedUser = streamCase.AllocatedUser == null ? "" : streamCase.AllocatedUser.Security.Username,
          CreateDate = streamCase.CreateDate,
          TotalLoanAmount = streamCase.TotalLoanAmount,
          TotalBalance = streamCase.TotalBalance,
          TotalArrearsAmount = streamCase.TotalArrearsAmount,
          CaseStatusType =
            streamCase.CaseStatus == null ? (CaseStatus.Type?)null : streamCase.CaseStatus.Status,
          Priority = streamCase.Priority != null ? streamCase.Priority.PriorityType : (Framework.Enumerators.Stream.PriorityType?)null,
          CategoryType =
            streamCase.SubCategory == null
              ? null
              : (Framework.Enumerators.Category.Type?)streamCase.SubCategory.Category.CategoryId,
          CompleteDate = streamCase.CompleteDate,
          TotalInstalmentsOutstanding = streamCase.TotalInstalmentsOutstanding,
          LastReceiptAmount = streamCase.LastReceiptAmount,
          LastReceiptDate = streamCase.LastReceiptDate,
          LastStatusDate = streamCase.LastStatusDate,
          TotalRequiredPayment = streamCase.TotalRequiredPayment,
          SmsCount = streamCase.SMSCount,
          WorkableCase = streamCase.WorkableCase,
          SubCategoryType = (Framework.Enumerators.Stream.SubCategory)streamCase.SubCategory.SubCategoryId
        };
      }
    }

    public ICaseStream GetOpenCaseStreamForCase(long caseId)
    {
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        uow.TrackPropertiesModifications = false;
        var caseStream =
          new XPQuery<STR_CaseStream>(uow).FirstOrDefault(c => c.Case.CaseId == caseId && !c.CompleteDate.HasValue);

        if (caseStream == null)
          return null;

        return new CaseStream
        {
          CreateUser = caseStream.CreateUser == null ? "" : caseStream.CreateUser.Security.Username,
          CaseId = caseStream.Case.CaseId,
          CreateDate = caseStream.CreateDate,
          CompleteDate = caseStream.CompleteDate,
          PriorityType =
            caseStream.Priority == null
              ? Framework.Enumerators.Stream.PriorityType.Normal
              : caseStream.Priority.PriorityType,
          StreamType = (Framework.Enumerators.Stream.StreamType)caseStream.Stream.StreamId,
          CompletedUserId = caseStream.CompletedUser == null ? 0 : caseStream.CompletedUser.PersonId,
          CompletedUser = caseStream.CompletedUser == null ? "" : caseStream.CompletedUser.Security.Username,
          CreateUserId = caseStream.CreateUser == null ? 0 : caseStream.CreateUser.PersonId,
          EscalationType = caseStream.Escalation.EscalationType,
          CaseStreamId = caseStream.CaseStreamId,
          CompleteComment = caseStream.CompleteComment == null ? "" : caseStream.CompleteComment.Description,
          LastPriorityDate = caseStream.LastPriorityDate,
          CompleteNote = caseStream.CompleteNote == null ? "" : caseStream.CompleteNote.Note
        };
      }
    }

    /// <summary>
    /// Adds a new case stream to the DB, otherwise, if the debtor exists, it just updates the current record
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public ICaseStream AddOrUpdateCaseStream(AddOrUpdateCaseStreamRequest request)
    {
      using (var uow = new UnitOfWork())
      {
        var caseStream = new XPQuery<STR_CaseStream>(uow).FirstOrDefault(d => request.CaseStreamId == d.CaseStreamId) ??
                         new STR_CaseStream(uow)
                         {
                           CreateDate = DateTime.Now,
                           CreateUser =
                             new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == request.CreateUserId)
                         };

        if (caseStream.Case == null || caseStream.Case.CaseId == request.CaseId)
        {
          caseStream.Case = new XPQuery<STR_Case>(uow).FirstOrDefault(c => c.CaseId == request.CaseId);
        }
        caseStream.LastPriorityDate = request.LastPriorityDate;
        if (caseStream.Escalation == null || caseStream.Escalation.EscalationId == request.EscalationType.ToInt())
        {
          caseStream.Escalation =
            new XPQuery<STR_Escalation>(uow).FirstOrDefault(e => e.EscalationId == request.EscalationType.ToInt());
        }
        if (caseStream.Priority == null || caseStream.Priority.PriorityId == request.PriorityType.ToInt())
        {
          caseStream.Priority =
            new XPQuery<STR_Priority>(uow).FirstOrDefault(p => p.PriorityId == request.PriorityType.ToInt());
        }
        if (caseStream.Stream == null || caseStream.Stream.StreamId == request.StreamType.ToInt())
        {
          caseStream.Stream = new XPQuery<STR_Stream>(uow).FirstOrDefault(s => s.StreamId == request.StreamType.ToInt());
        }

        if (request.CompleteDate.HasValue)
        {
          caseStream.CompleteDate = request.CompleteDate;
          caseStream.CompletedUser =
            new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == request.CompletedUserId);
          caseStream.CompleteComment =
            new XPQuery<STR_Comment>(uow).FirstOrDefault(c => c.CommentId == request.CompleteCommentId);
          caseStream.CompleteNote = new STR_Note
          {
            CreateDate = DateTime.Now,
            Case = caseStream.Case,
            AccountNoteType =
              new XPQuery<STR_AccountNoteType>(uow).FirstOrDefault(
                a => a.AccountNoteTypeId == Framework.Enumerators.Stream.AccountNoteType.Normal.ToInt()),
            Note = request.CompleteNote,
            CreateUser = caseStream.CompletedUser
          };
        }

        uow.CommitChanges();

        return new CaseStream
        {
          CreateUser = caseStream.CreateUser == null ? "" : caseStream.CreateUser.Security.Username,
          CaseId = caseStream.Case == null ? 0 : caseStream.Case.CaseId,
          CreateDate = caseStream.CreateDate,
          CompleteDate = caseStream.CompleteDate,
          PriorityType =
            caseStream.Priority == null
              ? Framework.Enumerators.Stream.PriorityType.Normal
              : caseStream.Priority.PriorityType,
          StreamType = (Framework.Enumerators.Stream.StreamType)caseStream.Stream.StreamId,
          CompletedUserId = caseStream.CompletedUser == null ? 0 : caseStream.CompletedUser.PersonId,
          CompletedUser = caseStream.CompletedUser == null ? "" : caseStream.CompletedUser.Security.Username,
          CreateUserId = caseStream.CreateUser == null ? 0 : caseStream.CreateUser.PersonId,
          EscalationType = caseStream.Escalation.EscalationType,
          CaseStreamId = caseStream.CaseStreamId,
          CompleteComment = caseStream.CompleteComment == null ? "" : caseStream.CompleteComment.Description,
          LastPriorityDate = caseStream.LastPriorityDate,
          CompleteNote = caseStream.CompleteNote == null ? "" : caseStream.CompleteNote.Note
        };
      }
    }

    public ICaseStreamAllocation AddOrUpdateCaseStreamAllocation(AddOrUpdateCaseStreamAllocationRequest request)
    {
      using (var uow = new UnitOfWork())
      {
        var caseStreamAllocation =
          new XPQuery<STR_CaseStreamAllocation>(uow).FirstOrDefault(
            d => request.CaseStreamAllocationId == d.CaseStreamAllocationId) ??
          new STR_CaseStreamAllocation(uow);

        if (caseStreamAllocation.CaseStream == null ||
            caseStreamAllocation.CaseStream.CaseStreamId != request.CaseStreamId)
        {
          caseStreamAllocation.CaseStream =
            new XPQuery<STR_CaseStream>(uow).FirstOrDefault(c => c.CaseStreamId == request.CaseStreamId);
        }
        caseStreamAllocation.AllocatedDate = request.AllocatedDate;
        if (caseStreamAllocation.AllocatedUser == null ||
            caseStreamAllocation.AllocatedUser.PersonId != request.AllocatedUserId)
        {
          caseStreamAllocation.AllocatedUser =
            new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == request.AllocatedUserId);
        }
        if (caseStreamAllocation.Escalation == null ||
            caseStreamAllocation.Escalation.EscalationId == request.EscalationType.ToInt())
        {
          caseStreamAllocation.Escalation =
            new XPQuery<STR_Escalation>(uow).FirstOrDefault(e => e.EscalationId == request.EscalationType.ToInt());
        }
        caseStreamAllocation.NoActionCount = request.NoActionCount;
        caseStreamAllocation.TransferredIn = request.TransferredIn;
        caseStreamAllocation.TransferredOut = request.TransferredOut;
        caseStreamAllocation.TransferredOutDate = request.TransferredOutDate;
        caseStreamAllocation.SMSCount = request.SmsCount;

        if (request.CompleteDate.HasValue)
        {
          caseStreamAllocation.CompleteComment =
            new XPQuery<STR_Comment>(uow).FirstOrDefault(c => c.CommentId == request.CompleteCommentId);
          caseStreamAllocation.CompleteDate = request.CompleteDate;
        }

        uow.CommitChanges();

        return new CaseStreamAllocation
        {
          CompleteDate = caseStreamAllocation.CompleteDate,
          AllocatedDate = caseStreamAllocation.AllocatedDate,
          CaseStreamAllocationId = caseStreamAllocation.CaseStreamAllocationId,
          TransferredOut = caseStreamAllocation.TransferredOut,
          TransferredOutDate = caseStreamAllocation.TransferredOutDate,
          NoActionCount = caseStreamAllocation.NoActionCount,
          TransferredIn = caseStreamAllocation.TransferredIn,
          SmsCount = caseStreamAllocation.SMSCount,
          AllocatedUser =
            caseStreamAllocation.AllocatedUser == null ? "" : caseStreamAllocation.AllocatedUser.Security.Username,
          EscalationType =
            caseStreamAllocation.Escalation == null
              ? Framework.Enumerators.Stream.EscalationType.None
              : caseStreamAllocation.Escalation.EscalationType,
          AllocatedUserId = caseStreamAllocation.AllocatedUser == null ? 0 : caseStreamAllocation.AllocatedUser.PersonId,
          CompleteComment =
            caseStreamAllocation.CompleteComment == null ? "" : caseStreamAllocation.CompleteComment.Description,
          CaseStreamId = caseStreamAllocation.CaseStream == null ? 0 : caseStreamAllocation.CaseStream.CaseStreamId
        };
      }

    }

    public ICaseStreamEscalation AddOrUpdateCaseStreamEscalation(AddOrUpdateCaseStreamEscalationRequest request)
    {
      using (var uow = new UnitOfWork())
      {
        var caseStreamEscalation =
          new XPQuery<STR_CaseStreamEscalation>(uow).FirstOrDefault(
            d => request.CaseStreamEscalationId == d.CaseStreamEscalationId) ??
          new STR_CaseStreamEscalation(uow)
          {
            CreateDate = DateTime.Now
          };

        if (caseStreamEscalation.CaseStream == null ||
            caseStreamEscalation.CaseStream.CaseStreamId == request.CaseStreamId)
        {
          var caseStream = new XPQuery<STR_CaseStream>(uow).FirstOrDefault(e => e.CaseStreamId == request.CaseStreamId);
          if (caseStream == null)
          {
            throw new Exception(string.Format("Case Stream with Id {0} does not exist", request.CaseStreamId));
          }
          caseStreamEscalation.CaseStream = caseStream;
        }
        if (caseStreamEscalation.Escalation == null ||
            caseStreamEscalation.Escalation.EscalationId == request.EscalationType.ToInt())
        {
          caseStreamEscalation.Escalation =
            new XPQuery<STR_Escalation>(uow).FirstOrDefault(e => e.EscalationId == request.EscalationType.ToInt());
        }

        uow.CommitChanges();

        return new CaseStreamEscalation
        {
          CaseStreamId = caseStreamEscalation.CaseStream.CaseStreamId,
          CaseStreamEscalationId = caseStreamEscalation.CaseStreamEscalationId,
          CreateDate = caseStreamEscalation.CreateDate,
          EscalationType =
            caseStreamEscalation.Escalation == null
              ? Framework.Enumerators.Stream.EscalationType.None
              : caseStreamEscalation.Escalation.EscalationType
        };
      }
    }

    public ICaseStreamAction AddOrUpdateCaseStreamAction(AddOrUpdateCaseStreamActionRequest request)
    {
      using (var uow = new UnitOfWork())
      {
        var caseStreamAction =
          new XPQuery<STR_CaseStreamAction>(uow).FirstOrDefault(
            d => request.CaseStreamActionId == d.CaseStreamActionId) ??
          new STR_CaseStreamAction(uow);

        if (caseStreamAction.CaseStream == null || caseStreamAction.CaseStream.CaseStreamId != request.CaseStreamId)
        {
          caseStreamAction.CaseStream =
            new XPQuery<STR_CaseStream>(uow).FirstOrDefault(e => e.CaseStreamId == request.CaseStreamId);
        }
        if (request.CompleteDate.HasValue)
        {
          caseStreamAction.CompleteDate = request.CompleteDate.Value;
        }
        caseStreamAction.ActionDate = request.ActionDate;
        if (caseStreamAction.ActionType == null ||
            caseStreamAction.ActionType.ActionTypeId != request.ActionType.ToInt())
        {
          caseStreamAction.ActionType =
            new XPQuery<STR_ActionType>(uow).FirstOrDefault(a => a.ActionTypeId == request.ActionType.ToInt());
        }
        caseStreamAction.Amount = request.Amount;
        if (request.DateActioned.HasValue)
        {
          caseStreamAction.DateActioned = request.DateActioned.Value;
        }
        if (request.IsSuccess.HasValue)
        {
          caseStreamAction.IsSuccess = request.IsSuccess.Value;
        }

        uow.CommitChanges();

        return new CaseStreamAction
        {
          CaseStreamId = caseStreamAction.CaseStream.CaseStreamId,
          CompleteDate = caseStreamAction.CompleteDate,
          CaseStreamActionId = caseStreamAction.CaseStreamActionId,
          Amount = caseStreamAction.Amount,
          DateActioned = caseStreamAction.DateActioned,
          ActionDate = caseStreamAction.ActionDate,
          IsSuccess = caseStreamAction.IsSuccess,
          ActionType = caseStreamAction.ActionType.Type,
        };
      }
    }

    public IDebtorContact AddOrUpdateDebtorContact(AddOrUpdateDebtorContactRequest request)
    {
      using (var uow = new UnitOfWork())
      {
        // make sure is not empty
        if (string.IsNullOrWhiteSpace(request.Value))
          return null;
        request.Value = request.Value.Replace(" ", string.Empty);

        // check for duplicates for this debtor
        var existingContact =
          new XPQuery<STR_DebtorContact>(uow).FirstOrDefault(
            c =>
              c.Value == request.Value && c.Debtor.DebtorId == request.DebtorId &&
              c.ContactType.ContactTypeId == request.ContactType.ToInt() && c.IsActive == request.IsActive);
        if (existingContact != null)
        {
          return new DebtorContact
          {
            DebtorContactId = existingContact.DebtorContactId,
            Value = existingContact.Value,
            DebtorId = existingContact.Debtor.DebtorId,
            CreateDate = existingContact.CreateDate,
            CreateUserId = existingContact.CreateUser.PersonId,
            CreateUser = existingContact.CreateUser.Security.Username,
            IsActive = existingContact.IsActive
          };
        }

        var debtorContact =
          new XPQuery<STR_DebtorContact>(uow).FirstOrDefault(
            d => request.DebtorContactId == d.DebtorContactId) ??
          new STR_DebtorContact(uow)
          {

            CreateDate = DateTime.Now,
            CreateUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == request.CreateUserId)
          };

        debtorContact.ContactType =
          new XPQuery<ContactType>(uow).FirstOrDefault(d => d.ContactTypeId == request.ContactType.ToInt());
        debtorContact.Debtor = new XPQuery<STR_Debtor>(uow).FirstOrDefault(d => d.DebtorId == request.DebtorId);
        debtorContact.IsActive = request.IsActive;
        debtorContact.Value = request.Value;

        uow.CommitChanges();

        return new DebtorContact
        {
          DebtorContactId = debtorContact.DebtorContactId,
          Value = debtorContact.Value,
          DebtorId = debtorContact.Debtor.DebtorId,
          CreateDate = debtorContact.CreateDate,
          CreateUserId = debtorContact.CreateUser.PersonId,
          CreateUser = debtorContact.CreateUser.Security.Username,
          IsActive = debtorContact.IsActive
        };
      }
    }

    public IDebtorAddress AddOrUpdateDebtorAddress(AddOrUpdateDebtorAddressRequest request)
    {
      using (var uow = new UnitOfWork())
      {
        // validation 
        if (string.IsNullOrWhiteSpace(request.Line1)
            && string.IsNullOrWhiteSpace(request.Line2)
            && string.IsNullOrWhiteSpace(request.Line3)
            && string.IsNullOrWhiteSpace(request.Line4)
            && string.IsNullOrWhiteSpace(request.PostalCode))
        {
          return null;
        }

        if (!string.IsNullOrWhiteSpace(request.Line1))
        {
          request.Line1 = request.Line1.Replace(" ", string.Empty);
        }
        if (!string.IsNullOrWhiteSpace(request.Line2))
        {
          request.Line2 = request.Line2.Replace(" ", string.Empty);
        }
        if (!string.IsNullOrWhiteSpace(request.Line3))
        {
          request.Line3 = request.Line3.Replace(" ", string.Empty);
        }
        if (!string.IsNullOrWhiteSpace(request.Line4))
        {
          request.Line4 = request.Line4.Replace(" ", string.Empty);
        }
        if (!string.IsNullOrWhiteSpace(request.PostalCode))
        {
          request.PostalCode = request.PostalCode.Replace(" ", string.Empty);
        }

        var existingAddress =
          new XPQuery<STR_DebtorAddress>(uow).FirstOrDefault(
            a => a.Debtor.DebtorId == request.DebtorId && a.AddressType.AddressTypeId == request.AddressType.ToInt() &&
                 a.Line1 == request.Line1 &&
                 a.Line2 == request.Line2 &&
                 a.Line3 == request.Line3 &&
                 a.Line4 == request.Line4 &&
                 a.PostalCode == request.PostalCode && a.IsActive == request.IsActive);
        if (existingAddress != null)
        {
          return new DebtorAddress
          {
            DebtorAddressId = existingAddress.DebtorAddressId,
            Line1 = existingAddress.Line1,
            Line2 = existingAddress.Line2,
            Line3 = existingAddress.Line3,
            Line4 = existingAddress.Line4,
            Province = (General.Province?)(existingAddress.Province == null ? null : (long?)existingAddress.Province.ProvinceId),
            PostalCode = existingAddress.PostalCode,
            DebtorId = existingAddress.Debtor.DebtorId,
            CreateDate = existingAddress.CreateDate,
            CreateUserId = existingAddress.CreateUser == null ? 0 : existingAddress.CreateUser.PersonId,
            CreateUser = existingAddress.CreateUser == null ? "" : existingAddress.CreateUser.Security.Username,
            IsActive = existingAddress.IsActive
          };
        }

        var debtorAddress =
          new XPQuery<STR_DebtorAddress>(uow).FirstOrDefault(
            d => request.DebtorAddressId == d.DebtorAddressId) ??
          new STR_DebtorAddress(uow);

        debtorAddress.AddressType =
          new XPQuery<ADR_Type>(uow).FirstOrDefault(d => d.AddressTypeId == request.AddressType.ToInt());
        if (debtorAddress.DebtorAddressId == 0)
        {
          debtorAddress.CreateDate = DateTime.Now;
          debtorAddress.CreateUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == request.CreateUserId);
        }
        debtorAddress.Debtor = new XPQuery<STR_Debtor>(uow).FirstOrDefault(d => d.DebtorId == request.DebtorId);
        debtorAddress.IsActive = request.IsActive;
        debtorAddress.Line1 = request.Line1;
        debtorAddress.Line2 = request.Line2;
        debtorAddress.Line3 = request.Line3;
        debtorAddress.Line4 = request.Line4;
        debtorAddress.Province =
          new XPQuery<Atlas.Domain.Model.Province>(uow).FirstOrDefault(p => p.ProvinceId == request.Province.ToInt());
        debtorAddress.PostalCode = request.PostalCode;

        uow.CommitChanges();

        return new DebtorAddress
        {
          DebtorAddressId = debtorAddress.DebtorAddressId,
          Line1 = debtorAddress.Line1,
          Line2 = debtorAddress.Line2,
          Line3 = debtorAddress.Line3,
          Line4 = debtorAddress.Line4,
          Province = (General.Province?)(debtorAddress.Province == null ? null : (long?)debtorAddress.Province.ProvinceId),
          PostalCode = debtorAddress.PostalCode,
          DebtorId = debtorAddress.Debtor.DebtorId,
          CreateDate = debtorAddress.CreateDate,
          CreateUserId = debtorAddress.CreateUser == null ? 0 : debtorAddress.CreateUser.PersonId,
          CreateUser = debtorAddress.CreateUser == null ? "" : debtorAddress.CreateUser.Security.Username,
          IsActive = debtorAddress.IsActive
        };
      }
    }

    /// <summary>
    /// Get Debtor Object by Id Number
    /// </summary>
    /// <param name="idNumber"></param>
    /// <returns></returns>
    public IDebtor GetDebtorByIdNumber(string idNumber)
    {
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        uow.TrackPropertiesModifications = false;
        var debtor = new XPQuery<STR_Debtor>(uow).FirstOrDefault(d => d.IdNumber == idNumber);

        if (debtor == null)
          return null;

        var debtorDto = new Debtor()
        {
          Addresses = new List<IAddress>(),
          Contacts = new List<IContact>(),
          DebtorId = debtor.DebtorId,
          IdNumber = debtor.IdNumber,
          Title = debtor.Title,
          FirstName = debtor.FirstName,
          LastName = debtor.LastName
        };

        foreach (var address in debtor.Addresses)
        {
          var addressDto = new Address
          {
            Line1 = address.Line1,
            Line2 = address.Line2,
            Line3 = address.Line3,
            Line4 = address.Line4,
            AddressType = new AddressType(),
            PostalCode = address.PostalCode,
            Province = new Falcon.Common.Structures.Province()
          };
          if (address.AddressType != null)
          {
            addressDto.AddressType.AddressTypeId = address.AddressType.AddressTypeId;
            addressDto.AddressType.Description = address.AddressType.Description;
            addressDto.AddressType.Type = (General.AddressType)address.AddressType.AddressTypeId;
          }
          if (address.Province != null)
            addressDto.Province.ShortCode = address.Province.ShortCode;
          debtorDto.Addresses.Add(addressDto);
        }

        foreach (var contact in debtor.Contacts)
        {
          var contactDto = new Falcon.Common.Structures.Contact
          {
            DebtorContactId = contact.DebtorContactId,
            Value = contact.Value,
            ContactType = new Falcon.Common.Structures.ContactType()
          };
          if (contact.ContactType != null)
          {
            contactDto.ContactType.ContactTypeId = contact.ContactType.ContactTypeId;
            contactDto.ContactType.Description = contact.ContactType.Description;
            contactDto.ContactType.Type = (General.ContactType)contact.ContactType.ContactTypeId;
          }
          debtorDto.Contacts.Add(contactDto);
        }
        return debtorDto;
      }
    }

    /// <summary>
    /// Get Debtor Object by Id
    /// </summary>
    /// <param name="debtorId"></param>
    /// <returns></returns>
    public IDebtor GetDebtorById(long debtorId)
    {
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        uow.TrackPropertiesModifications = false;
        var debtor = new XPQuery<STR_Debtor>(uow).FirstOrDefault(d => d.DebtorId == debtorId);

        if (debtor == null)
          return null;

        var debtorDto = new Debtor()
        {
          Addresses = new List<IAddress>(),
          Contacts = new List<IContact>(),
          DebtorId = debtor.DebtorId,
          IdNumber = debtor.IdNumber,
          Title = debtor.Title,
          FirstName = debtor.FirstName,
          LastName = debtor.LastName
        };

        foreach (var address in debtor.Addresses)
        {
          var addressDto = new Address
          {
            Line1 = address.Line1,
            Line2 = address.Line2,
            Line3 = address.Line3,
            Line4 = address.Line4,
            AddressType = new AddressType(),
            PostalCode = address.PostalCode,
            Province = new Falcon.Common.Structures.Province()
          };
          if (address.AddressType != null)
          {
            addressDto.AddressType.AddressTypeId = address.AddressType.AddressTypeId;
            addressDto.AddressType.Description = address.AddressType.Description;
            addressDto.AddressType.Type = (General.AddressType)address.AddressType.AddressTypeId;
          }
          if (address.Province != null)
            addressDto.Province.ShortCode = address.Province.ShortCode;
          debtorDto.Addresses.Add(addressDto);
        }

        foreach (var contact in debtor.Contacts)
        {
          var contactDto = new Falcon.Common.Structures.Contact
          {
            DebtorContactId = contact.DebtorContactId,
            Value = contact.Value,
            ContactType = new Falcon.Common.Structures.ContactType()
          };
          if (contact.ContactType != null)
          {
            contactDto.ContactType.ContactTypeId = contact.ContactType.ContactTypeId;
            contactDto.ContactType.Description = contact.ContactType.Description;
            contactDto.ContactType.Type = (General.ContactType)contact.ContactType.ContactTypeId;
          }
          debtorDto.Contacts.Add(contactDto);
        }
        return debtorDto;
      }
    }

    public IDebtor GetDebtorByReference(long reference)
    {
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        uow.TrackPropertiesModifications = false;
        var debtor = new XPQuery<STR_Debtor>(uow).FirstOrDefault(d => d.Reference == reference);

        if (debtor == null)
          return null;

        var debtorDto = new Debtor()
        {
          Addresses = new List<IAddress>(),
          Contacts = new List<IContact>(),
          DebtorId = debtor.DebtorId,
          IdNumber = debtor.IdNumber,
          Title = debtor.Title,
          FirstName = debtor.FirstName,
          LastName = debtor.LastName
        };

        foreach (var address in debtor.Addresses)
        {
          var addressDto = new Address
          {
            Line1 = address.Line1,
            Line2 = address.Line2,
            Line3 = address.Line3,
            Line4 = address.Line4,
            AddressType = new AddressType(),
            PostalCode = address.PostalCode,
            Province = new Falcon.Common.Structures.Province()
          };
          if (address.AddressType != null)
          {
            addressDto.AddressType.AddressTypeId = address.AddressType.AddressTypeId;
            addressDto.AddressType.Description = address.AddressType.Description;
            addressDto.AddressType.Type = (General.AddressType)address.AddressType.AddressTypeId;
          }
          if (address.Province != null)
            addressDto.Province.ShortCode = address.Province.ShortCode;
          debtorDto.Addresses.Add(addressDto);
        }

        foreach (var contact in debtor.Contacts)
        {
          var contactDto = new Falcon.Common.Structures.Contact
          {
            DebtorContactId = contact.DebtorContactId,
            Value = contact.Value,
            ContactType = new Falcon.Common.Structures.ContactType()
          };
          if (contact.ContactType != null)
          {
            contactDto.ContactType.ContactTypeId = contact.ContactType.ContactTypeId;
            contactDto.ContactType.Description = contact.ContactType.Description;
            contactDto.ContactType.Type = (General.ContactType)contact.ContactType.ContactTypeId;
          }
          debtorDto.Contacts.Add(contactDto);
        }
        return debtorDto;
      }
    }

    /// <summary>
    /// Completes an Escalated Case Stream
    /// </summary>
    /// <param name="caseStreamId"></param>
    /// <param name="allocatedUserId"></param>
    /// <param name="commentId"></param>
    public void MarkEscalationAsComplete(long caseStreamId, string allocatedUserId, int commentId)
    {
      var person = _userService.Get(allocatedUserId);
      if (person == null)
        throw new Exception(string.Format("UserId {0} does not exist", allocatedUserId));
      using (var uow = new UnitOfWork())
      {
        var caseStreamAllocation =
          new XPQuery<STR_CaseStreamAllocation>(uow).FirstOrDefault(
            c =>
              c.CaseStream.CaseStreamId == caseStreamId && c.AllocatedUser.PersonId == person.PersonId &&
              !c.CompleteDate.HasValue);
        if (caseStreamAllocation == null) return;
        caseStreamAllocation.CompleteDate = DateTime.Now;
        caseStreamAllocation.CompleteComment =
          new XPQuery<STR_Comment>(uow).FirstOrDefault(c => c.CommentId == commentId);
        caseStreamAllocation.CaseStream.Escalation =
          new XPQuery<STR_Escalation>(uow).FirstOrDefault(
            e => e.EscalationType == Framework.Enumerators.Stream.EscalationType.None);

        if (caseStreamAllocation.CompleteComment != null)
          AddAccountNote(userId: allocatedUserId,
            note:
              string.Format("Escalation Completed by {0} with reason: {1}", person.FullName,
                caseStreamAllocation.CompleteComment.Description),
            accountNoteType: Framework.Enumerators.Stream.AccountNoteType.Normal,
            caseId: caseStreamAllocation.CaseStream.Case.CaseId);
        uow.CommitChanges();
      }
    }

    /// <summary>
    /// Completes an Escalated Case Stream
    /// </summary>
    /// <param name="caseStreamId"></param>
    /// <param name="allocatedUserId"></param>
    public void MarkEscalationAsComplete(long caseStreamId, string allocatedUserId)
    {
      var person = _userService.Get(allocatedUserId);
      if (person == null)
        throw new Exception(string.Format("UserId {0} does not exist", allocatedUserId));
      using (var uow = new UnitOfWork())
      {
        var caseStreamAllocation =
          new XPQuery<STR_CaseStreamAllocation>(uow).FirstOrDefault(
            c =>
              c.CaseStream.CaseStreamId == caseStreamId && c.AllocatedUser.PersonId == person.PersonId &&
              !c.CompleteDate.HasValue);
        if (caseStreamAllocation == null) return;
        caseStreamAllocation.CompleteDate = DateTime.Now;
        if (caseStreamAllocation.CaseStream.Case.Group.GroupType == Framework.Enumerators.Stream.GroupType.Collections)
        {
          caseStreamAllocation.CompleteComment =
            new XPQuery<STR_Comment>(uow).FirstOrDefault(
              c =>
                c.CommentGroup.CommentGroupId ==
                Framework.Enumerators.Stream.CommentGroupType.Collections_DeEscalate.ToInt() && c.DisableDate == null);
        }
        else 
        {
          caseStreamAllocation.CompleteComment =
            new XPQuery<STR_Comment>(uow).FirstOrDefault(
              c =>
                c.CommentGroup.CommentGroupId ==
                Framework.Enumerators.Stream.CommentGroupType.Sales_DeEscalate.ToInt() && c.DisableDate == null);
        }

        caseStreamAllocation.CaseStream.Escalation =
          new XPQuery<STR_Escalation>(uow).FirstOrDefault(
            e => e.EscalationType == Framework.Enumerators.Stream.EscalationType.None);

        if (caseStreamAllocation.CompleteComment != null)
          AddAccountNote(userId: allocatedUserId,
            note:
              string.Format("Escalation Completed by {0} with reason: {1}", person.FullName,
                caseStreamAllocation.CompleteComment.Description),
            accountNoteType: Framework.Enumerators.Stream.AccountNoteType.Normal,
            caseId: caseStreamAllocation.CaseStream.Case.CaseId);
        uow.CommitChanges();
      }
    }

    /// <summary>
    /// Add an Actuator that determines when pause escalation
    /// </summary>
    /// <param name="actuatorType"></param>
    /// <param name="branchId"></param>
    /// <param name="regionId"></param>
    /// <param name="rangeStart"></param>
    /// <param name="rangeEnd"></param>
    /// <param name="isActive"></param>
    /// <param name="disableOverlappingActuators"></param>
    public void AddActuator(Framework.Enumerators.Stream.ActuatorType actuatorType, long? branchId, long? regionId,
      DateTime rangeStart, DateTime rangeEnd,
      bool isActive = true, bool disableOverlappingActuators = false)
    {
      using (var uow = new UnitOfWork())
      {
        var existingActuatorQuery =
          new XPQuery<STR_Actuator>(uow).Where(
            a => ((rangeStart.Date >= a.RangeStart.Date && rangeStart.Date <= a.RangeEnd.Date)
                  || (rangeEnd.Date >= a.RangeStart.Date && rangeEnd.Date <= a.RangeEnd.Date))
                 && a.ActuatorType.Type == actuatorType);

        if (branchId.HasValue)
          existingActuatorQuery = existingActuatorQuery.Where(a => a.Branch.BranchId == branchId);
        else if (regionId.HasValue)
          existingActuatorQuery = existingActuatorQuery.Where(a => a.Region.RegionId == regionId);

        var overlappingActuator = existingActuatorQuery.ToList();
        if (overlappingActuator.Count > 0)
        {
          if (disableOverlappingActuators)
            overlappingActuator.ForEach(a =>
            {
              a.IsActive = false;
              a.DisableDate = DateTime.Now;
            });
          else
            throw new Exception("New Actuator overlaps with another actuator");
        }

        var newActuator = new STR_Actuator(uow)
        {
          ActuatorType = new XPQuery<STR_ActuatorType>(uow).FirstOrDefault(a => a.Type == actuatorType),
          CreateDate = DateTime.Now,
          IsActive = isActive,
          RangeEnd = rangeEnd,
          RangeStart = rangeStart
        };
        if (branchId.HasValue)
        {
          newActuator.Branch = new XPQuery<BRN_Branch>(uow).FirstOrDefault(b => b.BranchId == branchId);
          if (newActuator.Branch != null) newActuator.Region = newActuator.Branch.Region;
        }
        else if (regionId.HasValue)
        {
          newActuator.Region = new XPQuery<Atlas.Domain.Model.Region>(uow).FirstOrDefault(r => r.RegionId == regionId);
        }

        uow.CommitChanges();
      }
    }

    public ICaseStreamAction GetNextAction(long caseStreamId, Action.Type actionType)
    {
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        uow.TrackPropertiesModifications = false;
        var caseStreamAction =
          new XPQuery<STR_CaseStreamAction>(uow).Where(
            c =>
              c.CaseStream.CaseStreamId == caseStreamId &&
              c.ActionType.Type == actionType)
            .OrderBy(c => c.ActionDate)
            .FirstOrDefault();

        if (caseStreamAction == null)
          return null;

        return new CaseStreamAction
        {
          CompleteDate = caseStreamAction.CompleteDate,
          CaseStreamId = caseStreamAction.CaseStream.CaseStreamId,
          CaseStreamActionId = caseStreamAction.CaseStreamActionId,
          Amount = caseStreamAction.Amount,
          IsSuccess = caseStreamAction.IsSuccess,
          ActionType = caseStreamAction.ActionType.Type,
          DateActioned = caseStreamAction.DateActioned,
          ActionDate = caseStreamAction.ActionDate
        };
      }
    }

    public ITransaction AddOrUpdateAccountTransaction(AddOrUpdateAccountTransactionRequest request)
    {
      using (var uow = new UnitOfWork())
      {
        // get the transaction by id,
        // if the transaction does not exist, try getting it by the account id that the transaction is supposed to be linked to and the reference of the transaction 
        // this could happen when then was an error importing the data, and the job failed
        // otherwise create a new transaction object
        var transaction =
          new XPQuery<STR_Transaction>(uow).FirstOrDefault(t => t.TransactionId == request.TransactionId) ??
          new XPQuery<STR_Transaction>(uow).FirstOrDefault(
            t => t.Account.AccountId == request.AccountId && t.Reference == request.Reference)
          ?? new STR_Transaction(uow)
          {
            CreateDate = DateTime.Now
          };

        if (transaction.Account == null)
        {
          transaction.Account = new XPQuery<STR_Account>(uow).FirstOrDefault(a => a.AccountId == request.AccountId);
        }
        transaction.Amount = request.Amount;
        transaction.InstalmentNumber = request.InstalmentNumber;
        transaction.Reference = request.Reference;
        transaction.TransactionDate = request.TransactionDate;
        if (request.TransactionStatus.HasValue)
        {
          if (transaction.TransactionStatus == null || (transaction.TransactionStatus != null &&
                                                        transaction.TransactionStatus.TransactionStatusId !=
                                                        request.TransactionStatus.Value.ToInt()))
          {
            transaction.TransactionStatus =
              new XPQuery<STR_TransactionStatus>(uow).FirstOrDefault(
                s => s.TransactionStatusId == request.TransactionStatus.ToInt());
          }
        }

        if (transaction.TransactionType == null || (transaction.TransactionType != null &&
                                                    transaction.TransactionType.TransactionTypeId !=
                                                    request.TransactionType.ToInt()))
        {
          transaction.TransactionType =
            new XPQuery<STR_TransactionType>(uow).FirstOrDefault(
              s => s.TransactionTypeId == request.TransactionType.ToInt());
        }

        uow.CommitChanges();

        return new Transaction
        {
          CreateDate = transaction.CreateDate,
          Reference = transaction.Reference,
          TransactionStatus =
            transaction.TransactionStatus == null
              ? (Framework.Enumerators.Stream.TransactionStatus?)null
              : transaction.TransactionStatus.Status,
          Amount = transaction.Amount,
          AccountId = transaction.Account.AccountId,
          TransactionId = transaction.TransactionId,
          InstalmentNumber = transaction.InstalmentNumber,
          TransactionDate = transaction.TransactionDate,
          TransactionType = transaction.TransactionType.Type
        };
      }
    }

    public ICollection<ITransaction> GetAccountTransactions(long accountId)
    {
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        uow.TrackPropertiesModifications = false;
        var transactions = new XPQuery<STR_Transaction>(uow).Where(t => t.Account.AccountId == accountId).ToList();

        return transactions.Select(transaction => (ITransaction)new Transaction
        {
          CreateDate = transaction.CreateDate,
          Reference = transaction.Reference,
          TransactionStatus =
            transaction.TransactionStatus == null
              ? (Framework.Enumerators.Stream.TransactionStatus?)null
              : transaction.TransactionStatus.Status,
          Amount = transaction.Amount,
          AccountId = transaction.Account.AccountId,
          AccountReference = transaction.Account.Reference,
          TransactionId = transaction.TransactionId,
          InstalmentNumber = transaction.InstalmentNumber,
          TransactionDate = transaction.TransactionDate,
          TransactionType = transaction.TransactionType.Type
        }).ToList();
      }
    }

    public ICollection<ITransaction> GetCaseTransactions(long caseId)
    {
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        uow.TrackPropertiesModifications = false;
        var accounts =
          new XPQuery<STR_Case>(uow).Where(c => c.CaseId == caseId).ToList().SelectMany(c => c.Debtor.Accounts).ToList();
        var transactions = new XPQuery<STR_Transaction>(uow).Where(t => accounts.Contains(t.Account)).ToList();

        return transactions.Select(transaction => (ITransaction)new Transaction
        {
          CreateDate = transaction.CreateDate,
          Reference = transaction.Reference,
          TransactionStatus =
            transaction.TransactionStatus == null
              ? (Framework.Enumerators.Stream.TransactionStatus?)null
              : transaction.TransactionStatus.Status,
          Amount = transaction.Amount,
          AccountId = transaction.Account.AccountId,
          AccountReference = transaction.Account.Reference,
          TransactionId = transaction.TransactionId,
          InstalmentNumber = transaction.InstalmentNumber,
          TransactionDate = transaction.TransactionDate,
          TransactionType = transaction.TransactionType.Type
        }).ToList();
      }
    }

    public ICollection<ITransaction> GetCaseAccountTransactions(long accountId, long caseId)
    {
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        uow.TrackPropertiesModifications = false;
        var accounts =
          new XPQuery<STR_Case>(uow).Where(
            c => c.CaseId == caseId && c.Debtor.Accounts.Any(a => a.AccountId == accountId))
            .ToList()
            .SelectMany(c => c.Debtor.Accounts)
            .ToList();
        var transactions = new XPQuery<STR_Transaction>(uow).Where(t => accounts.Contains(t.Account)).ToList();

        return transactions.Select(transaction => (ITransaction)new Transaction
        {
          CreateDate = transaction.CreateDate,
          Reference = transaction.Reference,
          TransactionStatus =
            transaction.TransactionStatus == null
              ? (Framework.Enumerators.Stream.TransactionStatus?)null
              : transaction.TransactionStatus.Status,
          Amount = transaction.Amount,
          AccountId = transaction.Account.AccountId,
          AccountReference = transaction.Account.Reference,
          TransactionId = transaction.TransactionId,
          InstalmentNumber = transaction.InstalmentNumber,
          TransactionDate = transaction.TransactionDate,
          TransactionType = transaction.TransactionType.Type
        }).ToList();
      }
    }

    public void UpdateAccountWithLastImportReference(long accountId, string lastImportReference)
    {
      using (var uow = new UnitOfWork())
      {
        var account = new XPQuery<STR_Account>(uow).FirstOrDefault(d => accountId == d.AccountId);
        if (account != null)
        {
          account.LastImportReference = lastImportReference;
          uow.CommitChanges();
        }
      }
    }

    public void EscalateUnworkedCases()
    {
      using (var uow = new UnitOfWork())
      {
        var actuators = FlattenActuatorsPerBranch(Framework.Enumerators.Stream.ActuatorType.Escalation);
        var publicHolidays =
          new XPQuery<Atlas.Domain.Model.PublicHoliday>(uow).Where(d => d.Date >= DateTime.Today.AddDays(-10).Date)
            .Select(d => d.Date)
            .ToList();

        var branchIds = new XPQuery<BRN_Branch>(uow).Select(b => b.BranchId).ToList();

        foreach (var branchId in branchIds)
        {
          // Get 3 business days ago, incl today
          var get3WorkingDays = 0;
          var threeWorkingDaysAgo = DateTime.Today;
          while (get3WorkingDays < 5)
          {
            if (threeWorkingDaysAgo.DayOfWeek == DayOfWeek.Sunday || publicHolidays.Contains(threeWorkingDaysAgo))
            {
              // not working day
            }
            else
            {
              // working day
              // is day bypassed?
              var branchActuatorClosed =
                actuators.FirstOrDefault(
                  b =>
                    b.Branch.BranchId == branchId && threeWorkingDaysAgo.Date >= b.RangeStart.Date &&
                    threeWorkingDaysAgo.Date <= b.RangeEnd.Date);
              if (branchActuatorClosed == null)
                get3WorkingDays++;
            }
            threeWorkingDaysAgo = threeWorkingDaysAgo.AddDays(-1);
          }

          var casesStatusIds = new[] { CaseStatus.Type.New.ToInt(), CaseStatus.Type.InProgress.ToInt(), CaseStatus.Type.OnHold.ToInt() };

          var caseStreamActionsUnworked =
            new XPQuery<STR_CaseStreamAction>(uow).Where(
              c => c.ActionDate.Date <= threeWorkingDaysAgo.Date && !c.CompleteDate.HasValue
                   && c.CaseStream.Case.Branch.BranchId == branchId
                   && casesStatusIds.Contains(c.CaseStream.Case.CaseStatus.CaseStatusId)
                   && c.CaseStream.Escalation.EscalationType != Framework.Enumerators.Stream.EscalationType.Director
                   && c.CaseStream.CaseStreamEscalations.Count(e => e.CreateDate.Date == DateTime.Today) == 0).ToList();


          Parallel.ForEach(caseStreamActionsUnworked, new ParallelOptions
          {
            MaxDegreeOfParallelism = 15
          }, caseStreamAction =>
          {
            try
            {
              EscalateCaseStream(caseStreamId: caseStreamAction.CaseStream.CaseStreamId,
                personId: (int)General.Person.System,
                commentId: (int)Framework.Enumerators.Stream.Comment.EscalatedBySystem);
            }
            catch (Exception exception)
            {
              _logger.Error(string.Format("Error escalating case stream: {0} - {1}", exception.Message,
                exception.StackTrace));
            }
          });
        }

        uow.CommitChanges();
      }
    }

    public List<IAccountStreamAction> GetAccountStreamActions(params long[] caseIds)
    {
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        uow.TrackPropertiesModifications = false;
        // get caseIds from ungrouped query
        const int actionTypeId = (int)Action.Type.Action;
        var cases = new XPQuery<STR_CaseStreamAction>(uow).Where(c => c.ActionType.ActionTypeId != actionTypeId &&
                                                                      caseIds.Contains(c.CaseStream.Case.CaseId))
          .OrderBy(c => c.CaseStream.Case.CaseId)
          .ThenByDescending(c => c.CaseStreamActionId)
          .ToList();

        // get latest case stream action per case
        var caseStreamActions = (from c in cases
                                 orderby c.CaseStream.Case.CaseId, c.CaseStreamActionId descending
                                 select new
                                 {
                                   c,
                                   count = (from r in cases
                                            where c.CaseStream.Case.CaseId == r.CaseStream.Case.CaseId
                                                  && c.CaseStreamActionId < r.CaseStreamActionId
                                            select r).Count()
                                 }).Where(c => c.count == 0)
          .Select(c => c.c)
          .OrderByDescending(p => p.ActionDate)
          .ThenBy(p => p.CaseStream.Priority.Value)
          .ToList();

        var distinctCaseStreamActions = new List<STR_CaseStreamAction>();
        caseStreamActions.ForEach(caseStreamAction =>
        {
          if (
            distinctCaseStreamActions.Count(c => c.CaseStream.CaseStreamId == caseStreamAction.CaseStream.CaseStreamId) ==
            0)
            distinctCaseStreamActions.Add(caseStreamAction);
        });

        var caseStreamActionsDto = new List<IAccountStreamAction>();
        distinctCaseStreamActions.ForEach(caseStreamAction =>
        {
          var accountStreamActionDto = new AccountStreamAction
          {
            CaseStreamActionId = caseStreamAction.CaseStreamActionId,
            AccountReference = caseStreamAction.CaseStream.Case.Reference
          };
          var allocatedUser =
            caseStreamAction.CaseStream.CaseStreamAllocations.Where(a => !a.TransferredOut)
              .OrderBy(a => a.Escalation.EscalationId)
              .ThenBy(a => a.AllocatedDate)
              .FirstOrDefault();
          if (allocatedUser != null)
          {
            accountStreamActionDto.AllocatedUserId = allocatedUser.AllocatedUser.PersonId;
            accountStreamActionDto.AllocatedUserFullName = string.Format("{0} {1}",
              allocatedUser.AllocatedUser.Firstname, allocatedUser.AllocatedUser.Lastname);
            accountStreamActionDto.NoActionCount = allocatedUser.NoActionCount;
          }
          accountStreamActionDto.DebtorFullName = string.Format("{0} {1}",
            caseStreamAction.CaseStream.Case.Debtor.FirstName,
            caseStreamAction.CaseStream.Case.Debtor.LastName);
          accountStreamActionDto.CaseId = caseStreamAction.CaseStream.Case.CaseId;
          accountStreamActionDto.CaseStreamId = caseStreamAction.CaseStream.CaseStreamId;
          accountStreamActionDto.CaseStatus = caseStreamAction.CaseStream.Case.CaseStatus.Description;
          accountStreamActionDto.DebtorIdNumber = caseStreamAction.CaseStream.Case.Debtor.IdNumber;
          accountStreamActionDto.Priority = caseStreamAction.CaseStream.Priority.Description;
          accountStreamActionDto.StreamId = caseStreamAction.CaseStream.Stream.StreamId;
          accountStreamActionDto.Category = caseStreamAction.CaseStream.Case.SubCategory.Category.Description;
          accountStreamActionDto.SubCategory = caseStreamAction.CaseStream.Case.SubCategory.Description;
          accountStreamActionDto.ActionDate = caseStreamAction.ActionDate;
          accountStreamActionDto.ActionTypeId = caseStreamAction.ActionType.ActionTypeId;

          accountStreamActionDto.LoanAmount = caseStreamAction.CaseStream.Case.TotalLoanAmount;
          accountStreamActionDto.Balance = caseStreamAction.CaseStream.Case.TotalBalance;
          accountStreamActionDto.ArrearsAmount = caseStreamAction.CaseStream.Case.TotalArrearsAmount;

          caseStreamActionsDto.Add(accountStreamActionDto);
        });

        return caseStreamActionsDto;
      }
    }

    public byte[] GetFinalLetterOfDemandPdf(long caseStreamId)
    {
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        uow.TrackPropertiesModifications = false;
        var caseStream = new XPQuery<STR_CaseStream>(uow).FirstOrDefault(c => c.CaseStreamId == caseStreamId);
        if (caseStream == null)
          throw new Exception(string.Format("Case Stream with Id {0} does not exist", caseStreamId));

        var finalLetterOfDemandTemplate =
          new XPQuery<NTF_Template>(uow).FirstOrDefault(
            n => n.TemplateType.Type == Atlas.Enumerators.Notification.NotificationTemplate.Stream_Letter_FinalDemand);
        if (finalLetterOfDemandTemplate == null)
          return null;

        //var mste = new System.IO.StreamReader(@"C:\Users\Lee\Documents\FDL.mht");
        //finalLetterOfDemandTemplate.Template = mste.ReadToEnd();
        //uow.CommitChanges();
        var keys = new Dictionary<string, string>
        {
          {"[CURRENT_DATE]", DateTime.Now.ToString("MMMM yyyy")},
          {
            "[FULL_NAME]",
            string.Format("{0} {1} {2}", caseStream.Case.Debtor.Title, caseStream.Case.Debtor.FirstName,
              caseStream.Case.Debtor.LastName)
          },
          {"[ID_NUMBER]", caseStream.Case.Debtor.IdNumber},
          {"[EMPLOYEE_NUMBER]", caseStream.Case.Debtor.EmployerCode},
          {"[CLIENT_REFERENCE]", caseStream.Case.Debtor.ThirdPartyReferenceNo}
        };
        var residentialAddress =
          caseStream.Case.Debtor.Addresses.FirstOrDefault(
            a => a.AddressType.AddressTypeId == General.AddressType.Residential.ToInt() && a.IsActive);
        if (residentialAddress != null)
        {
          keys.Add("[ADDRESS_LINE_1]", residentialAddress.Line1);
          keys.Add("[ADDRESS_LINE_2]", residentialAddress.Line2);
          keys.Add("[ADDRESS_LINE_3]", residentialAddress.Line3);
          keys.Add("[ADDRESS_LINE_POSTAL_CODE]", residentialAddress.PostalCode);
        }
        //keys.Add("[LOAN_REFERENCE]", caseStream.Case.Reference);
        //keys.Add("[LOAN_DATE]", caseStream.Case.LoanDate.ToString("dd/MM/yyyy"));
        //keys.Add("[BALANCE]", caseStream.Case.Balance.ToString("#.##"));

        var template = FillInTheBlanks(finalLetterOfDemandTemplate.Template, keys);

        return _pdfService.GetPdfForMhtml(template);
      }
    }

    public void IncBudget(Framework.Enumerators.Stream.BudgetType budgetType, DateTime? enquiryDate = null)
    {
      using (var uow = new UnitOfWork())
      {
        if (!enquiryDate.HasValue)
          enquiryDate = DateTime.Today;
        var budget = new XPQuery<STR_Budget>(uow).FirstOrDefault(b => b.BudgetType == budgetType
                                                                      && b.IsActive
                                                                      && b.RangeStart.Date <= enquiryDate.Value.Date
                                                                      && b.RangeEnd.Date >= enquiryDate.Value.Date);

        if (budget == null)
          throw new Exception(string.Format("Budget with type {0} does not exist in DB", budgetType.ToStringEnum()));

        budget.CurrentValue++;

        uow.CommitChanges();
      }
    }

    public void DeActivateBudget(int budgetId)
    {
      using (var uow = new UnitOfWork())
      {
        var budget = new XPQuery<STR_Budget>(uow).FirstOrDefault(b => b.BudgetId == budgetId);
        if (budget != null && budget.IsActive)
          budget.IsActive = false;
        else
          throw new Exception(string.Format("Budget with Id {0} is already de-activated", budgetId));

        uow.CommitChanges();
      }
    }

    // refactor - srinivas
    public IBudget CreateBudget(Framework.Enumerators.Stream.BudgetType budgetType, DateTime rangeStart,
      DateTime rangeEnd,
      long maxValue, bool isActive = true, bool deActivateCurrent = false, long currentValue = 0)
    {
      using (var uow = new UnitOfWork())
      {
        var budget = new XPQuery<STR_Budget>(uow).Where(b => b.BudgetType == budgetType
                                                             && b.IsActive
                                                             && (
                                                               (b.RangeStart.Date <= rangeStart.Date &&
                                                                b.RangeEnd.Date >= rangeStart.Date)
                                                               ||
                                                               (b.RangeStart.Date <= rangeEnd.Date &&
                                                                b.RangeEnd.Date >= rangeEnd.Date))
          ).ToList();

        if (budget.Count > 1)
          throw new Exception(string.Format("Budget type {0} has more than 1 over-laping date ranges",
            budgetType.ToStringEnum()));

        if (deActivateCurrent)
        {
          if (budget.Count == 1)
          {
            var strBudget = budget.FirstOrDefault();
            if (strBudget != null) strBudget.IsActive = false;
          }
        }
        else
        {
          if (budget.Count > 0)
          {
            var strBudget = budget.FirstOrDefault();
            if (strBudget != null)
              throw new Exception(string.Format("Budget with Id {0} has over-laping dates with new budget",
                strBudget.BudgetId));
          }
        }

        var newBudget = new STR_Budget(uow)
        {
          Description = budgetType.ToStringEnum(),
          RangeStart = rangeStart,
          RangeEnd = rangeEnd,
          MaxValue = maxValue,
          IsActive = isActive,
          CurrentValue = currentValue,
          CreateDate = DateTime.Now
        };

        uow.CommitChanges();

        return new Budget
        {
          BudgetId = newBudget.BudgetId,
          BudgetType = newBudget.BudgetType,
          CreateDate = newBudget.CreateDate,
          CurrentValue = newBudget.CurrentValue,
          Description = newBudget.Description,
          IsActive = newBudget.IsActive,
          MaxValue = newBudget.MaxValue,
          RangeEnd = newBudget.RangeEnd,
          RangeStart = newBudget.RangeStart
        };
      }
    }

    public bool DoesBudgetAllow(Framework.Enumerators.Stream.BudgetType budgetType, DateTime? enquiryDate = null,
      long enquiryValue = 0)
    {
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        uow.TrackPropertiesModifications = false;
        if (!enquiryDate.HasValue)
          enquiryDate = DateTime.Today;
        var budget = new XPQuery<STR_Budget>(uow).FirstOrDefault(b => b.BudgetType == budgetType
                                                                      && b.IsActive
                                                                      && b.RangeStart.Date <= enquiryDate.Value.Date
                                                                      && b.RangeEnd.Date >= enquiryDate.Value.Date);

        if (budget == null)
          throw new Exception(string.Format("Budget with type {0} does not exist in DB", budgetType.ToStringEnum()));

        return budget.MaxValue > ((enquiryValue > 0) ? enquiryValue : budget.CurrentValue);
      }
    }

    public List<IComment> GetCommentsByStreamGroup(Framework.Enumerators.Stream.GroupType groupType)
    {
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        uow.TrackPropertiesModifications = false;
        var groupId = (int)groupType;
        var comments =
          new XPQuery<STR_Comment>(uow).Where(c => c.CommentGroup.Group.GroupId == groupId && !c.DisableDate.HasValue)
            .Select(a => a)
            .OrderBy(a => a.CommentGroup.CommentGroupId)
            .ToList();

        var commentsDto = new List<IComment>();

        commentsDto.AddRange(comments.Select(c => new Comment
        {
          Description = c.Description,
          CommentGroup = new CommentGroup
          {
            Description = c.CommentGroup.Description,
            CommentGroupId = c.CommentGroup.CommentGroupId,
            CommentGroupType = c.CommentGroup.CommentGroupType,
            Group = new Group
            {
              Description = c.CommentGroup.Group.Description,
              GroupType = c.CommentGroup.Group.GroupType,
              GroupId = c.CommentGroup.Group.GroupId
            }
          },
          CommentId = c.CommentId,
          DisableDate = c.DisableDate
        }).ToList());

        return commentsDto;
      }
    }

    public List<IAccountStreamAction> GetWorkItems(Framework.Enumerators.Stream.GroupType[] groupTypes,
      CaseStatus.Type?[] caseStatuses = null,
      long? branchId = null, long? allocatedPersonId = null, string allocatedUserId = null, TimeSpan? bufferTime = null,
      DateTime? completeDate = null,
      long caseId = 0, string idNumber = null, string accountReference = null, DateTime? actionDateStartRange = null,
      DateTime? actionDateEndRange = null,
      int limitResults = 10, DateTime? createDateStartRange = null, DateTime? createDateEndRange = null, params Framework.Enumerators.Stream.StreamType[] streamTypes)
    {
      //if (!bufferTime.HasValue)
      //  bufferTime = new TimeSpan(0, 15, 0);
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        uow.TrackPropertiesModifications = false;
        var groupIds = groupTypes.Select(g => (int)g).ToArray();
        var actionTypeId = (int)Action.Type.Action;
        var caseQuery =
          new XPQuery<STR_CaseStreamAction>(uow).Where(p => groupIds.Contains(p.CaseStream.Case.Group.GroupId)
                                                            && p.ActionType.ActionTypeId != actionTypeId).AsQueryable();

        if (caseId > 0)
          caseQuery = caseQuery.Where(c => c.CaseStream.Case.CaseId == caseId).AsQueryable();

        if (createDateStartRange.HasValue)
        {
          caseQuery = caseQuery.Where(p => p.CaseStream.CreateDate.Date >= createDateStartRange.Value.Date).AsQueryable();
        }
        if (createDateEndRange.HasValue)
        {
          caseQuery = caseQuery.Where(p => p.CaseStream.CreateDate.Date >= createDateEndRange.Value.Date).AsQueryable();
        }

        if (actionDateStartRange.HasValue && actionDateEndRange.HasValue)
          caseQuery =
            caseQuery.Where(
              p =>
                p.ActionDate.Date >= actionDateStartRange.Value.Date &&
                p.ActionDate.Date <= actionDateEndRange.Value.Date).AsQueryable();
        //else
        //  caseQuery = caseQuery.Where(p => p.ActionDate <= DateTime.Now.Add(bufferTime.Value));
        if (caseStatuses != null && caseStatuses.Length > 0)
        {
          var caseStatusIds = caseStatuses.Where(c => c != null).Select(c => c.ToInt());
          caseQuery =
            caseQuery.Where(q => caseStatusIds.Contains(q.CaseStream.Case.CaseStatus.CaseStatusId)).AsQueryable();
        }
        else
        {
          var caseStatusIds = new[]
          {
            CaseStatus.Type.New.ToInt(),
            CaseStatus.Type.InProgress.ToInt(),
            CaseStatus.Type.OnHold.ToInt()
          };
          caseQuery =
            caseQuery.Where(q => caseStatusIds.Contains(q.CaseStream.Case.CaseStatus.CaseStatusId)).AsQueryable();
        }

        if (streamTypes.Length > 0 && streamTypes.Any(s => s != 0))
        {
          var streamIds = streamTypes.Select(s => (int)s).ToList();
          caseQuery =
            caseQuery.Where(
              p =>
                streamIds.Contains(p.CaseStream.Stream.StreamId) &&
                (p.CompleteDate == null ||
                 streamTypes.FirstOrDefault().ToInt() == Framework.Enumerators.Stream.StreamType.Completed.ToInt()))
              .AsQueryable();
        }
        if (branchId.HasValue)
        {
          caseQuery = caseQuery.Where(a => a.CaseStream.Case.Branch.BranchId == branchId).AsQueryable();
        }

        if (allocatedPersonId.HasValue)
        {
          var systemPersonId = (long)General.Person.System;
          caseQuery =
            caseQuery.Where(
              p =>
                !p.CaseStream.CompleteDate.HasValue &&
                p.CaseStream.CaseStreamAllocations.Any(
                  a =>
                    (a.AllocatedUser.PersonId == allocatedPersonId || a.AllocatedUser.PersonId == systemPersonId) &&
                    !a.TransferredOut)).AsQueryable();
          // allocations only for that case stream escalation 
        }
        else if (!string.IsNullOrWhiteSpace(allocatedUserId))
        {
          var person = _userService.Get(allocatedUserId);
          if (person == null)
            throw new Exception(string.Format("UserId {0} does not exist", allocatedUserId));
          caseQuery =
            caseQuery.Where(
              p =>
                !p.CaseStream.CompleteDate.HasValue &&
                p.CaseStream.CaseStreamAllocations.Any(
                  a => a.AllocatedUser.PersonId == person.PersonId && !a.TransferredOut
                       && a.Escalation.EscalationId == p.CaseStream.Escalation.EscalationId)).AsQueryable();
          // allocations only for that case stream escalation
        }
        if (completeDate.HasValue)
          caseQuery = caseQuery.Where(p => p.CompleteDate == completeDate).AsQueryable();

        if (!string.IsNullOrEmpty(idNumber))
          caseQuery = caseQuery.Where(c => c.CaseStream.Case.Debtor.IdNumber == idNumber).AsQueryable();

        if (!string.IsNullOrEmpty(accountReference))
        {
          caseQuery =
            caseQuery.Where(
              c =>
                c.CaseStream.Case.Reference.Contains(accountReference) ||
                c.CaseStream.Case.Debtor.Accounts.Any(a => a.Reference.Contains(accountReference))).AsQueryable();
        }

        var caseStreamActions =
          caseQuery.OrderBy(p => p.ActionDate)
            .ThenByDescending(p => p.CaseStream.Case.SubCategory.SubCategoryId)
            .ThenBy(p => p.CaseStream.Priority.Value)
            .Take(limitResults).AsQueryable()
            .ToList();
        var distinctCaseStreamActions = new List<STR_CaseStreamAction>();
        foreach (
          var caseStreamAction in
            caseStreamActions.Where(
              caseStreamAction =>
                distinctCaseStreamActions.Count(c => c.CaseStream.Case.CaseId == caseStreamAction.CaseStream.Case.CaseId) ==
                0))
        {
          distinctCaseStreamActions.Add(caseStreamAction);
        }

        var caseStreamActionsDto = new List<IAccountStreamAction>();
        distinctCaseStreamActions.ForEach(caseStreamAction =>
        {
          var accountStreamActionDto = new AccountStreamAction();
          accountStreamActionDto.CaseStreamActionId = caseStreamAction.CaseStreamActionId;
          accountStreamActionDto.AccountReference = caseStreamAction.CaseStream.Case.Reference;
          var allocatedUser =
            caseStreamAction.CaseStream.CaseStreamAllocations.Where(a => !a.TransferredOut)
              .OrderBy(a => a.Escalation.EscalationId)
              .ThenBy(a => a.AllocatedDate)
              .FirstOrDefault();
          if (allocatedUser != null)
          {
            accountStreamActionDto.AllocatedUserId = allocatedUser.AllocatedUser.PersonId;
            accountStreamActionDto.AllocatedUserFullName = string.Format("{0} {1}",
              allocatedUser.AllocatedUser.Firstname, allocatedUser.AllocatedUser.Lastname);
            accountStreamActionDto.NoActionCount = allocatedUser.NoActionCount;
          }
          accountStreamActionDto.DebtorFullName = string.Format("{0} {1}",
            caseStreamAction.CaseStream.Case.Debtor.FirstName,
            caseStreamAction.CaseStream.Case.Debtor.LastName);
          accountStreamActionDto.CaseId = caseStreamAction.CaseStream.Case.CaseId;
          accountStreamActionDto.CaseStreamId = caseStreamAction.CaseStream.CaseStreamId;
          accountStreamActionDto.CaseStatus = caseStreamAction.CaseStream.Case.CaseStatus.Description;
          accountStreamActionDto.DebtorIdNumber = caseStreamAction.CaseStream.Case.Debtor.IdNumber;
          accountStreamActionDto.Priority = caseStreamAction.CaseStream.Priority.Description;
          accountStreamActionDto.StreamId = caseStreamAction.CaseStream.Stream.StreamId;
          accountStreamActionDto.Category = caseStreamAction.CaseStream.Case.SubCategory.Category.Description;
          accountStreamActionDto.SubCategory = caseStreamAction.CaseStream.Case.SubCategory.Description;
          accountStreamActionDto.ActionDate = caseStreamAction.ActionDate;
          accountStreamActionDto.ActionTypeId = caseStreamAction.ActionType.ActionTypeId;

          accountStreamActionDto.LoanAmount = caseStreamAction.CaseStream.Case.TotalLoanAmount;
          accountStreamActionDto.Balance = caseStreamAction.CaseStream.Case.TotalBalance;
          accountStreamActionDto.ArrearsAmount = caseStreamAction.CaseStream.Case.TotalArrearsAmount;

          caseStreamActionsDto.Add(accountStreamActionDto);
        });

        return caseStreamActionsDto;
      }
    }

    //TODO: make relevent response model
    public List<IEscalatedCaseStream> GetEscalatedWorkItemsOnly(string allocatedUserId, long branchId,
      int limitResults = 100)
    {
      var person = _userService.Get(allocatedUserId);
      if (person == null)
        throw new Exception(string.Format("UserId {0} does not exist", allocatedUserId));

      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        uow.TrackPropertiesModifications = false;
        var noneEscalationId = Framework.Enumerators.Stream.EscalationType.None.ToInt();
        var activeCaseStatusIds = new[]
        {
          CaseStatus.Type.New.ToInt(),
          CaseStatus.Type.InProgress.ToInt(),
          CaseStatus.Type.OnHold.ToInt()
        };
        var csa = new XPQuery<STR_CaseStreamAllocation>(uow).Where(a => a.AllocatedUser.PersonId == person.PersonId
                                                                        && !a.CaseStream.CompleteDate.HasValue
                                                                        && !a.CompleteDate.HasValue
                                                                        && !a.TransferredOut
                                                                        &&
                                                                        a.Escalation.EscalationId ==
                                                                        a.CaseStream.Escalation.EscalationId
                                                                        && a.Escalation.EscalationId != noneEscalationId
                                                                        &&
                                                                        activeCaseStatusIds.Contains(
                                                                          a.CaseStream.Case.CaseStatus.CaseStatusId))
          .Select(a => a.CaseStream)
          .ToList();

        var caseStreamActions =
          new XPQuery<STR_CaseStreamAction>(uow).Where(p => csa.Contains(p.CaseStream) && !p.CompleteDate.HasValue).
            OrderByDescending(p => p.ActionDate).ThenBy(p => p.CaseStream.Priority.Value).Take(limitResults).ToList();

        var caseStreamActionsDto = caseStreamActions.Select(c => new EscalatedCaseStream
        {
          Amount = c.Amount,
          CaseId = c.CaseStream.Case.CaseId,
          AccountReference = c.CaseStream.Case.Reference,
          CaseStreamId = c.CaseStream.CaseStreamId,
          DebtorFullName = string.Format("{0} {1}", c.CaseStream.Case.Debtor.FirstName, c.CaseStream.Case.Debtor.LastName),
          DebtorIdNumber = c.CaseStream.Case.Debtor.IdNumber,
          Priority = c.CaseStream.Priority.Description,
          CaseStatus = c.CaseStream.Case.CaseStatus.Description,
          CaseStreamActionId = c.CaseStreamActionId,
          ActionDate = c.ActionDate,
          ActionType = c.ActionType.Type,
          CompleteDate = c.CompleteDate,
          DateActioned = c.DateActioned,
          IsSuccess = c.IsSuccess,
          CaseStreamAllocations = new List<ICaseStreamAllocation>()
        }).ToList();

        // get and set original users
        var caseStreamIds = caseStreamActions.Select(c => c.CaseStream.CaseStreamId);
        var originalCaseStreamAllocations =
          new XPQuery<STR_CaseStreamAllocation>(uow).Where(
            c =>
              caseStreamIds.Contains(c.CaseStream.CaseStreamId) &&
              c.CaseStream.CaseStreamAllocations.Any(
                a => a.Escalation.EscalationType == Framework.Enumerators.Stream.EscalationType.None)).ToList();
        caseStreamActionsDto.ForEach(caseStreamAction =>
        {
          var originalCaseStreamAllocation =
            originalCaseStreamAllocations.FirstOrDefault(
              c => c.CaseStream.CaseStreamId == caseStreamAction.CaseStreamId && !c.TransferredOut);
          if (originalCaseStreamAllocation != null)
          {
            caseStreamAction.CaseStreamAllocations.Add(new CaseStreamAllocation
            {
              CaseStreamId = originalCaseStreamAllocation.CaseStream.CaseStreamId,
              CompleteDate = originalCaseStreamAllocation.CompleteDate,
              SmsCount = originalCaseStreamAllocation.SMSCount,
              AllocatedDate = originalCaseStreamAllocation.AllocatedDate,
              AllocatedUser =
                originalCaseStreamAllocation.AllocatedUser.Security == null
                  ? originalCaseStreamAllocation.AllocatedUser.Firstname + " " +
                    originalCaseStreamAllocation.AllocatedUser.Lastname
                  : originalCaseStreamAllocation.AllocatedUser.Security.Username,
              AllocatedUserId = originalCaseStreamAllocation.AllocatedUser.PersonId,
              CaseStreamAllocationId = originalCaseStreamAllocation.CaseStreamAllocationId,
              CompleteComment =
                originalCaseStreamAllocation.CompleteComment == null
                  ? string.Empty
                  : originalCaseStreamAllocation.CompleteComment.Description,
              EscalationType =
                originalCaseStreamAllocation.Escalation == null
                  ? Framework.Enumerators.Stream.EscalationType.None
                  : originalCaseStreamAllocation.Escalation.EscalationType,
              NoActionCount = originalCaseStreamAllocation.NoActionCount,
              TransferredIn = originalCaseStreamAllocation.TransferredIn,
              TransferredOut = originalCaseStreamAllocation.TransferredOut,
              TransferredOutDate = originalCaseStreamAllocation.TransferredOutDate
            });
          }
        });

        return new List<IEscalatedCaseStream>(caseStreamActionsDto);
      }
    }

    public Tuple<IWorkItem, List<INote>, string> GetNextWorkItem(
      Framework.Enumerators.Stream.GroupType groupType, string userId, Framework.Enumerators.Stream.StreamType streamType)
    {
      var bufferTime = new TimeSpan(0, 15, 0);
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        var groupId = (int)groupType;
        var person = _userService.Get(userId);
        if (person == null)
          throw new Exception(string.Format("UserId {0} does not exist", userId));

        var caseStatuses = new[] { (int)CaseStatus.Type.New, (int)CaseStatus.Type.InProgress };

        //var allocatedCaseStreams =
        //  new XPQuery<STR_CaseStreamAllocation>(uow).Where(
        //    a => a.AllocatedUser.PersonId == person.PersonId && !a.TransferredOut
        //         && a.Escalation == a.CaseStream.Escalation
        //         && caseStatuses.Contains(a.CaseStream.Case.CaseStatus.CaseStatusId)
        //         && a.CaseStream.Case.Group.GroupId == groupId).Select(a => a.CaseStream).ToList();

        const int actionTypeId = (int)Action.Type.Action;
        var caseQuery = new XPQuery<STR_CaseStreamAction>(uow).Where(a =>
          a.CaseStream.CaseStreamAllocations.Any(s => s.AllocatedUser.PersonId == person.PersonId && !s.TransferredOut
                                                      &&
                                                      s.Escalation.EscalationId == a.CaseStream.Escalation.EscalationId
                                                      &&
                                                      caseStatuses.Contains(s.CaseStream.Case.CaseStatus.CaseStatusId)
                                                      && s.CaseStream.Case.Group.GroupId == groupId)
          && !a.CaseStream.CompleteDate.HasValue
          && a.ActionDate <= DateTime.Now.Add(bufferTime)
          && a.ActionType.ActionTypeId != actionTypeId);

        if (streamType != 0)
        {
          var streamTypeId = (int)streamType;
          caseQuery = caseQuery.Where(p => p.CaseStream.Stream.StreamId == streamTypeId);
        }
        if (groupType == Framework.Enumerators.Stream.GroupType.Collections)
        {
          caseQuery = caseQuery.OrderBy(p => p.ActionDate).ThenByDescending(p => p.CaseStream.Stream.Priority.Value)
            .ThenByDescending(p => p.CaseStream.Case.Priority == null ? 0 : p.CaseStream.Case.Priority.Value)
            .ThenByDescending(p => p.CaseStream.Priority.Value)
            .ThenByDescending(c => c.CaseStream.Case.SubCategory.Category.CategoryId);
        }
        else
        {
          caseQuery = caseQuery.OrderBy(p => p.ActionDate).ThenByDescending(p => p.CaseStream.Stream.Priority.Value)
            .ThenByDescending(p => p.CaseStream.Priority.Value)
            .ThenBy(c => c.CaseStream.Case.SubCategory.SubCategoryId);
        }
        var caseStreamAction = caseQuery.FirstOrDefault();
        if (caseStreamAction == null) return null;
        caseStreamAction.DateActioned = DateTime.Now;
        caseStreamAction.CaseStream.Case.WorkableCase = true;
        if (caseStreamAction.CaseStream.Case.CaseStatus.CaseStatusId ==
            CaseStatus.Type.New.ToInt())
        {
          ChangeCaseStatus(caseStreamAction.CaseStream.Case.CaseId, General.Person.System.ToInt(), CaseStatus.Type.InProgress.ToInt());
        }

        AddAccountNote(personId: person.PersonId,
          note:
            string.Format("Case {0} in Stream {1}, actioned to [{2} {3}]", caseStreamAction.CaseStream.Case.CaseId,
              caseStreamAction.CaseStream.Stream.Description, person.Firstname, person.Lastname),
          accountNoteType: Framework.Enumerators.Stream.AccountNoteType.Normal, uow: uow,
          caseId: caseStreamAction.CaseStream.Case.CaseId);

        uow.CommitChanges();

        // get previoues CaseStream for completed reason + note
        var previousCaseStream =
          new XPQuery<STR_CaseStream>(uow).Where(
            c =>
              c.Case.CaseId == caseStreamAction.CaseStream.Case.CaseId &&
              c.CaseStreamId != caseStreamAction.CaseStream.CaseStreamId
              && c.CreateDate < caseStreamAction.CaseStream.CreateDate && c.CompleteDate.HasValue)
            .OrderByDescending(c => c.CreateDate)
            .FirstOrDefault();
        var completeNote = string.Empty;
        if (previousCaseStream != null)
          completeNote = string.Format("{0}: {1}",
            previousCaseStream.CompleteComment == null ? string.Empty : previousCaseStream.CompleteComment.Description,
            previousCaseStream.CompleteNote == null
              ? string.Empty
              : previousCaseStream.CompleteNote.Note);

        // get notes for case
        var notes =
          new XPQuery<STR_Note>(uow).Where(n => n.Case.CaseId == caseStreamAction.CaseStream.Case.CaseId)
            .OrderByDescending(n => n.CreateDate)
            .ToList();

        var notesDto = notes.Select(note => new Notes
        {
          NoteId = note.NoteId,
          Note = note.Note,
          CreateDate = note.CreateDate
        }).Cast<INote>().ToList();

        var workItem = new WorkItem
        {
          CaseReference = caseStreamAction.CaseStream.Case.Reference,
          ActionAmount = caseStreamAction.Amount,
          ActionDate = caseStreamAction.ActionDate,
          ActionTypeId = caseStreamAction.ActionType.ActionTypeId,
          ArrearsAmount = caseStreamAction.CaseStream.Case.TotalArrearsAmount,
          Balance = caseStreamAction.CaseStream.Case.TotalBalance,
          BranchId = caseStreamAction.CaseStream.Case.Branch.BranchId,
          CaseId = caseStreamAction.CaseStream.Case.CaseId,
          CaseStreamActionId = caseStreamAction.CaseStreamActionId,
          CaseStreamId = caseStreamAction.CaseStream.CaseStreamId,
          Category = caseStreamAction.CaseStream.Case.SubCategory.Category.Description,
          DebtorCell = new List<IContact>(),
          DebtorEmail = new List<IContact>(),
          DebtorFax = new List<IContact>(),
          DebtorFirstname = caseStreamAction.CaseStream.Case.Debtor.FirstName,
          DebtorId = caseStreamAction.CaseStream.Case.Debtor.DebtorId,
          DebtorIdNumber = caseStreamAction.CaseStream.Case.Debtor.IdNumber,
          DebtorLastname = caseStreamAction.CaseStream.Case.Debtor.LastName,
          DebtorWorkNo = new List<IContact>(),
          EscalationId = caseStreamAction.CaseStream.Escalation.EscalationId,
          Group = caseStreamAction.CaseStream.Case.Group.Description,
          GroupId = caseStreamAction.CaseStream.Case.Group.GroupId,
          InstalmentsOustanding = caseStreamAction.CaseStream.Case.TotalInstalmentsOutstanding,
          LastReceiptAmount = caseStreamAction.CaseStream.Case.LastReceiptAmount,
          LastReceiptDate = caseStreamAction.CaseStream.Case.LastReceiptDate,
          LoanAmount = caseStreamAction.CaseStream.Case.TotalLoanAmount,
          StreamId = caseStreamAction.CaseStream.Stream.StreamId,
          SubCategory = caseStreamAction.CaseStream.Case.SubCategory.Description
        };

        // populate Accounts
        var accounts = caseStreamAction.CaseStream.Case.Debtor.Accounts.OrderByDescending(a => a.LoanDate).ToList();
        workItem.LinkedAccounts = new List<IWorkItemAccount>();
        workItem.LinkedAccounts.AddRange(
          accounts.Select(a => new WorkItemAccount
          {
            AccountId = a.AccountId,
            Arrears = a.ArrearsAmount,
            Frequency = a.Frequency.Description,
            InArrears = a.ArrearsAmount > 0,
            InstalmentsOutstanding = a.InstalmentsOutstanding,
            LastReceiptAmount = a.LastReceiptAmount,
            LastReceiptDate = a.LastReceiptDate,
            LoanTerm = a.LoanTerm,
            Reference = a.Reference
          }));

        var lastLoan = accounts.FirstOrDefault();
        if (lastLoan != null)
        {
          workItem.LastLoanAmount = lastLoan.LoanAmount;
          workItem.LastLoanDate = lastLoan.LoanDate;
          workItem.LastLoanTerm = lastLoan.LoanTerm;
          workItem.LastLoanFrequency = lastLoan.Frequency.Description;
        }

        // get transactions
        workItem.Transactions = new List<ITransaction>();
        var transactions = GetCaseTransactions(caseStreamAction.CaseStream.Case.CaseId).OrderBy(c => c.AccountReference);
        workItem.Transactions.AddRange(transactions.Select(t => new Transaction
        {
          Amount = t.Amount,
          AccountId = t.AccountId,
          Reference = t.Reference,
          AccountReference = t.AccountReference,
          TransactionStatus = t.TransactionStatus,
          CreateDate = t.CreateDate,
          InstalmentNumber = t.InstalmentNumber,
          TransactionDate = t.TransactionDate,
          TransactionId = t.TransactionId,
          TransactionType = t.TransactionType
        }));

        // get contacts
        workItem.DebtorCell.AddRange(caseStreamAction.CaseStream.Case.Debtor.Contacts.Where(c =>
          c.ContactType != null &&
          c.ContactType.ContactTypeId == General.ContactType.CellNo.ToInt()).Select(CastContact).ToList());
        workItem.DebtorEmail.AddRange(caseStreamAction.CaseStream.Case.Debtor.Contacts.Where(c =>
          c.ContactType != null &&
          c.ContactType.ContactTypeId == General.ContactType.Email.ToInt()).Select(CastContact).ToList());
        workItem.DebtorFax.AddRange(caseStreamAction.CaseStream.Case.Debtor.Contacts.Where(c =>
          c.ContactType != null &&
          c.ContactType.ContactTypeId == General.ContactType.TelNoWorkFax.ToInt()).Select(CastContact).ToList());
        workItem.DebtorWorkNo.AddRange(caseStreamAction.CaseStream.Case.Debtor.Contacts.Where(c =>
          c.ContactType != null &&
          c.ContactType.ContactTypeId == General.ContactType.TelNoWork.ToInt()).Select(CastContact).ToList());

        // get no action count of the consultant or the original consultant
        var caseStreamAllocations =
          caseStreamAction.CaseStream.CaseStreamAllocations.FirstOrDefault(
            a => a.AllocatedUser.PersonId == person.PersonId) ??
          caseStreamAction.CaseStream.CaseStreamAllocations.FirstOrDefault(
            a => a.Escalation.EscalationType == Framework.Enumerators.Stream.EscalationType.None && !a.TransferredOut);
        if (caseStreamAllocations != null)
          workItem.NoActionCount = caseStreamAllocations.NoActionCount;

        // can perform a new credit enquiry if the last receipt date is null or is more than 90days ago
        workItem.PerformNewCreditEnquiry = caseStreamAction.CaseStream.Case.LastReceiptDate.HasValue
          ? ((DateTime.Today - caseStreamAction.CaseStream.Case.LastReceiptDate.Value).TotalDays > 90)
          : ((DateTime.Today - caseStreamAction.CaseStream.Case.CreateDate).TotalDays > 90);

        return new Tuple<IWorkItem, List<INote>, string>(workItem, notesDto, completeNote);
      }
    }

    private Falcon.Common.Structures.Contact CastContact(STR_DebtorContact contact)
    {
      if (contact == null || contact.ContactType == null)
        return null;
      return new Falcon.Common.Structures.Contact
      {
        ContactType = new Falcon.Common.Structures.ContactType
        {
          Description = contact.ContactType.Description,
          Type = (General.ContactType)contact.ContactType.ContactTypeId,
          ContactTypeId = contact.ContactType.ContactTypeId
        },
        DebtorContactId = contact.DebtorContactId,
        Value = contact.Value
      };
    }

    public Tuple<IWorkItem, List<INote>, string> GetWorkItem(long caseStreamActionId, string userId)
    {
      var person = _userService.Get(userId);
      if (person == null)
        throw new Exception(string.Format("UserId {0} does not exist", userId));
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        uow.TrackPropertiesModifications = false;
        var caseStreamAction =
          new XPQuery<STR_CaseStreamAction>(uow).FirstOrDefault(p => p.CaseStreamActionId == caseStreamActionId
          //&& p.CaseStream.CaseStreamAllocations.Any(a => a.AllocatedUser.PersonId == person.PersonId && !a.TransferredOut)
            );
        if (caseStreamAction == null)
        {
          throw new Exception(string.Format("Case Stream Action with Id {0} does not exist", caseStreamActionId));
        }
        if (caseStreamAction.CompleteDate == null)
        {
          caseStreamAction.DateActioned = DateTime.Now;

          //AddAccountNote(accountId: caseStreamAction.CaseStream.Case.Account.AccountId, userId: userId,
          //  note: string.Format("Case {0} in Stream {1}, actioned to {2}", caseStreamAction.CaseStream.Case.CaseId, caseStreamAction.CaseStream.Stream.Description, caseStreamAction.CaseStream.CaseStreamAllocations.FirstOrDefault(a => a.Escalation.EscalationType == Framework.Enumerators.Stream.EscalationType.None && !a.TransferredOut).AllocatedUser.Security.Username),
          //  accountNoteType: Framework.Enumerators.Stream.AccountNoteType.Normal, uow: uow, caseId: caseStreamAction.CaseStream.Case.CaseId);
        }
        // get previoues CaseStream for completed reason + note
        var previousCaseStream =
          new XPQuery<STR_CaseStream>(uow).Where(
            c =>
              c.Case.CaseId == caseStreamAction.CaseStream.Case.CaseId &&
              c.CaseStreamId != caseStreamAction.CaseStream.CaseStreamId
              && c.CreateDate < caseStreamAction.CaseStream.CreateDate && c.CompleteDate.HasValue)
            .OrderByDescending(c => c.CreateDate)
            .FirstOrDefault();
        var completeNote = string.Empty;
        if (previousCaseStream != null && previousCaseStream.CompleteComment != null)
          completeNote =
            $"{previousCaseStream.CompleteComment.Description}: {(previousCaseStream.CompleteNote == null ? string.Empty : previousCaseStream.CompleteNote.Note)}";

        var notes =
          new XPQuery<STR_Note>(uow).Where(n => n.Case.CaseId == caseStreamAction.CaseStream.Case.CaseId)
            .OrderByDescending(n => n.CreateDate)
            .ToList();

        var notesDto = notes.Select(note => new Notes
        {
          NoteId = note.NoteId,
          Note = note.Note,
          CreateDate = note.CreateDate
        }).Cast<INote>().ToList();

        //var caseStreamActionDTO = _mappingEngine.Map<CaseStreamAction, ICaseStreamAction>(_mappingEngine.Map<STR_CaseStreamAction, CaseStreamAction>(caseStreamAction));

        var workItem = new WorkItem
        {
          CaseReference = caseStreamAction.CaseStream.Case.Reference,
          ActionAmount = caseStreamAction.Amount,
          ActionDate = caseStreamAction.ActionDate,
          ActionTypeId = caseStreamAction.ActionType.ActionTypeId,
          ArrearsAmount = caseStreamAction.CaseStream.Case.TotalArrearsAmount,
          Balance = caseStreamAction.CaseStream.Case.TotalBalance,
          BranchId = caseStreamAction.CaseStream.Case.Branch.BranchId,
          CaseId = caseStreamAction.CaseStream.Case.CaseId,
          CaseStreamActionId = caseStreamAction.CaseStreamActionId,
          CaseStreamId = caseStreamAction.CaseStream.CaseStreamId,
          Category = caseStreamAction.CaseStream.Case.SubCategory.Category.Description,
          DebtorCell = new List<IContact>(),
          DebtorEmail = new List<IContact>(),
          DebtorFax = new List<IContact>(),
          DebtorFirstname = caseStreamAction.CaseStream.Case.Debtor.FirstName,
          DebtorId = caseStreamAction.CaseStream.Case.Debtor.DebtorId,
          DebtorIdNumber = caseStreamAction.CaseStream.Case.Debtor.IdNumber,
          DebtorLastname = caseStreamAction.CaseStream.Case.Debtor.LastName,
          DebtorWorkNo = new List<IContact>(),
          EscalationId = caseStreamAction.CaseStream.Escalation.EscalationId,
          Group = caseStreamAction.CaseStream.Case.Group.Description,
          GroupId = caseStreamAction.CaseStream.Case.Group.GroupId,
          InstalmentsOustanding = caseStreamAction.CaseStream.Case.TotalInstalmentsOutstanding,
          LastReceiptAmount = caseStreamAction.CaseStream.Case.LastReceiptAmount,
          LastReceiptDate = caseStreamAction.CaseStream.Case.LastReceiptDate,
          LoanAmount = caseStreamAction.CaseStream.Case.TotalLoanAmount,
          StreamId = caseStreamAction.CaseStream.Stream.StreamId,
          SubCategory = caseStreamAction.CaseStream.Case.SubCategory.Description
        };

        // get contacts
        workItem.DebtorCell.AddRange(caseStreamAction.CaseStream.Case.Debtor.Contacts.Where(c =>
          c.ContactType.ContactTypeId == General.ContactType.CellNo.ToInt()).Select(CastContact).ToList());
        workItem.DebtorEmail.AddRange(caseStreamAction.CaseStream.Case.Debtor.Contacts.Where(c =>
          c.ContactType.ContactTypeId== General.ContactType.Email.ToInt()).Select(CastContact).ToList());
        workItem.DebtorFax.AddRange(caseStreamAction.CaseStream.Case.Debtor.Contacts.Where(c =>
          c.ContactType.ContactTypeId == General.ContactType.TelNoWorkFax.ToInt()).Select(CastContact).ToList());
        workItem.DebtorWorkNo.AddRange(caseStreamAction.CaseStream.Case.Debtor.Contacts.Where(c =>
          c.ContactType.ContactTypeId == General.ContactType.TelNoWork.ToInt()).Select(CastContact).ToList());

        // get no action count of the consultant or the original consultant
        var caseStreamAllocations =
          caseStreamAction.CaseStream.CaseStreamAllocations.FirstOrDefault(
            a => a.AllocatedUser.PersonId == person.PersonId) ??
          caseStreamAction.CaseStream.CaseStreamAllocations.FirstOrDefault(
            a => a.Escalation.EscalationType == Framework.Enumerators.Stream.EscalationType.None && !a.TransferredOut);
        if (caseStreamAllocations != null)
          workItem.NoActionCount = caseStreamAllocations.NoActionCount;

        // populate Accounts
        var accounts = caseStreamAction.CaseStream.Case.Debtor.Accounts.OrderByDescending(a => a.LoanDate).ToList();
        workItem.LinkedAccounts = new List<IWorkItemAccount>();
        workItem.LinkedAccounts.AddRange(
          accounts.Select(a => new WorkItemAccount
          {
            AccountId = a.AccountId,
            Arrears = a.ArrearsAmount,
            Frequency = a.Frequency.Description,
            InArrears = a.ArrearsAmount > 0,
            InstalmentsOutstanding = a.InstalmentsOutstanding,
            LastReceiptAmount = a.LastReceiptAmount,
            LastReceiptDate = a.LastReceiptDate,
            LoanTerm = a.LoanTerm,
            Reference = a.Reference
          }));

        var lastLoan = accounts.FirstOrDefault();
        if (lastLoan != null)
        {
          workItem.LastLoanAmount = lastLoan.LoanAmount;
          workItem.LastLoanDate = lastLoan.LoanDate;
          workItem.LastLoanTerm = lastLoan.LoanTerm;
          workItem.LastLoanFrequency = lastLoan.Frequency.Description;
        }

        // get transactions
        workItem.Transactions = new List<ITransaction>();
        var transactions =
          GetCaseTransactions(caseStreamAction.CaseStream.Case.CaseId)
            .OrderBy(c => c.AccountReference)
            .ThenBy(t => t.TransactionDate)
            .ThenBy(t => t.InstalmentNumber);
        workItem.Transactions.AddRange(transactions.Select(t => new Transaction
        {
          Amount = t.Amount,
          AccountId = t.AccountId,
          Reference = t.Reference,
          AccountReference = t.AccountReference,
          TransactionStatus = t.TransactionStatus,
          CreateDate = t.CreateDate,
          InstalmentNumber = t.InstalmentNumber,
          TransactionDate = t.TransactionDate,
          TransactionId = t.TransactionId,
          TransactionType = t.TransactionType
        }));

        // can perform a new credit enquiry if the last receipt date is null or is more than 90days ago
        workItem.PerformNewCreditEnquiry = caseStreamAction.CaseStream.Case.LastReceiptDate.HasValue
          ? ((DateTime.Today - caseStreamAction.CaseStream.Case.LastReceiptDate.Value).TotalDays > 90)
          : ((DateTime.Today - caseStreamAction.CaseStream.Case.CreateDate).TotalDays > 90);

        return new Tuple<IWorkItem, List<INote>, string>(workItem, notesDto, completeNote);
      }
    }

    public List<IStream> GetStreamTypes(Framework.Enumerators.Stream.GroupType groupType)
    {
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        uow.TrackPropertiesModifications = false;
        var groupId = (int)groupType;
        var streams =
          new XPQuery<STR_GroupStream>(uow).Where(s => s.Group.GroupId == groupId).Select(a => a.Stream).ToList();

        var streamDtos = streams.Select(stream => new Structures.Models.Stream
        {
          Description = stream.Description,
          StreamType = (Framework.Enumerators.Stream.StreamType)stream.StreamId,
          DefaultCaseStreamPriority = stream.DefaultCaseStreamPriority.PriorityType,
          Priority = stream.Priority.PriorityType,
          StreamId = stream.StreamId
        });

        return new List<IStream>(streamDtos);
      }
    }

    public IEnumerable<CaseStatus.Type> GetCaseStatuses()
    {
      return EnumUtil.GetValues<CaseStatus.Type>();
    }

    public Dictionary<string, List<Tuple<IStream, int>>> GetWorkItemsSummary(
      Framework.Enumerators.Stream.GroupType groupType,
      List<string> userIds, params Framework.Enumerators.Stream.StreamType[] streamTypes)
    {
      var workSummaryItems = new Dictionary<string, List<Tuple<IStream, int>>>();
      var bufferTime = new TimeSpan(0, 15, 0);
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        uow.TrackPropertiesModifications = false;
        var persons = userIds.Select(userId => _userService.Get(userId)).Where(person => person != null).ToList();
        var personIds = persons.Select(p => p.PersonId).ToList();

        var groupId = (int)groupType;
        var caseStatuses = new[] { (int)CaseStatus.Type.New, (int)CaseStatus.Type.InProgress };

        var streamIds = streamTypes.Select(s => (int)s).ToList();

        const int actionTypeId = (int)Action.Type.Action;
        var caseStreams = new XPQuery<STR_CaseStreamAction>(uow).Where(a =>
          a.CompleteDate == null
          && a.ActionDate <= DateTime.Now.Add(bufferTime)
          && streamIds.Contains(a.CaseStream.Stream.StreamId)
          && a.ActionType.ActionTypeId != actionTypeId).Select(a => a.CaseStream).ToList();

        var caseStreamAllocations = new List<PersonStreamTypes>();
        const int batches = 1000;
        var batchRuns = caseStreams.Count / batches;
        batchRuns += caseStatuses.Length % batches > 0 ? 1 : 0;
        for (var i = 0; i < batchRuns; i++)
        {
          var tempCaseStreams = caseStreams.Skip(i * batches).Take(batches);
          var tempCaseStreamAllocations = new XPQuery<STR_CaseStreamAllocation>(uow).Where(
            a => personIds.Contains(a.AllocatedUser.PersonId) && !a.TransferredOut
                 && a.Escalation.EscalationId == a.CaseStream.Escalation.EscalationId
                 && caseStatuses.Contains(a.CaseStream.Case.CaseStatus.CaseStatusId)
                 && streamIds.Contains(a.CaseStream.Stream.StreamId)
                 && a.CaseStream.Case.Group.GroupId == groupId
                 && tempCaseStreams.Contains(a.CaseStream))
            .Select(
              c =>
                new PersonStreamTypes
                {
                  PersonId = c.AllocatedUser.PersonId,
                  StreamType = (Framework.Enumerators.Stream.StreamType)c.CaseStream.Stream.StreamId
                })
            .ToList();

          caseStreamAllocations.AddRange(tempCaseStreamAllocations);
        }

        var streams = new XPQuery<STR_Stream>(uow).Where(p => streamIds.Contains(p.StreamId)).ToList();
        foreach (var person in persons)
        {
          var userStreams =
            streams.Select(
              stream =>
                new Tuple<IStream, int>(
                  new Structures.Models.Stream
                  {
                    Description = stream.Description,
                    StreamType = (Framework.Enumerators.Stream.StreamType)stream.StreamId,
                    DefaultCaseStreamPriority = stream.DefaultCaseStreamPriority.PriorityType,
                    Priority = stream.Priority.PriorityType,
                    StreamId = stream.StreamId
                  },
                  caseStreamAllocations.Count(
                    c =>
                      c.PersonId == person.PersonId && c.StreamType == (Framework.Enumerators.Stream.StreamType)stream.StreamId)))
              .ToList();
          if (person.WebReference != null) workSummaryItems.Add(person.WebReference, userStreams);
        }

        return workSummaryItems;
      }
    }

    private class PersonStreamTypes
    {
      public long PersonId { get; set; }
      public Framework.Enumerators.Stream.StreamType StreamType { get; set; }
    }

    /// <summary>
    /// Get account.Reference2 and LastImportReference by group type and case status
    /// </summary>
    /// <param name="groupType"></param>
    /// <param name="caseStatuses"></param>
    /// <returns></returns>
    public Dictionary<long, string> GetAccountReferenceByCaseStatus(Framework.Enumerators.Stream.GroupType[] groupType,
      params CaseStatus.Type[] caseStatuses)
    {
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        uow.TrackPropertiesModifications = false;
        var caseStatusIds = caseStatuses.Select(a => (int)a).ToList();
        var groupIds = groupType.Select(a => (int)a).ToList();
        return new XPQuery<STR_Case>(uow).Where(c => groupIds.Contains(c.Group.GroupId)
                                                     && caseStatusIds.Contains(c.CaseStatus.CaseStatusId)).ToList()
          .SelectMany(a => a.Debtor.Accounts.Select(n => new { n.Reference2, n.LastImportReference }))
          .Distinct()
          .OrderBy(c => c.LastImportReference)
          .ToDictionary(k => k.Reference2, v => v.LastImportReference);
      }
    }

    public List<long> GetClientReferencesByAccountStatus(Framework.Enumerators.Stream.GroupType[] groupType,
      long? branchId,
      params CaseStatus.Type[] caseStatuses)
    {
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        uow.TrackPropertiesModifications = false;
        var caseStatusIds = caseStatuses.Select(a => (int)a).ToList();
        var groupIds = groupType.Select(a => (int)a).ToList();
        var clientReferencesQuery =
          new XPQuery<STR_Case>(uow).Where(
            c => groupIds.Contains(c.Group.GroupId) && caseStatusIds.Contains(c.CaseStatus.CaseStatusId));

        if (branchId.HasValue)
          clientReferencesQuery = clientReferencesQuery.Where(c => c.Branch.BranchId == branchId);

        var page = 1;
        const int itemsPerPage = 1000;

        var clientReferences = new List<long>();
        List<long> tempClientReferences;

        do
        {
          tempClientReferences = clientReferencesQuery
            .OrderBy(c => c.Debtor.DebtorId)
            .Skip((page - 1) * itemsPerPage)
            .Take(itemsPerPage)
            .ToList()
            .Select(a => a.Debtor.Reference)
            .Distinct()
            .ToList();

          clientReferences.AddRange(tempClientReferences);
          page++;
        } while (tempClientReferences.Count > 0);

        return clientReferences;
      }
    }

    public Dictionary<long, DateTime> GetClientReferencesAndLoanDateByAccountStatus(
      Framework.Enumerators.Stream.GroupType[] groupType,
      long branchId, params CaseStatus.Type[] caseStatuses)
    {
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        uow.TrackPropertiesModifications = false;
        var caseStatusIds = caseStatuses.Select(a => (int)a).ToList();
        var groupIds = groupType.Select(a => (int)a).ToList();
        var clientReferences =
          new XPQuery<STR_Case>(uow).Where(
            c =>
              groupIds.Contains(c.Group.GroupId) && c.Branch.BranchId == branchId &&
              caseStatusIds.Contains(c.CaseStatus.CaseStatusId)).ToList()
            .SelectMany(a => a.Debtor.Accounts.Select(n => new { a.Debtor.Reference, n.LoanDate }))
            .OrderByDescending(d => d.LoanDate)
            .Distinct()
            .ToList();

        var clientLoanDates = new Dictionary<long, DateTime>();
        clientReferences.ForEach(clientReference =>
        {
          if (!clientLoanDates.ContainsKey(clientReference.Reference))
            clientLoanDates.Add(clientReference.Reference, clientReference.LoanDate);
        });

        return clientLoanDates;
      }
    }

    /// <summary>
    /// Get All case stream actions based on filter
    /// </summary>
    /// <param name="groupType"></param>
    /// <param name="completeDate"></param>
    /// <param name="actionDates"></param>
    /// <param name="actionTypes"></param>
    /// <param name="caseStatuses"></param>
    /// <param name="streamType"></param>
    /// <returns></returns>
    public List<IGetCaseStreamAction> GetCaseStreamActions(Framework.Enumerators.Stream.GroupType groupType,
      DateTime? completeDate, List<DateTime> actionDates, Action.Type[] actionTypes,
      CaseStatus.Type[] caseStatuses, params Framework.Enumerators.Stream.StreamType[] streamType)
    {
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        uow.TrackPropertiesModifications = false;
        var groupId = (int)groupType;
        var streamIds = streamType.Select(s => s.ToInt());
        var caseStreamActionsQuery =
          new XPQuery<STR_CaseStreamAction>(uow).Where(
            p => p.CaseStream.Case.Group.GroupId == groupId && streamIds.Contains(p.CaseStream.Stream.StreamId));
        caseStreamActionsQuery = completeDate.HasValue
          ? caseStreamActionsQuery.Where(p => p.CompleteDate == completeDate)
          : caseStreamActionsQuery.Where(p => p.CompleteDate == null);
        if (actionDates != null && actionDates.Count > 0)
          caseStreamActionsQuery = caseStreamActionsQuery.Where(p => actionDates.Contains(p.ActionDate.Date));
        if (actionTypes.Count() > 0)
          caseStreamActionsQuery = caseStreamActionsQuery.Where(p => actionTypes.Contains(p.ActionType.Type));

        var caseStreamList = caseStreamActionsQuery.ToList();
        return caseStreamList.Select(c => new GetCaseStreamAction
        {
          AccountReference2 = c.CaseStream.Case.Debtor.Accounts.Select(a => a.Reference2).ToList(),
          CaseStreamActionId = c.CaseStreamActionId,
          CaseStreamId = c.CaseStream.CaseStreamId,
          Amount = c.Amount,
          ActionDate = c.ActionDate,
          ActionType = c.ActionType.Type,
          CompleteDate = c.CompleteDate,
          StreamType = (Framework.Enumerators.Stream.StreamType)c.CaseStream.Stream.StreamId
        }).Cast<IGetCaseStreamAction>().ToList();
      }
    }

    public IGetCaseStreamAction GetCaseStreamAction(long caseStreamId, DateTime? completeDate, DateTime actionDate,
      Action.Type actionType,
      Framework.Enumerators.Stream.StreamType streamType)
    {
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        uow.TrackPropertiesModifications = false;
        var caseStreamActionsQuery =
          new XPQuery<STR_CaseStreamAction>(uow).Where(
            p => p.CaseStream.CaseStreamId == caseStreamId && p.CaseStream.Stream.StreamId == streamType.ToInt());

        caseStreamActionsQuery = completeDate.HasValue
          ? caseStreamActionsQuery.Where(p => p.CompleteDate == completeDate)
          : caseStreamActionsQuery.Where(p => p.CompleteDate == null);

        caseStreamActionsQuery =
          caseStreamActionsQuery.Where(
            p => p.ActionDate.Date == actionDate && p.ActionType.ActionTypeId == actionType.ToInt());

        var caseStreamAction = caseStreamActionsQuery.OrderBy(d => d.ActionDate).FirstOrDefault();

        if (caseStreamAction == null)
        {
          return null;
        }

        return new GetCaseStreamAction
        {
          AccountReference2 = caseStreamAction.CaseStream.Case.Debtor.Accounts.Select(a => a.Reference2).ToList(),
          CaseStreamActionId = caseStreamAction.CaseStreamActionId,
          CaseStreamId = caseStreamAction.CaseStream.CaseStreamId,
          Amount = caseStreamAction.Amount,
          ActionDate = caseStreamAction.ActionDate,
          ActionType = caseStreamAction.ActionType.Type,
          CompleteDate = caseStreamAction.CompleteDate,
          StreamType = (Framework.Enumerators.Stream.StreamType)caseStreamAction.CaseStream.Stream.StreamId
        };
      }
    }

    public void MoveCaseToFollowUpStream(long caseStreamId, string userId, int commentId, string completeNote,
      DateTime actionDate)
    {
      if (actionDate < DateTime.Today)
        throw new Exception("Invalid Action Date");
      var person = _userService.Get(userId);
      if (person == null)
        throw new Exception(string.Format("UserId {0} does not exist", userId));
      var caseStream = MoveCaseToStream(caseStreamId: caseStreamId, personId: person.PersonId,
        newStream: Framework.Enumerators.Stream.StreamType.FollowUp, completedCommentId: commentId,
        completeNote: completeNote,
        makeDefaultAction: false, uow: null, newCaseStatus: CaseStatus.Type.InProgress);
      if (caseStream != null)
        AddCaseStreamAction(caseStream.CaseStreamId, actionDate, Action.Type.Reminder, false, true);
    }

    public void MoveCaseToPtpStream(long caseStreamId, string userId, int commentId, string completeNote, decimal amount,
      DateTime[] actionDates)
    {
      if (actionDates.Any(d => d < DateTime.Today))
        throw new Exception("Invalid Action Date");

      var person = _userService.Get(userId);
      if (person == null)
        throw new Exception(string.Format("UserId {0} does not exist", userId));
      var caseStream = MoveCaseToStream(caseStreamId: caseStreamId, personId: person.PersonId,
        newStream: Framework.Enumerators.Stream.StreamType.PTP, completedCommentId: commentId,
        completeNote: completeNote, makeDefaultAction: false, uow: null, newCaseStatus: CaseStatus.Type.InProgress);
      if (caseStream != null)
      {
        for (var i = 0; i < actionDates.Count(); i++)
        {
          AddCaseStreamAction(caseStream.CaseStreamId, actionDates[i].Date, Action.Type.Action,
            true, i == 0, null, amount);
          SendSms(caseStreamId: caseStream.CaseStreamId, personId: person.PersonId,
            smsTemplate: Atlas.Enumerators.Notification.NotificationTemplate.Stream_SMS_PTPCreated,
            actionDate: actionDates[i].Date, amount: amount);


          if (actionDates[i].Date > DateTime.Today.AddDays(1))
          {
            // if the action date is after tomorrow
            // add reminder for the day before
            var daysDiff = actionDates[i].DayOfWeek == DayOfWeek.Monday ? 2 : 1;
            AddCaseStreamAction(caseStream.CaseStreamId,
              actionDates[i].AddDays(-daysDiff), Action.Type.Reminder,
              true, false,
              null, amount);
          }
          else if (actionDates[i] > DateTime.Now.AddMinutes(59))
          {
            // add reminder for 30 minutes prior
            AddCaseStreamAction(caseStream.CaseStreamId,
              actionDates[i].AddMinutes(-30), Action.Type.Reminder, true, false,
              null, amount);
          }
          // if the action date is more than 3 days from now, set up reminder for consultant to all debtor
          if (Math.Abs((actionDates[i].Date - DateTime.Today).Days) > 3)
          // if actiondate.day of week, then consultant needs to call on saturday, otherwise the day before 
          {
            var daysDiff = actionDates[i].DayOfWeek == DayOfWeek.Tuesday ? 3 : 2;
            AddCaseStreamAction(caseStream.CaseStreamId,
              actionDates[i].AddDays(-daysDiff),
              Action.Type.Reminder, true, false, null, amount);
          }
        }
      }
    }

    public void MoveCaseToPtcStream(long caseStreamId, string userId, int commentId, string completeNote,
      DateTime actionDate)
    {
      if (actionDate <= DateTime.Today)
        throw new Exception("Invalid Action Date");

      var person = _userService.Get(userId);
      if (person == null)
        throw new Exception(string.Format("UserId {0} does not exist", userId));
      var caseStream = MoveCaseToStream(caseStreamId: caseStreamId, personId: person.PersonId,
        newStream: Framework.Enumerators.Stream.StreamType.PTC, completedCommentId: commentId,
        completeNote: completeNote,
        makeDefaultAction: false, uow: null, newCaseStatus: CaseStatus.Type.InProgress);
      if (caseStream != null)
      {
        AddCaseStreamAction(caseStream.CaseStreamId, actionDate, Action.Type.Reminder, false, true);
        if (Math.Abs((actionDate.Date - DateTime.Today).Days) > 3)
        {
          var daysDiff = (actionDate.DayOfWeek == DayOfWeek.Tuesday ? 3 : 2);
          AddCaseStreamAction(caseStream.CaseStreamId,
            actionDate.AddDays(-daysDiff),
            Action.Type.Reminder, false, true);
        }
        SendSms(caseStreamId: caseStreamId, personId: person.PersonId,
          smsTemplate: Atlas.Enumerators.Notification.NotificationTemplate.Stream_SMS_PTCCreated, actionDate: actionDate);
      }
    }

    public void MovePtcCaseToPtcBrokenStream(long caseStreamId, string userId, int commentId, string completeNote)
    {
      var person = _userService.Get(userId);
      if (person == null)
        throw new Exception(string.Format("UserId {0} does not exist", userId));
      // move PTC into broken
      long caseId;
      using (var uow = new UnitOfWork())
      {
        var caseStream = new XPQuery<STR_CaseStream>(uow).FirstOrDefault(c => c.CaseStreamId == caseStreamId);
        if (caseStream == null)
          throw new Exception(string.Format("Case Stream with Id: {0} not found in DB", caseStreamId));
        if (caseStream.Stream.StreamId != Framework.Enumerators.Stream.StreamType.PTC.ToInt())
          throw new Exception(string.Format("Case Stream with Id: {0} is no in PTC stream", caseStreamId));
        var caseStreamAction =
          new XPQuery<STR_CaseStreamAction>(uow).Where(
            c => c.CaseStream.CaseStreamId == caseStream.CaseStreamId && c.CompleteDate == null)
            .OrderByDescending(c => c.ActionDate)
            .FirstOrDefault();
        if (caseStreamAction != null)
        {
          caseStreamAction.IsSuccess = false;
        }

        caseId = caseStream.Case.CaseId;

        uow.CommitChanges();
      }

      MoveCaseToStream(caseStreamId: caseStreamId, personId: person.PersonId,
        newStream: Framework.Enumerators.Stream.StreamType.PTCBroken, completedCommentId: commentId,
        completeNote: completeNote,
        makeDefaultAction: false, uow: null, newCaseStatus: CaseStatus.Type.InProgress);

      Escalate3ConsecutiveBrokenPtc(caseId: caseId, uow: null);
      // move PTC broken to completed
      //var caseStreamCompleted = MoveCaseToStream(caseStreamId: caseStreamPTCBroken.CaseStreamId, personId: person.PersonId, newStream: Framework.Enumerators.Stream.StreamType.Completed, completedCommentId: (int)Framework.Enumerators.Stream.Comment.Complete, completeNote: completeNote, cancelPendingActions: true,
      //  makeDefaultAction: false, uow: null, returnIfPendingActionsExist: false);

      //if (actionDate.HasValue)
      //  AddCaseStreamAction(caseStream.CaseStreamId, actionDate.Value, Framework.Enumerators.Action.Type.Reminder, false, true);

      SendSms(caseStreamId: caseStreamId, personId: person.PersonId,
        smsTemplate: Atlas.Enumerators.Notification.NotificationTemplate.Stream_SMS_PTPBroken);
    }

    // find 3 consecutive PTC Broken streams, and then excalate the case
    private void Escalate3ConsecutiveBrokenPtc(long caseId, UnitOfWork uow)
    {
      var commitChanges = uow == null;
      if (commitChanges)
        uow = new UnitOfWork();

      var caseStreams =
        new XPQuery<STR_CaseStream>(uow).Where(c => c.Case.CaseId == caseId)
          .OrderByDescending(c => c.CreateDate)
          .ToList();
      var consecutivePtcBrokens =
        caseStreams.Count(
          caseStream => caseStream.Stream.StreamId == Framework.Enumerators.Stream.StreamType.PTCBroken.ToInt());
      if (consecutivePtcBrokens == 3)
      {
        // escalate 
        var incompleteCaseStream = caseStreams.FirstOrDefault(c => c.CompleteDate == null);
        if (incompleteCaseStream != null)
        {
          EscalateCaseStream(incompleteCaseStream.CaseStreamId, (long)General.Person.System,
            (int)Framework.Enumerators.Stream.Comment.EscalatedBySystem);
        }
      }

      if (commitChanges)
      {
        uow.CommitChanges();
      }
    }

    public void BreakOutstandingPtCs(
      Framework.Enumerators.Stream.GroupType group = Framework.Enumerators.Stream.GroupType.Sales)
    {
      using (var uow = new UnitOfWork())
      {
        // break PTC's older than today, limiting query to 6 days ago
        var ptcCaseStreams =
          new XPQuery<STR_CaseStreamAction>(uow).Where(
            c =>
              c.CompleteDate == null && c.CaseStream.Stream.StreamId == Framework.Enumerators.Stream.StreamType.PTC.ToInt() &&
              c.ActionDate <= DateTime.Now).ToList();
        foreach (var ptcCaseStream in ptcCaseStreams)
        {
          try
          {
            MoveCaseToStream(caseStreamId: ptcCaseStream.CaseStream.CaseStreamId, personId: (long)General.Person.System,
              newStream: Framework.Enumerators.Stream.StreamType.PTCBroken,
              completedCommentId: (int)Framework.Enumerators.Stream.Comment.PTCBrokenBySystem,
              completeNote: string.Empty,
              makeDefaultAction: true, uow: null, newCaseStatus: CaseStatus.Type.InProgress);

            Escalate3ConsecutiveBrokenPtc(caseId: ptcCaseStream.CaseStream.Case.CaseId, uow: uow);

            ptcCaseStream.IsSuccess = false;

            uow.CommitChanges();

          }
          catch (Exception exception)
          {
            _logger.Error(string.Format("Error Breaking outstanding PTC's: {0} - {1}", exception.Message,
              exception.StackTrace));
          }
        }
      }
    }

    public void PtcClientNotInterested(long caseStreamId, string userId, string completeNote)
    {
      var person = _userService.Get(userId);
      if (person == null)
        throw new Exception(string.Format("UserId {0} does not exist", userId));

      // check if client said notInterested 3 times
      using (var uow = new UnitOfWork())
      { 
        var currentCaseStream = new XPQuery<STR_CaseStream>(uow).FirstOrDefault(c => c.CaseStreamId == caseStreamId);
        if (currentCaseStream == null)
          throw new Exception(string.Format("Case Stream with Id {0} does not exist", caseStreamId));
        var pastCaseStreams =
          new XPQuery<STR_CaseStream>(uow).Where(
            c => c.Case.CaseId == currentCaseStream.Case.CaseId && c.CompleteDate.HasValue)
            .OrderByDescending(c => c.CreateDate)
            .ToList();

        var notInterestedCounts = 0;
        foreach (var tmpCaseStream in pastCaseStreams)
        {
          if (tmpCaseStream.Stream.StreamId == Framework.Enumerators.Stream.StreamType.FollowUp.ToInt() &&
              tmpCaseStream.CompleteComment.CommentId ==
              (int)Framework.Enumerators.Stream.Comment.PTCClientNotInterested)
            notInterestedCounts++;
          else
            break;
        }

        currentCaseStream.NotInterestedCount = notInterestedCounts;

        // save not interested Count to case stream
        uow.CommitChanges();

        DateTime dateToFollowUpAgain;
        switch (notInterestedCounts)
        {
          case 0:
            // follow up case in 14 days
            dateToFollowUpAgain = DateTime.Today.AddDays(14);
            break;
          case 1:
            // follow up case in 21 days
            dateToFollowUpAgain = DateTime.Today.AddDays(21);
            break;
          case 2:
            // follow up case in 30 days
            dateToFollowUpAgain = DateTime.Today.AddDays(30);
            break;
          default:
            dateToFollowUpAgain = DateTime.Today.AddMonths(6);
            break;
        }
        var caseStream = MoveCaseToStream(caseStreamId: caseStreamId, personId: person.PersonId,
          newStream: Framework.Enumerators.Stream.StreamType.FollowUp,
          completedCommentId: (int)Framework.Enumerators.Stream.Comment.PTCClientNotInterested,
          completeNote: completeNote,
          makeDefaultAction: false, uow: null, newCaseStatus: CaseStatus.Type.InProgress);
        if (caseStream != null)
          AddCaseStreamAction(caseStream.CaseStreamId, dateToFollowUpAgain,
            Action.Type.Reminder, false, true);

        //else 
        //{
        //  // complete Case and dont bring it up again
        //  CompleteCase(caseStreamId: caseStreamId, personId: person.PersonId,
        //    completedCommentId: Framework.Enumerators.Stream.Comment.PTCClientNotInterested.ToInt(),
        //    completeNote: completeNote);
        //}
      }
    }

    public void MarkActionReminderAsComplete(long caseStreamActionId, string userId, int commentId)
    {
      var person = _userService.Get(userId);
      if (person == null)
        throw new Exception(string.Format("UserId {0} does not exist", userId));

      using (var uow = new UnitOfWork())
      {
        var caseStreamAction =
          new XPQuery<STR_CaseStreamAction>(uow).FirstOrDefault(c => c.CaseStreamActionId == caseStreamActionId);
        if (caseStreamAction == null)
          throw new Exception(string.Format("Case Stream Action with Id {0} does not exist", caseStreamActionId));

        caseStreamAction.CompleteDate = DateTime.Now;

        uow.CommitChanges();
      }
    }

    public void MovePtcCaseToComplete(long caseStreamId, string userId, int commentId)
    {
      var person = _userService.Get(userId);
      if (person == null)
        throw new Exception(string.Format("UserId {0} does not exist", userId));
      using (var uow = new UnitOfWork())
      {
        var caseStream = new XPQuery<STR_CaseStream>(uow).FirstOrDefault(c => c.CaseStreamId == caseStreamId);
        if (caseStream == null)
          throw new Exception(string.Format("Case Stream with Id: {0} not found in DB", caseStreamId));
        if (caseStream.Stream.StreamId != Framework.Enumerators.Stream.StreamType.PTC.ToInt())
          throw new Exception(string.Format("Case Stream with Id: {0} is no in PTC stream", caseStreamId));
        var caseStreamAction =
          new XPQuery<STR_CaseStreamAction>(uow).Where(
            c => c.CaseStream.CaseStreamId == caseStream.CaseStreamId && c.CompleteDate == null)
            .OrderByDescending(c => c.ActionDate)
            .FirstOrDefault();
        if (caseStreamAction != null)
        {
          caseStreamAction.IsSuccess = true;
        }

        uow.CommitChanges();
      }
      CompleteCase(caseStreamId: caseStreamId, personId: person.PersonId, completedCommentId: commentId,
        completeNote: "PTC Completed");
    }

    public void NoActionOnCaseStream(long caseStreamId, long caseStreamActionId, string userId, int commentId,
      DateTime? actionDate = null)
    {
      var person = _userService.Get(userId);
      if (person == null)
        throw new Exception(string.Format("UserId {0} does not exist", userId));
      if (!actionDate.HasValue)
        actionDate = DateTime.Now.AddMinutes(30); // needs to be moved to redis&appconfig
      var caseStreamAction = IncreaseNoActionCount(caseStreamId: caseStreamId,
        completedCaseStreamActionId: caseStreamActionId, personId: person.PersonId);
      AddCaseStreamAction(caseStreamId, actionDate.Value, Action.Type.Reminder, true, true, null,
        caseStreamAction.Amount);
    }

    public void AddCaseStreamActionWithPriority(long caseStreamId, string userId, DateTime actionDate,
      Action.Type actionType, bool allowMultipleActionTypes = true,
      bool completePendingActions = true, Framework.Enumerators.Stream.PriorityType? prirotyType = null)
    {
      var person = _userService.Get(userId);
      if (person == null)
        throw new Exception(string.Format("UserId {0} does not exist", userId));
      AddCaseStreamActionWithPriority(caseStreamId: caseStreamId, personId: person.PersonId, actionDate: actionDate,
        actionType: actionType, allowMultipleActionTypes: allowMultipleActionTypes,
        completePendingActions: completePendingActions, prirotyType: prirotyType);
    }

    public void AddCaseStreamAction(long caseStreamId, DateTime actionDate,
      Action.Type actionType, bool allowMultipleActionTypes = true,
      bool completePendingActions = true)
    {
      AddCaseStreamAction(caseStreamId, actionDate, actionType, allowMultipleActionTypes, completePendingActions, null);
    }

    public void IncreaseCaseStreamPriority(long caseStreamId, string userId,
      Framework.Enumerators.Stream.PriorityType? priorityType = null)
    {
      var person = _userService.Get(userId);
      if (person == null)
        throw new Exception(string.Format("UserId {0} does not exist", userId));
      SetCaseStreamPriority(caseStreamId: caseStreamId, personId: person.PersonId, uow: null, increasePrioty: true,
        priorityType: priorityType, escalate: false);
    }

    public void DecreaseCaseStreamPriority(long caseStreamId, string userId,
      Framework.Enumerators.Stream.PriorityType? priorityType = null)
    {
      var person = _userService.Get(userId);
      if (person == null)
        throw new Exception(string.Format("UserId {0} does not exist", userId));
      SetCaseStreamPriority(caseStreamId: caseStreamId, personId: person.PersonId, uow: null, increasePrioty: false,
        priorityType: priorityType, escalate: false);
    }

    public INote AddAccountNote(string userId, string note,
      long caseId,
      Framework.Enumerators.Stream.AccountNoteType accountNoteType = Framework.Enumerators.Stream.AccountNoteType.Normal
      )
    {
      var person = _userService.Get(userId);
      if (person == null)
        throw new Exception(string.Format("UserId {0} does not exist", userId));
      var savedNote = AddAccountNote(personId: person.PersonId, note: note, accountNoteType: accountNoteType, uow: null,
        caseId: caseId);

      INote noteDto = new Notes
      {
        NoteId = savedNote.NoteId,
        Note = savedNote.Note,
        CreateDate = savedNote.CreateDate
      };

      return noteDto;
    }

    public ICollection<long> GetAccountsReference2ByCaseStreamId(long caseStreamId)
    {
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        uow.TrackPropertiesModifications = false;
        return
          new XPQuery<STR_CaseStream>(uow).Where(c => c.CaseStreamId == caseStreamId).ToList()
            .SelectMany(c => c.Case.Debtor.Accounts)
            .Select(a => a.Reference2)
            .ToList();
      }
    }

    public Dictionary<string, long> GetCurrentAccountReferences(Framework.Enumerators.Stream.GroupType groupType)
    {
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        uow.TrackPropertiesModifications = false;
        var caseStatusIds = new[]
        {
          CaseStatus.Type.New,
          CaseStatus.Type.InProgress,
          CaseStatus.Type.OnHold
        }.Select(c => c.ToInt());

        var accountsAlreadyInSystemList = (from c in uow.Query<STR_Case>()
          join a in uow.Query<STR_Account>() on c.Debtor equals a.Debtor
          where caseStatusIds.Contains(c.CaseStatus.CaseStatusId)
                && c.Group.GroupId == groupType.ToInt()
                && c.Reference != null
          select new
          {
            a.Reference,
            a.Reference2
          }).Distinct().ToList();

        var accountsAlreadyInSystem = new Dictionary<string, long>();
        foreach (var accountAlreadyInSystem in accountsAlreadyInSystemList)
        {
          if (!string.IsNullOrWhiteSpace(accountAlreadyInSystem.Reference))
          {
            if (!accountsAlreadyInSystem.ContainsKey(accountAlreadyInSystem.Reference))
            {
              accountsAlreadyInSystem.Add(accountAlreadyInSystem.Reference, accountAlreadyInSystem.Reference2);
            }
          }
        }

        //var accountsAlreadyInSystem =
        //  new XPQuery<STR_Case>(uow).Where(
        //    c => caseStatusIds .Contains(c.CaseStatus.CaseStatusId) && c.Group.GroupId == groupType.ToInt()).ToList()
        //    .SelectMany(c => c.Debtor.Accounts.Select(a => new {a.Reference, a.Reference2})).Distinct()
        //    .ToDictionary(k => k.Reference, v => v.Reference2);

        if (groupType == Framework.Enumerators.Stream.GroupType.Sales)
        {
          var ptcClientNotInterestedCommentId = (int)Framework.Enumerators.Stream.Comment.PTCClientNotInterested;
          var completedStreamId = Framework.Enumerators.Stream.StreamType.Completed.ToInt();
          var caseStreamQuery = new XPQuery<STR_CaseStream>(uow).Where(c => c.Stream.StreamId == completedStreamId
                                                                            &&
                                                                            c.CompleteComment.CommentId ==
                                                                            ptcClientNotInterestedCommentId
                                                                            && c.CompleteDate.HasValue
                                                                            &&
                                                                            c.CompleteDate.Value >=
                                                                            DateTime.Today.AddDays(90)
            // && (DateTime.Today.Date - c.CompleteDate.Value.Date).TotalDays <= 90
            ).ToList().SelectMany(a => a.Case.Debtor.Accounts);

          foreach (var strAccount in caseStreamQuery.ToList())
          {
            accountsAlreadyInSystem.Add(strAccount.Reference, strAccount.Reference2);
          }
        }

        return accountsAlreadyInSystem;
      }
    }

    public ICollection<IAccountLastTransactionImportReference> GetCurrentAccountReferencesAndLastImportReference(
      Framework.Enumerators.Stream.GroupType groupType)
    {
      var accountReferences = new List<IAccountLastTransactionImportReference>();
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        uow.TrackPropertiesModifications = false;
        var caseStatusIds = new[]
        {
          CaseStatus.Type.InProgress,
          CaseStatus.Type.New
        }.Select(a => (int)a).ToList();

        var temp = (from cse in new XPQuery<STR_Case>(uow)
                    join a in new XPQuery<STR_Account>(uow) on cse.Debtor equals a.Debtor
                    where cse.Group.GroupId == groupType.ToInt() && caseStatusIds.Contains(cse.CaseStatus.CaseStatusId)
                    select new AccountLastTransactionImportReference
                    {
                      CaseId = cse.CaseId,
                      Reference = a.Reference,
                      Reference2 = a.Reference2,
                      AccountId = a.AccountId,
                      LastImportReference = a.LastImportReference
                    }).ToList();

        accountReferences.AddRange(temp);
      }
      return accountReferences.Distinct().ToList();
    }

    public Tuple<List<INote>, string> GetNoteHistory(long caseId)
    {
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        uow.TrackPropertiesModifications = false;
        var previousCaseStream =
          new XPQuery<STR_CaseStream>(uow).Where(c => c.Case.CaseId == caseId && c.CompleteDate.HasValue)
            .OrderByDescending(c => c.CreateDate)
            .FirstOrDefault();
        var completeNote = string.Empty;
        if (previousCaseStream != null)
          completeNote = string.Format("{0}: {1}", previousCaseStream.CompleteComment.Description,
            previousCaseStream.CompleteNote == null
              ? string.Empty
              : previousCaseStream.CompleteNote.Note);

        var notes =
          new XPQuery<STR_Note>(uow).Where(n => n.Case.CaseId == caseId)
            .OrderByDescending(n => n.CreateDate)
            .ToList();

        var notesDto = notes.Select(note => new Notes
        {
          NoteId = note.NoteId,
          Note = note.Note,
          CreateDate = note.CreateDate
        }).Cast<INote>().ToList();

        return new Tuple<List<INote>, string>(notesDto, completeNote);
      }
    }

    public void ChangeCaseStreamAllocatedUser(long caseStreamId, string userId, long currentUserId, long newUserId)
    {
      var person = _userService.Get(userId);
      if (person == null)
        throw new Exception(string.Format("UserId {0} does not exist", userId));

      using (var uow = new UnitOfWork())
      {
        var caseStream = new XPQuery<STR_CaseStream>(uow).FirstOrDefault(c => c.CaseStreamId == caseStreamId);
        if (caseStream == null)
          throw new Exception(string.Format("Case stream with Id {0} does not exist for this case", caseStreamId));

        //var person = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == personTmp.PersonId);
        //if (person == null || person.Security == null)
        //  throw new Exception(string.Format("Person with Id {0} does not exist", personTmp.PersonId));

        var newUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == newUserId);
        if (newUser == null || newUser.Security == null)
          throw new Exception(string.Format("Person with Id {0} does not exist", newUserId));

        if (caseStream.CaseStreamAllocations.Any(a => a.AllocatedUser.PersonId == newUserId && !a.TransferredOut))
          throw new Exception(string.Format("Person with Id {0} is already allocated to Case Stream with Id {1}",
            newUserId, caseStreamId));

        var currentCaseStreamAllocation =
          caseStream.CaseStreamAllocations.FirstOrDefault(
            a => !a.TransferredOut && a.AllocatedUser.PersonId == currentUserId);
        if (currentCaseStreamAllocation == null)
          throw new Exception(string.Format("Case Stream with Id {0} does not have any allocated User",
            caseStream.CaseStreamId));

        if (currentCaseStreamAllocation.Escalation != null)
        {
          // does new allocated user have proper role?
          switch (currentCaseStreamAllocation.Escalation.EscalationType)
          {
            case Framework.Enumerators.Stream.EscalationType.None:
              break;
            case Framework.Enumerators.Stream.EscalationType.BranchManager:
              if (newUser.GetRoles.All(r => r.RoleType.Type != General.RoleType.Branch_Manager))
                throw new Exception(string.Format("User with Id {0} does not have role of {1}", newUser.PersonId,
                  General.RoleType.Branch_Manager.ToStringEnum()));
              break;
            case Framework.Enumerators.Stream.EscalationType.AdminManager:
              if (newUser.GetRoles.All(r => r.RoleType.Type != General.RoleType.Admin_Manager))
                throw new Exception(string.Format("User with Id {0} does not have role of {1}", newUser.PersonId,
                  General.RoleType.Admin_Manager.ToStringEnum()));
              break;
            case Framework.Enumerators.Stream.EscalationType.OperationExecutive:
              if (newUser.GetRoles.All(r => r.RoleType.Type != General.RoleType.Operation_Executive))
                throw new Exception(string.Format("User with Id {0} does not have role of {1}", newUser.PersonId,
                  General.RoleType.Operation_Executive.ToStringEnum()));
              break;
            case Framework.Enumerators.Stream.EscalationType.RegionManager:
              if (newUser.GetRoles.All(r => r.RoleType.Type != General.RoleType.Regional_Manager))
                throw new Exception(string.Format("User with Id {0} does not have role of {1}", newUser.PersonId,
                  General.RoleType.Regional_Manager.ToStringEnum()));
              break;
            case Framework.Enumerators.Stream.EscalationType.Director:
              if (newUser.GetRoles.All(r => r.RoleType.Type != General.RoleType.Director))
                throw new Exception(string.Format("User with Id {0} does not have role of {1}", newUser.PersonId,
                  General.RoleType.Director.ToStringEnum()));
              break;
          }
        }

        currentCaseStreamAllocation.TransferredOut = true;
        currentCaseStreamAllocation.TransferredOutDate = DateTime.Now;
        new STR_CaseStreamAllocation(uow)
        {
          AllocatedDate = DateTime.Now,
          AllocatedUser = newUser,
          CaseStream = caseStream,
          Escalation = currentCaseStreamAllocation.Escalation,
          NoActionCount = 0,
          TransferredIn = true,
          TransferredOut = false
        };

        AddAccountNote(personId: person.PersonId,
          note:
            string.Format("[{0}] changed allocation of case, from [{1} {2}] to [{3} {4}]", person.FullName,
              currentCaseStreamAllocation.AllocatedUser.Firstname, currentCaseStreamAllocation.AllocatedUser.Lastname,
              newUser.Firstname, newUser.Lastname),
          accountNoteType: Framework.Enumerators.Stream.AccountNoteType.Normal, uow: uow, caseId: caseStream.Case.CaseId);

        uow.CommitChanges();
      }
    }

    // breakIfPendingActionsExist will only be looked at if cancelPendingActions is set to false.
    // if breakIfPendingActionsExist == false and there are pending actions, method will just return, otherwise an exception will be thrown
    public void MoveCaseToStream(long caseStreamId, long personId, Framework.Enumerators.Stream.StreamType newStream,
      int completedCommentId, string completeNote, bool makeDefaultAction,
      CaseStatus.Type? newCaseStatus = null)
    {
      if (newStream == Framework.Enumerators.Stream.StreamType.Completed)
      {
        CompleteCase(caseStreamId, personId, completedCommentId, completeNote,
          newCaseStatus ?? CaseStatus.Type.Closed);
      }
      else
      {
        MoveCaseToStream(caseStreamId: caseStreamId, personId: personId, newStream: newStream,
          completedCommentId: completedCommentId, completeNote: completeNote,
          makeDefaultAction: makeDefaultAction, uow: null,
          newCaseStatus: newCaseStatus);
      }
    }

    public void CloseCasesWithoutArrears(General.Host host, Framework.Enumerators.Stream.GroupType groupType)
    {
      using (var uow = new UnitOfWork())
      {
        var caseStatusIds = new[]
        {
          CaseStatus.Type.New.ToInt(),
          CaseStatus.Type.InProgress.ToInt(),
          CaseStatus.Type.OnHold.ToInt()
        };

        var caseStreams =
          new XPQuery<STR_CaseStream>(uow).Where(
            c => c.Case.Group.GroupId == groupType.ToInt() && caseStatusIds.Contains(c.Case.CaseStatus.CaseStatusId) &&
                 c.Case.Debtor.Accounts.Any(a => !string.IsNullOrEmpty(a.LastImportReference)) &&
                 c.Case.TotalArrearsAmount <= 0 &&
                 c.CompleteDate == null && c.Case.Host.HostId == host.ToInt()).ToList();
        foreach (var caseStream in caseStreams)
        {
          CompleteCase(caseStreamId: caseStream.CaseStreamId, personId: General.Person.System.ToInt(),
            completedCommentId: Framework.Enumerators.Stream.Comment.UpToDate.ToInt(),
            completeNote: "Closing Account without arrears");

          AddAccountNote(personId: (long)General.Person.System,
            note: string.Format("Account is not in arrears on {0}", host.ToStringEnum()),
            accountNoteType: Framework.Enumerators.Stream.AccountNoteType.Normal,
            uow: uow, caseId: caseStream.Case.CaseId);
        }
        uow.CommitChanges();
      }
    }

    public void CheckAndCloseCaseWithoutArrears(long caseId)
    {
      using (var uow = new UnitOfWork())
      {
        var caseStatusIds = new[]
        {
          CaseStatus.Type.New.ToInt(),
          CaseStatus.Type.InProgress.ToInt(),
          CaseStatus.Type.OnHold.ToInt()
        };

        var caseStreams =
          new XPQuery<STR_CaseStream>(uow).Where(
            c => c.Case.CaseId == caseId 
            && c.Case.Group.GroupId == Framework.Enumerators.Stream.GroupType.Collections.ToInt()
            && caseStatusIds.Contains(c.Case.CaseStatus.CaseStatusId) 
            && c.Case.Debtor.Accounts.Any(a => !string.IsNullOrEmpty(a.LastImportReference)) 
            && c.Case.TotalArrearsAmount <= 0 
            && c.CompleteDate == null).ToList();
        foreach (var caseStream in caseStreams)
        {
          CompleteCase(caseStreamId: caseStream.CaseStreamId, personId: General.Person.System.ToInt(),
            completedCommentId: Framework.Enumerators.Stream.Comment.UpToDate.ToInt(),
            completeNote: "Closed Case without arrears");

          AddAccountNote(personId: (long)General.Person.System,
            note: string.Format("Account is not in arrears on {0}", caseStream.Case.Host.Description),
            accountNoteType: Framework.Enumerators.Stream.AccountNoteType.Normal,
            uow: uow, caseId: caseStream.Case.CaseId);
        }
        uow.CommitChanges();
      }
    }

    public void ForceCloseCase(long caseStreamId, int commentId)
    {
      using (var uow = new UnitOfWork())
      {
        var caseStream = new XPQuery<STR_CaseStream>(uow).FirstOrDefault(c => c.CaseStreamId == caseStreamId);
        if (caseStream != null)
        {
          CloseCase(caseStream.Case.CaseId);
        }
      }
    }

    public void CloseCase(long caseId, int? commentId = null)
    {
      using (var uow = new UnitOfWork())
      {
        var caseStatusIds = new[]
        {
          CaseStatus.Type.New.ToInt(),
          CaseStatus.Type.InProgress.ToInt(),
          CaseStatus.Type.OnHold.ToInt()
        };

        var caseStreams =
          new XPQuery<STR_CaseStream>(uow).Where(
            c => c.Case.CaseId == caseId && caseStatusIds.Contains(c.Case.CaseStatus.CaseStatusId) &&
                 c.Case.Debtor.Accounts.Any(a => !string.IsNullOrEmpty(a.LastImportReference)) &&
                 c.Case.TotalArrearsAmount <= 0 &&
                 c.CompleteDate == null).ToList();
        foreach (var caseStream in caseStreams)
        {
          CompleteCase(caseStreamId: caseStream.CaseStreamId, personId: General.Person.System.ToInt(),
            completedCommentId: commentId ?? Framework.Enumerators.Stream.Comment.UpToDate.ToInt(),
            completeNote: "Closed Case by request");

          AddAccountNote(personId: (long)General.Person.System, note: "Case closed on request",
            accountNoteType: Framework.Enumerators.Stream.AccountNoteType.Normal,
            uow: uow, caseId: caseStream.Case.CaseId);
        }
        uow.CommitChanges();
      }
    }

    public void ProcessPtpResults(Dictionary<long, Tuple<long, bool?>> ptpResults)
    {
      foreach (var ptpResult in ptpResults)
      {
        // is the PTP completed?
        if (ptpResult.Value.Item2.HasValue)
          CompletePtp(caseStreamActionId: ptpResult.Key, success: ptpResult.Value.Item2.Value,
            personId: (long)General.Person.System);
        else
          // PTP not yet completed - Add Reminder + increase priority
          AddCaseStreamActionWithPriority(caseStreamId: ptpResult.Value.Item1, personId: (long)General.Person.System,
            actionDate: DateTime.Today.AddDays(1).AddHours(8), actionType: Action.Type.Reminder,
            allowMultipleActionTypes: true, completePendingActions: false,
            prirotyType: Framework.Enumerators.Stream.PriorityType.High);
      }
    }

    public void AddContact(long debtorId, string userId, General.ContactType contactType, string value)
    {
      using (var uow = new UnitOfWork())
      {
        if (string.IsNullOrWhiteSpace(value))
          throw new Exception("Value is blank");

        var personTmp = _userService.Get(userId);
        if (personTmp == null)
          throw new Exception(string.Format("UserId {0} does not exist", userId));

        var debtor = new XPQuery<STR_Debtor>(uow).FirstOrDefault(d => d.DebtorId == debtorId);
        if (debtor == null)
          throw new Exception(string.Format("Debtor with Id {0} does not exist", debtorId));

        var person = new XPQuery<PER_Person>(uow).FirstOrDefault(d => d.PersonId == personTmp.PersonId);
        if (person == null)
          throw new Exception(string.Format("Person with Id {0} does not exist", userId));

        new STR_DebtorContact(uow)
        {
          CreateDate = DateTime.Now,
          Debtor = debtor,
          CreateUser = person,
          ContactType = new XPQuery<ContactType>(uow).FirstOrDefault(c => c.ContactTypeId == contactType.ToInt()),
          IsActive = true,
          Value = value
        };

        uow.CommitChanges();
      }
    }

    public bool EscalateCaseStream(long caseStreamId, string userId, int commentId,
      params Framework.Enumerators.Stream.EscalationType[] escalationTypes)
    {
      var person = _userService.Get(userId);
      if (person == null)
        throw new Exception(string.Format("UserId {0} does not exist", userId));
      foreach (var escalationType in escalationTypes)
      {
        EscalateCaseStream(caseStreamId: caseStreamId, personId: person.PersonId, commentId: commentId, uow: null,
          escalateToSpecific: escalationType);
      }

      return true;
    }

    public void RemoveDeceasedClients(List<long> debtorReferences)
    {
      using (var uow = new UnitOfWork())
      {
        var caseStatusIds = new[] { (int)CaseStatus.Type.New, (int)CaseStatus.Type.InProgress };
        var caseStreams =
          new XPQuery<STR_CaseStream>(uow).Where(
            a =>
              debtorReferences.Contains(a.Case.Debtor.Reference) &&
              caseStatusIds.Contains(a.Case.CaseStatus.CaseStatusId) && !a.CompleteDate.HasValue).ToList();
        foreach (var caseStream in caseStreams)
        {
          CompleteCase(caseStreamId: caseStream.CaseStreamId, personId: General.Person.System.ToInt(),
            completedCommentId: Framework.Enumerators.Stream.StreamType.Completed.ToInt(),
            completeNote: "Removed Deceased client");
        }
      }
    }

    #endregion


    #region private Methods

    private STR_CaseStreamAction IncreaseNoActionCount(long caseStreamId, long completedCaseStreamActionId,
      long? personId = null, UnitOfWork uow = null)
    {
      var commitChanges = uow == null;
      if (commitChanges)
        uow = new UnitOfWork();

      var caseStream = new XPQuery<STR_CaseStream>(uow).FirstOrDefault(c => c.CaseStreamId == caseStreamId);
      if (caseStream == null)
        throw new Exception(string.Format("Case Stream with Id {0} does not exist", caseStreamId));
      STR_CaseStreamAllocation allocation;
      if (personId.HasValue)
      {
        allocation =
          caseStream.CaseStreamAllocations.FirstOrDefault(a => a.AllocatedUser.PersonId == personId && !a.TransferredOut);
        if (allocation == null)
          throw new Exception(string.Format("UserId {0} is not allocated to Case Stream Id {1}", personId, caseStreamId));
        allocation.NoActionCount++;
      }
      else
      {
        allocation =
          caseStream.CaseStreamAllocations.FirstOrDefault(
            a => a.Escalation.EscalationType == Framework.Enumerators.Stream.EscalationType.None && !a.TransferredOut);
        if (allocation == null)
          throw new Exception(string.Format("Case Stream Id {0} does not a primary user", caseStreamId));
        allocation.NoActionCount++;
      }

      var caseStreamAction =
        new XPQuery<STR_CaseStreamAction>(uow).FirstOrDefault(c => c.CaseStreamActionId == completedCaseStreamActionId);
      if (caseStreamAction == null)
        throw new Exception(string.Format("Case Stream Action with Id {0} does not exist", completedCaseStreamActionId));
      caseStreamAction.CompleteDate = DateTime.Now;

      if (allocation.NoActionCount % 3 == 0 && allocation.NoActionCount / 3 > 1)
        SetCaseStreamPriority(caseStreamId: caseStreamId, personId: (long)General.Person.System);

      AddAccountNote(personId: (long)General.Person.System,
        note:
          string.Format("No Action performed by {0} {1}[{2}]", allocation.AllocatedUser.Firstname,
            allocation.AllocatedUser.Lastname, allocation.Escalation.EscalationType.ToStringEnum()),
        accountNoteType: Framework.Enumerators.Stream.AccountNoteType.Normal,
        uow: uow, caseId: caseStream.Case.CaseId);

      if (commitChanges)
      {
        uow.CommitChanges();
      }

      return caseStreamAction;
    }

    private void ChangeCaseStatus(long caseId, long personId, int caseStatusId, UnitOfWork uow = null)
    {
      var commitChanges = uow == null;
      if (commitChanges)
        uow = new UnitOfWork();

      var streamCase = new XPQuery<STR_Case>(uow).FirstOrDefault(a => a.CaseId == caseId);
      if (streamCase == null)
        throw new Exception(string.Format("Case with Id {0} does not exist", caseId));

      var casestatus = new XPQuery<STR_CaseStatus>(uow).FirstOrDefault(c => c.CaseStatusId == caseStatusId);
      if (casestatus == null)
        throw new Exception(string.Format("Status with Id {0} does not exist", caseStatusId));

      var person = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == personId);
      if (person == null || person.Security == null)
        throw new Exception(string.Format("Person with Id {0} does not exist", personId));

      AddAccountNote(personId,
        string.Format("[{0} {1}] changed case status from {2} to {3}", person.Firstname, person.Lastname,
          streamCase.CaseStatus.Description, casestatus.Description),
        Framework.Enumerators.Stream.AccountNoteType.Normal,
        uow, caseId);
      streamCase.CaseStatus = casestatus;
      streamCase.LastStatusDate = DateTime.Now;
      if (casestatus.Status == CaseStatus.Type.Closed && !streamCase.WorkableCase.HasValue)
      {
        streamCase.WorkableCase = false;
      }

      if (commitChanges)
      {
        uow.CommitChanges();
      }
    }

    private void CompleteCase(long caseStreamId, long personId, int completedCommentId, string completeNote, CaseStatus.Type newCaseStatus = CaseStatus.Type.Closed)
    {
      using (var uow = new UnitOfWork())
      {
        var caseStream = new XPQuery<STR_CaseStream>(uow).FirstOrDefault(c => c.CaseStreamId == caseStreamId);
        if (caseStream == null)
          throw new Exception(string.Format("Case stream with Id {0} does not exist", caseStreamId));

        var person = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == personId);
        if (person == null)
          throw new Exception(string.Format("Person with Id {0} does not exist", personId));

        var stream =
          new XPQuery<STR_Stream>(uow).FirstOrDefault(
            s => s.StreamId == Framework.Enumerators.Stream.StreamType.Completed.ToInt());

        if (stream == null)
          return;

        // check if actions for the stream is completed
        var pendingActions =
          new XPQuery<STR_CaseStreamAction>(uow).Where(
            a => a.CaseStream.CaseStreamId == caseStream.CaseStreamId && a.CompleteDate == null).ToList();
        pendingActions.ForEach(p =>
        {
          p.CompleteDate = DateTime.Now;
        });

        caseStream.CompletedUser = person;
        caseStream.CompleteDate = DateTime.Now;
        caseStream.CompleteComment = new XPQuery<STR_Comment>(uow).FirstOrDefault(c => c.CommentId == completedCommentId);

        if (!string.IsNullOrWhiteSpace(completeNote))
        {
          var accountNote = AddAccountNote(
            personId: (long)General.Person.System,
            note: completeNote,
            accountNoteType: Framework.Enumerators.Stream.AccountNoteType.Normal,
            uow: uow, caseId: caseStream.Case.CaseId);
          caseStream.CompleteNote = accountNote;
        }

        var newCaseStream = new STR_CaseStream(uow)
        {
          Case = caseStream.Case,
          CreateDate = DateTime.Now,
          CreateUser = person,
          Escalation =
            new XPQuery<STR_Escalation>(uow).FirstOrDefault(
              e => e.EscalationType == Framework.Enumerators.Stream.EscalationType.None),
          LastPriorityDate = DateTime.Now,
          Priority =
            stream.DefaultCaseStreamPriority ??
            new XPQuery<STR_Priority>(uow).FirstOrDefault(
              p => p.PriorityType == Framework.Enumerators.Stream.PriorityType.Normal),
          Stream = stream,
          CompleteComment =
            new XPQuery<STR_Comment>(uow).FirstOrDefault(
              c => c.CommentId == (int)Framework.Enumerators.Stream.Comment.Complete),
          CompleteDate = DateTime.Now
        };
        ChangeCaseStatus(caseId: newCaseStream.Case.CaseId, personId: personId,
          caseStatusId: (int)newCaseStatus, uow: uow);

        var newAllocatedUser =
          caseStream.CaseStreamAllocations.FirstOrDefault(
            a => a.Escalation.EscalationType == Framework.Enumerators.Stream.EscalationType.None && !a.TransferredOut) ??
          caseStream.CaseStreamAllocations.FirstOrDefault(
            a => a.Escalation.EscalationType == Framework.Enumerators.Stream.EscalationType.None);

        new STR_CaseStreamAllocation(uow)
        {
          AllocatedDate = DateTime.Now,
          AllocatedUser =
            newAllocatedUser == null
              ? new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == (int)General.Person.System)
              : newAllocatedUser.AllocatedUser,
          CaseStream = newCaseStream,
          Escalation = newCaseStream.Escalation,
          NoActionCount = 0,
          TransferredIn = false,
          TransferredOut = false
        };

        new STR_CaseStreamEscalation(uow)
        {
          CaseStream = newCaseStream,
          Escalation = newCaseStream.Escalation,
          CreateDate = DateTime.Now
        };

        if (newCaseStream.Stream.StreamId != caseStream.Stream.StreamId)
        {
          if (caseStream.Case.CaseStatus.Status != CaseStatus.Type.Closed)
            ChangeCaseStatus(caseId: caseStream.Case.CaseId, personId: personId,
              caseStatusId: (int)CaseStatus.Type.InProgress, uow: uow);

          AddAccountNote(personId: (long)General.Person.System,
            note:
              string.Format("Case Moved from Stream {0} to {1}: {2}", caseStream.Stream.Description, stream.Description,
                caseStream.CompleteComment == null ? string.Empty : caseStream.CompleteComment.Description),
            accountNoteType: Framework.Enumerators.Stream.AccountNoteType.Normal,
            uow: uow, caseId: caseStream.Case.CaseId);
        }
        uow.CommitChanges();
      }
    }

    private STR_CaseStream MoveCaseToStream(long caseStreamId, long personId,
      Framework.Enumerators.Stream.StreamType newStream, int completedCommentId, string completeNote, bool makeDefaultAction, UnitOfWork uow,
      CaseStatus.Type? newCaseStatus = null)
    {
      if (newStream == Framework.Enumerators.Stream.StreamType.Completed)
      {
        var stackTrace = new System.Diagnostics.StackTrace();
        _logger.Fatal(string.Format("Still trying to close stream from old method: {0}", stackTrace.ToString()));
      }

      var commitChanges = uow == null;
      if (commitChanges)
        uow = new UnitOfWork();

      var caseStream = new XPQuery<STR_CaseStream>(uow).FirstOrDefault(c => c.CaseStreamId == caseStreamId);
      if (caseStream == null)
        throw new Exception(string.Format("Case stream with Id {0} does not exist", caseStreamId));
      if (caseStream.CompleteDate.HasValue)
        return null;
      //throw new Exception(string.Format("Case Stream with Id {0} already completed", caseStreamId));

      var person = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == personId);
      if (person == null)
        throw new Exception(string.Format("Person with Id {0} does not exist", personId));

      // check if actions for the stream is completed
      var pendingActions =
        new XPQuery<STR_CaseStreamAction>(uow).Where(
          a => a.CaseStream.CaseStreamId == caseStream.CaseStreamId && a.CompleteDate == null).ToList();

      pendingActions.ForEach(p =>
      {
        p.CompleteDate = DateTime.Now;
      });

      caseStream.CompletedUser = person;
      caseStream.CompleteDate = DateTime.Now;
      caseStream.CompleteComment = new XPQuery<STR_Comment>(uow).FirstOrDefault(c => c.CommentId == completedCommentId);

      if (!string.IsNullOrWhiteSpace(completeNote))
      {
        var accountNote = AddAccountNote(
          personId: (long)General.Person.System,
          note: completeNote,
          accountNoteType: Framework.Enumerators.Stream.AccountNoteType.Normal,
          uow: uow, caseId: caseStream.Case.CaseId);
        caseStream.CompleteNote = accountNote;
      }

      var stream = new XPQuery<STR_Stream>(uow).FirstOrDefault(s => s.StreamId == newStream.ToInt());

      if (stream != null)
      {
        var newCaseStream = new STR_CaseStream(uow)
        {
          Case = caseStream.Case,
          CreateDate = DateTime.Now,
          CreateUser = person,
          Escalation =
            new XPQuery<STR_Escalation>(uow).FirstOrDefault(
              e => e.EscalationType == Framework.Enumerators.Stream.EscalationType.None),
          LastPriorityDate = DateTime.Now,
          Priority =
            stream.DefaultCaseStreamPriority ??
            new XPQuery<STR_Priority>(uow).FirstOrDefault(
              p => p.PriorityType == Framework.Enumerators.Stream.PriorityType.Normal),
          Stream = stream
        };

        var newAllocatedUser =
          caseStream.CaseStreamAllocations.FirstOrDefault(
            a => a.Escalation.EscalationType == Framework.Enumerators.Stream.EscalationType.None && !a.TransferredOut) ??
          caseStream.CaseStreamAllocations.FirstOrDefault(
            a => a.Escalation.EscalationType == Framework.Enumerators.Stream.EscalationType.None);

        new STR_CaseStreamAllocation(uow)
        {
          AllocatedDate = DateTime.Now,
          AllocatedUser =
            newAllocatedUser == null
              ? new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == (int)General.Person.System)
              : newAllocatedUser.AllocatedUser,
          CaseStream = newCaseStream,
          Escalation = newCaseStream.Escalation,
          NoActionCount = 0,
          TransferredIn = false,
          TransferredOut = false
        };

        new STR_CaseStreamEscalation(uow)
        {
          CaseStream = newCaseStream,
          Escalation = newCaseStream.Escalation,
          CreateDate = DateTime.Now
        };

        if (newStream != Framework.Enumerators.Stream.StreamType.Completed && makeDefaultAction)
        {
          new STR_CaseStreamAction(uow)
          {
            ActionDate = DateTime.Today.AddDays(1).AddHours(8),
            CaseStream = newCaseStream,
            ActionType =
              new XPQuery<STR_ActionType>(uow).FirstOrDefault(p => p.Type == Action.Type.Normal)
          };
          if (newCaseStatus.HasValue)
            ChangeCaseStatus(caseId: newCaseStream.Case.CaseId, personId: personId, caseStatusId: (int)newCaseStatus,
              uow: uow);
        }
        else if (newStream == Framework.Enumerators.Stream.StreamType.Completed)
        {
          newCaseStream.CompleteComment =
            new XPQuery<STR_Comment>(uow).FirstOrDefault(
              c => c.CommentId == (int)Framework.Enumerators.Stream.Comment.Complete);
          newCaseStream.CompleteDate = DateTime.Now;
          ChangeCaseStatus(caseId: newCaseStream.Case.CaseId, personId: personId,
            caseStatusId: (int)(newCaseStatus ?? CaseStatus.Type.Closed), uow: uow);
        }
        else
        {
          if (newCaseStatus.HasValue)
            ChangeCaseStatus(caseId: newCaseStream.Case.CaseId, personId: personId, caseStatusId: (int)newCaseStatus,
              uow: uow);
        }

        if (newCaseStream.Stream.StreamId != caseStream.Stream.StreamId)
        {
          if (newCaseStatus.HasValue && caseStream.Case.CaseStatus.Status != newCaseStatus.Value)
            ChangeCaseStatus(caseId: caseStream.Case.CaseId, personId: personId,
              caseStatusId: (int)CaseStatus.Type.InProgress, uow: uow);

          AddAccountNote(personId: (long)General.Person.System,
            note:
              string.Format("Case Moved from Stream {0} to {1}: {2}", caseStream.Stream.Description, stream.Description,
                caseStream.CompleteComment == null ? string.Empty : caseStream.CompleteComment.Description),
            accountNoteType: Framework.Enumerators.Stream.AccountNoteType.Normal,
            uow: uow, caseId: caseStream.Case.CaseId);
        }

        if (commitChanges)
        {
          uow.CommitChanges();
        }

        return newCaseStream;
      }
      return null;
    }

    public void CompletePtp(long caseStreamActionId, bool success, long personId)
    {
      using (var uow = new UnitOfWork())
      {
        var ptpTotalBrokenPtPs = 0;
        STR_CaseStream caseStream = null;
        var ptp = new XPQuery<STR_CaseStreamAction>(uow).FirstOrDefault(p => p.CaseStreamActionId == caseStreamActionId);
        if (ptp == null)
          throw new Exception("PTP does not exist");

        if (ptp.CompleteDate.HasValue)
          throw new Exception("PTP is already completed");

        ptp.CompleteDate = DateTime.Now;
        ptp.IsSuccess = success;

        if (success)
        {
          CompleteCase(ptp.CaseStream.CaseStreamId, personId: personId,
            completedCommentId: Framework.Enumerators.Stream.Comment.PTPSuccessful.ToInt(), completeNote: "Complete PTP");
          SendSms(caseStreamId: ptp.CaseStream.CaseStreamId, personId: personId,
            smsTemplate: Atlas.Enumerators.Notification.NotificationTemplate.Stream_SMS_PaymentThanks);
        }
        else
        {
          const int ptpBrokenId = (int)Framework.Enumerators.Stream.StreamType.PTPBroken;
          ptpTotalBrokenPtPs =
            new XPQuery<STR_CaseStream>(uow).Count(
              c => c.Case.CaseId == ptp.CaseStream.Case.CaseId && c.Stream.StreamId == ptpBrokenId);
          caseStream = MoveCaseToStream(caseStreamId: ptp.CaseStream.CaseStreamId, personId: personId,
            newStream: Framework.Enumerators.Stream.StreamType.PTPBroken,
            completedCommentId: (int)Framework.Enumerators.Stream.Comment.PTPBroken, completeNote: string.Empty,
            makeDefaultAction: true, uow: uow, newCaseStatus: CaseStatus.Type.InProgress);
          ptpTotalBrokenPtPs++;
          SendSms(caseStreamId: ptp.CaseStream.CaseStreamId, personId: personId,
            smsTemplate: Atlas.Enumerators.Notification.NotificationTemplate.Stream_SMS_PTPBroken);
          uow.CommitChanges();
        }


        if (ptpTotalBrokenPtPs > 0 && ptpTotalBrokenPtPs % 3 == 0 && caseStream != null)
          EscalateCaseStream(caseStreamId: caseStream.CaseStreamId, personId: personId,
            commentId: (int)Framework.Enumerators.Stream.Comment.PTPBrokenMoreThanThreeTimes, uow: uow);
        uow.CommitChanges();
      }
    }

    private void SendSms(long caseStreamId, long personId,
      Atlas.Enumerators.Notification.NotificationTemplate smsTemplate, DateTime? actionDate = null,
      decimal? amount = null)
    {
      var blockedSmsTemplates = new[]
      {
        Atlas.Enumerators.Notification.NotificationTemplate.Stream_SMS_Initiate,
        Atlas.Enumerators.Notification.NotificationTemplate.Stream_SMS_PaymentThanks
      };

      if (blockedSmsTemplates.Contains(smsTemplate))
        return;

      using (var uow = new UnitOfWork())
      {
        var caseStream = new XPQuery<STR_CaseStream>(uow).FirstOrDefault(a => a.CaseStreamId == caseStreamId);
        if (caseStream == null)
          throw new Exception(string.Format("Case Stream with Id {0} does not exist", caseStreamId));

        var user = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == personId);
        if (user == null)
          throw new Exception(string.Format("Person with Id {0} does not exist", personId));

        var cellContact =
          caseStream.Case.Debtor.Contacts.Where(c => c.ContactType.ContactTypeId == General.ContactType.CellNo.ToInt())
            .OrderBy(c => c.CreateDate)
            .FirstOrDefault();
        if (cellContact != null)
        {
          var template = new XPQuery<NTF_Template>(uow).FirstOrDefault(t => t.TemplateType.Type == smsTemplate);
          if (template == null)
          {
            AddAccountNote(personId: user.PersonId,
              note:
                string.Format("SMS Sending Failed by [{0} {1}]: SMS template {2} does not exist", user.Firstname,
                  user.Lastname, smsTemplate.ToStringEnum()),
              accountNoteType: Framework.Enumerators.Stream.AccountNoteType.Normal, uow: uow,
              caseId: caseStream.Case.CaseId);
          }
          else
          {
            var templateKeys = new Dictionary<string, string>
            {
              {
                "[CLIENT_NAME]", string.Format("{0} {1} {2}", caseStream.Case.Debtor.Title,
                  caseStream.Case.Debtor.FirstName, caseStream.Case.Debtor.LastName)
              }
            };
            var contact = caseStream.Case.Branch.Company.GetContacts.FirstOrDefault();
            if (contact != null)
              templateKeys.Add("[BRANCH_TEL]", contact.Contact.Value);

            var branchManager =
              new XPQuery<PER_Person>(uow).Where(
                p => p.Security.IsActive
                     && p.Branch.BranchId == caseStream.Case.Branch.BranchId)
                .ToList()
                .FirstOrDefault(p => p.GetRoles.Any(a => a.RoleType.Type == General.RoleType.Branch_Manager));

            templateKeys.Add("[BRANCH_MANAGER_NAME]",
              branchManager == null
                ? "Branch Manager"
                : string.Format("{0} {1}", branchManager.Firstname, branchManager.Lastname));
            if (actionDate.HasValue)
              templateKeys.Add("[ACTION_DATE]", actionDate.Value.ToString("dd MMM yyyy"));
            if (amount.HasValue)
              templateKeys.Add("[AMOUNT]", amount.Value.ToString("##.##"));

            var smsMessage = FillInTheBlanks(template.Template, templateKeys);

            //_smsService.Send(cellContact.Contact.Value, smsMessage, Atlas.Enumerators.Notification.NotificationPriority.High);
            AddAccountNote(personId: personId,
              note:
                string.Format("SMS Sent to {0} by [{1} {2}]: '{3}'", cellContact.Value, user.Firstname,
                  user.Lastname, smsMessage),
              accountNoteType: Framework.Enumerators.Stream.AccountNoteType.Action, uow: uow,
              caseId: caseStream.Case.CaseId);

            var caseStreamAllocation =
              caseStream.CaseStreamAllocations.FirstOrDefault(a => a.AllocatedUser.PersonId == personId);
            if (caseStreamAllocation != null)
              caseStreamAllocation.SMSCount++;

            caseStream.Case.SMSCount++;
          }
        }

        uow.CommitChanges();
      }
    }

    private string FillInTheBlanks(string originalText, Dictionary<string, string> options)
    {
      return options.Aggregate(originalText, (current, option) => current.Replace(option.Key, option.Value));
    }

    private bool AddCaseStreamAction(long caseStreamId, DateTime actionDate,
      Action.Type actionType,
      bool allowMultipleActionTypes = true, bool completePendingActions = false, UnitOfWork uow = null,
      decimal? amount = null)
    {
      var actionAdded = false;
      var commitChanges = uow == null;
      if (commitChanges)
        uow = new UnitOfWork();

      var caseStream = new XPQuery<STR_CaseStream>(uow).FirstOrDefault(s => s.CaseStreamId == caseStreamId);
      if (caseStream == null)
        throw new Exception(string.Format("Case stream with Id {0} does not exist", caseStreamId));
      if (caseStream.CompleteDate != null)
        throw new Exception(string.Format("Case stream with Id {0} is not current", caseStreamId));

      if (!allowMultipleActionTypes)
      {
        var caseStreamActions =
          new XPQuery<STR_CaseStreamAction>(uow).Where(
            c =>
              c.CaseStream.CaseStreamId == caseStream.CaseStreamId && c.CompleteDate == null &&
              c.ActionType.ActionTypeId == actionType.ToInt()).ToList();
        if (caseStreamActions.Any())
        {
          if (completePendingActions)
          {
            caseStreamActions.ForEach(csa =>
            {
              csa.CompleteDate = DateTime.Now;
            });
          }
          else
          {
            throw new Exception(string.Format(
              "There are existing actions of the same type for case stream with Id {0}", caseStreamId));
          }
        }
      }

      // make sure there's not a reminder at that exact time and date
      var duplicateCaseStreamActions =
        new XPQuery<STR_CaseStreamAction>(uow).Any(c => c.CaseStream.CaseStreamId == caseStream.CaseStreamId && c.CompleteDate == null &&
            c.ActionType.ActionTypeId == actionType.ToInt() &&
            c.ActionDate == actionDate && (amount.HasValue ? c.Amount == amount : c.Amount == null));
      if (!duplicateCaseStreamActions)
      {
        new STR_CaseStreamAction(uow)
        {
          Amount = amount,
          ActionDate = actionDate,
          CaseStream = caseStream,
          ActionType = new XPQuery<STR_ActionType>(uow).FirstOrDefault(p => p.ActionTypeId == actionType.ToInt())
        };
        actionAdded = true;
      }

      if (commitChanges)
      {
        uow.CommitChanges();
      }
      return actionAdded;
    }

    private STR_Note AddAccountNote(long personId, string note,
      Framework.Enumerators.Stream.AccountNoteType accountNoteType, UnitOfWork uow, long caseId)
    {
      var commitChanges = uow == null;
      if (commitChanges)
        uow = new UnitOfWork();

      var createUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == personId);
      if (createUser == null)
        throw new Exception(string.Format("Person {0} does not exist", personId));

      var accountCase = new XPQuery<STR_Case>(uow).FirstOrDefault(a => a.CaseId == caseId);
      var accountNote = new STR_Note(uow)
      {
        AccountNoteType =
          new XPQuery<STR_AccountNoteType>(uow).FirstOrDefault(a => a.AccountNoteType == accountNoteType),
        Case = accountCase,
        CreateDate = DateTime.Now,
        CreateUser = createUser,
        Note = note
      };

      if (commitChanges)
      {
        uow.CommitChanges();
      }
      return accountNote;
    }

    private void SetCaseStreamPriority(long caseStreamId, long personId, UnitOfWork uow = null,
      bool increasePrioty = true, Framework.Enumerators.Stream.PriorityType? priorityType = null, bool escalate = true)
    {
      var commitChanges = uow == null;
      if (commitChanges)
        uow = new UnitOfWork();

      var caseStream = new XPQuery<STR_CaseStream>(uow).FirstOrDefault(s => s.CaseStreamId == caseStreamId);
      if (caseStream == null)
        throw new Exception(string.Format("Case stream with Id {0} does not exist", caseStreamId));
      if (caseStream.CompleteDate != null)
        throw new Exception(string.Format("Case stream with Id {0} is not current", caseStreamId));
      var person = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == personId);
      if (person == null)
        throw new Exception(string.Format("Person {0} does not exist", personId));

      if (priorityType == null)
      {
        STR_Priority newPriority;
        if (increasePrioty)
          newPriority =
            new XPQuery<STR_Priority>(uow).Where(p => p.Value > caseStream.Priority.Value)
              .OrderBy(p => p.Value)
              .FirstOrDefault();
        else
          newPriority =
            new XPQuery<STR_Priority>(uow).Where(p => p.Value < caseStream.Priority.Value)
              .OrderByDescending(p => p.Value)
              .FirstOrDefault();

        if (escalate)
          EscalateCaseStream(caseStreamId, personId, (int)Framework.Enumerators.Stream.Comment.PriorityIncrease, uow);

        if (newPriority != null)
        {
          AddAccountNote(personId: personId,
            note:
              string.Format("Priority changed from {0} to {1} by [{2} {3}]", caseStream.Priority.Description,
                newPriority.Description, person.Firstname, person.Lastname),
            accountNoteType: Framework.Enumerators.Stream.AccountNoteType.Normal, uow: uow,
            caseId: caseStream.Case.CaseId);
          caseStream.Priority = newPriority;
          caseStream.LastPriorityDate = DateTime.Now;
        }
        else
        {
          AddAccountNote(personId: personId,
            note: "Priority of case is at MAX", accountNoteType: Framework.Enumerators.Stream.AccountNoteType.Normal,
            uow: uow, caseId: caseStream.Case.CaseId);
          //if (escalateIfMaxed)
          //EscalateCaseStream(caseStreamId, userId, uow);
        }
      }
      else
      {
        if (caseStream.Priority.PriorityType != priorityType)
        {
          var newPriority = new XPQuery<STR_Priority>(uow).FirstOrDefault(p => p.PriorityType == priorityType);
          if (newPriority == null)
            throw new Exception(string.Format("Priority {0} not found in database", priorityType.Value.ToStringEnum()));
          AddAccountNote(personId: personId,
            note:
              string.Format("Priority changed from {0} to {1} by [{2} {3}]", caseStream.Priority.Description,
                newPriority.Description, person.Firstname, person.Lastname),
            accountNoteType: Framework.Enumerators.Stream.AccountNoteType.Normal, uow: uow,
            caseId: caseStream.Case.CaseId);
          caseStream.Priority = newPriority;
          caseStream.LastPriorityDate = DateTime.Now;
        }
      }

      if (commitChanges)
      {
        uow.CommitChanges();
      }
    }

    private void EscalateCaseStream(long caseStreamId, long personId, int commentId, UnitOfWork uow = null,
      Framework.Enumerators.Stream.EscalationType? escalateToSpecific = null)
    {
      var commitChanges = uow == null;
      if (commitChanges)
        uow = new UnitOfWork();

      var caseStream = new XPQuery<STR_CaseStream>(uow).FirstOrDefault(s => s.CaseStreamId == caseStreamId);
      if (caseStream == null)
        throw new Exception(string.Format("Case stream with Id {0} does not exist", caseStreamId));
      if (caseStream.CompleteDate != null)
        throw new Exception(string.Format("Case stream with Id {0} is not current", caseStreamId));

      var person = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == personId);
      if (person == null)
        throw new Exception(string.Format("Person {0} does not exist", personId));

      STR_Escalation newEscalation;
      if (escalateToSpecific.HasValue)
        newEscalation =
          new XPQuery<STR_Escalation>(uow).Where(e => e.EscalationType == escalateToSpecific)
            .OrderBy(e => e.Value)
            .FirstOrDefault();
      else
        newEscalation =
          new XPQuery<STR_Escalation>(uow).Where(e => e.Value > caseStream.Escalation.Value)
            .OrderBy(e => e.Value)
            .FirstOrDefault();
      if (newEscalation != null)
      {
        if (newEscalation.Value > caseStream.Escalation.Value)
        {
          var caseStreamAllocation =
            caseStream.CaseStreamAllocations.FirstOrDefault(
              a => a.Escalation.EscalationType == Framework.Enumerators.Stream.EscalationType.None);
          if (caseStreamAllocation != null)
          {
            var userBranch = caseStreamAllocation.AllocatedUser.Branch;
            // get higher user
            List<PER_Person> allocatedUsers;
            if (newEscalation.EscalationType == Framework.Enumerators.Stream.EscalationType.BranchManager &&
                userBranch != null)
            {
              allocatedUsers =
                new XPQuery<PER_Person>(uow).Where(
                  p => p.Security.IsActive 
                       && p.Branch.BranchId == userBranch.BranchId).ToList().Where(p=>p.GetRoles.Any(a => a.RoleType.Type == General.RoleType.Branch_Manager)).ToList();
            }
            else if (newEscalation.EscalationType == Framework.Enumerators.Stream.EscalationType.AdminManager &&
                     userBranch != null)
            {
              allocatedUsers =
                new XPQuery<PER_Person>(uow).Where(
                  p => p.Security.IsActive).ToList().Where(p=> p.GetRoles.Any(a => a.RoleType.Type == General.RoleType.Admin_Manager)
                       && p.GetBranches.Any(b => b.Branch.BranchId == userBranch.BranchId)).ToList();
            }
            else if (newEscalation.EscalationType == Framework.Enumerators.Stream.EscalationType.RegionManager &&
                     userBranch != null)
            {
              allocatedUsers =
                new XPQuery<PER_Person>(uow).Where(
                  p => p.Security.IsActive).ToList().Where(p=> p.GetRoles.Any(a => a.RoleType.Type == General.RoleType.Regional_Manager)
                       && p.GetRegions.Any(r => r.Region.RegionId == userBranch.Region.RegionId)).ToList();
            }
            else
              switch (newEscalation.EscalationType)
              {
                case Framework.Enumerators.Stream.EscalationType.OperationExecutive:
                  allocatedUsers =
                    new XPQuery<PER_Person>(uow).Where(
                      p =>
                        p.Security.IsActive).ToList().Where(p=> p.GetRoles.Any(a => a.RoleType.Type == General.RoleType.Operation_Executive)
                        && p.GetRegions.Any(r => userBranch != null && r.Region.RegionId == userBranch.Region.RegionId)).ToList();
                  break;
                case Framework.Enumerators.Stream.EscalationType.Director:
                  allocatedUsers =
                    new XPQuery<PER_Person>(uow).Where(
                      p => p.Security.IsActive).ToList().Where(p=> p.GetRoles.Any(a => a.RoleType.Type == General.RoleType.Director))
                      .ToList();
                  break;
                default:
                  allocatedUsers = new List<PER_Person>();
                  break;
              }

            if (allocatedUsers.Count > 0)
            {
              foreach (var allocatedUser in allocatedUsers)
              {
                new STR_CaseStreamAllocation(uow)
                {
                  AllocatedDate = DateTime.Now,
                  AllocatedUser = allocatedUser,
                  CaseStream = caseStream,
                  Escalation = newEscalation,
                  NoActionCount = 0,
                  TransferredIn = false,
                  TransferredOut = false
                };
              }
            }
            else
            {
              if (userBranch != null)
                AddAccountNote(personId: personId,
                  note:
                    string.Format("There are no users allocated to this escalation level {0} for branch {1}",
                      newEscalation.Description, userBranch.Company.Name),
                  accountNoteType: Framework.Enumerators.Stream.AccountNoteType.Normal, uow: uow,
                  caseId: caseStream.Case.CaseId);
            }
          }

          var comment = new XPQuery<STR_Comment>(uow).FirstOrDefault(c => c.CommentId == commentId);

          AddAccountNote(personId: personId,
            note:
              string.Format("Escalated from {0} to {1} by [{2} {3}] | Reason: {4}", caseStream.Escalation.Description,
                newEscalation.Description, person.Firstname, person.Lastname,
                comment == null ? "Unknown" : comment.Description),
            accountNoteType: Framework.Enumerators.Stream.AccountNoteType.Normal, uow: uow,
            caseId: caseStream.Case.CaseId);

          caseStream.Escalation = newEscalation;
          new STR_CaseStreamEscalation(uow)
          {
            CaseStream = caseStream,
            CreateDate = DateTime.Now,
            Escalation = newEscalation
          };
        }
        else
        {
          AddAccountNote(personId: personId,
            note:
              string.Format(
                "Cannot Escalate Case: [{0} - {1}] tried Escalating to a lower escalation ({2}) than current ({3}).",
                person.Firstname, person.Lastname, newEscalation.Description, caseStream.Escalation.Description),
            accountNoteType: Framework.Enumerators.Stream.AccountNoteType.Normal, uow: uow,
            caseId: caseStream.Case.CaseId);
        }
      }
      else
      {
        AddAccountNote(personId: personId,
          note: "Escalation of case is at MAX", accountNoteType: Framework.Enumerators.Stream.AccountNoteType.Normal,
          uow: uow, caseId: caseStream.Case.CaseId);
      }

      if (commitChanges)
      {
        uow.CommitChanges();
      }
    }

    public void AddCaseStreamActionWithPriority(long caseStreamId, long personId, DateTime actionDate,
      Action.Type actionType, bool allowMultipleActionTypes = true,
      bool completePendingActions = true, Framework.Enumerators.Stream.PriorityType? prirotyType = null)
    {
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        var actionAdded = AddCaseStreamAction(caseStreamId, actionDate, actionType, allowMultipleActionTypes,
          completePendingActions, uow);
        if (!actionAdded) return;
        SetCaseStreamPriority(caseStreamId: caseStreamId, personId: personId, uow: uow, increasePrioty: true,
          priorityType: prirotyType, escalate: false);
        uow.CommitChanges();
      }
    }

    // srinivas - refactor - 3rd
    private List<IActuator> FlattenActuatorsPerBranch(Framework.Enumerators.Stream.ActuatorType actuatorType)
    {
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        uow.TrackPropertiesModifications = false;
        var actuators = new XPQuery<STR_Actuator>(uow).Where(a => (a.RangeStart.Date >= DateTime.Today.AddMonths(-1)
                                                                   || a.RangeEnd.Date >= DateTime.Today.AddMonths(-1)) &&
                                                                  a.IsActive && a.ActuatorType.Type == actuatorType)
          .ToList();

        var groupActuators = actuators.Where(a => a.Branch == null && a.Region == null).ToList();
        var regionActuators = actuators.Where(a => a.Branch == null && a.Region != null).ToList();
        var branchActuators = actuators.Where(a => a.Branch != null).ToList();

        var branchIds = branchActuators.Select(a => a.Branch.BranchId).Distinct().ToList();

        foreach (var branchId in branchIds)
        {
          var branchActuatorsB = branchActuators.Where(a => a.Branch.BranchId == branchId).ToList();
          foreach (var branchActuator in branchActuatorsB)
          {
            foreach (var groupActuator in groupActuators)
            {
              var merged = false;
              if (branchActuator.RangeStart.Date >= groupActuator.RangeStart.Date &&
                  branchActuator.RangeStart.Date <= groupActuator.RangeEnd.Date)
              {
                branchActuator.RangeStart = groupActuator.RangeStart;
                merged = true;
              }
              if (branchActuator.RangeEnd.Date >= groupActuator.RangeStart.Date &&
                  branchActuator.RangeEnd.Date <= groupActuator.RangeEnd.Date)
              {
                branchActuator.RangeEnd = groupActuator.RangeEnd;
                merged = true;
              }

              if (!merged)
              {
                branchActuators.Add(groupActuator);
              }
            }
          }
          branchActuatorsB = branchActuators.Where(a => a.Branch.BranchId == branchId).ToList();
          foreach (var branchActuator in branchActuatorsB)
          {
            var actuator = branchActuator;
            foreach (
              var regionActuator in regionActuators.Where(r => r.Region.RegionId == actuator.Region.RegionId))
            {
              var merged = false;
              if (branchActuator.RangeStart.Date >= regionActuator.RangeStart.Date &&
                  branchActuator.RangeStart.Date <= regionActuator.RangeEnd.Date)
              {
                branchActuator.RangeStart = regionActuator.RangeStart;
                merged = true;
              }
              if (branchActuator.RangeEnd.Date >= regionActuator.RangeStart.Date &&
                  branchActuator.RangeEnd.Date <= regionActuator.RangeEnd.Date)
              {
                branchActuator.RangeEnd = regionActuator.RangeEnd;
                merged = true;
              }

              if (!merged)
              {
                branchActuators.Add(regionActuator);
              }
            }
          }
        }

        var branchActuatorsDto = new List<IActuator>();
        foreach (var b in branchActuators)
        {
          var bDto = new Actuator
          {
            Branch = new Branch(),
            Region = new Falcon.Common.Structures.Region(),
            ActuatorId = b.ActuatorId
          };
          bDto.Branch.BranchId = b.Branch.BranchId;
          bDto.CreateDate = b.CreateDate;
          bDto.DisableDate = b.DisableDate;
          bDto.IsActive = b.IsActive;
          bDto.Region.RegionId = b.Region.RegionId;
          bDto.RangeStart = b.RangeStart;
          bDto.RangeEnd = b.RangeEnd;
          branchActuatorsDto.Add(bDto);
        }
        return branchActuatorsDto;
      }
    }

    #endregion

  }
}