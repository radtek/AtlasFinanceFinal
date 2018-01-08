using System;
using System.Collections.Generic;
using Atlas.Enumerators;
using Stream.Framework.Enumerators;

namespace Falcon.Gyrkin.Controllers.Api.Models
{
  public class StreamModel
  {

    public class GetStaticDataModel
    {
      public int StaticData { get; set; }
      public Stream.Framework.Enumerators.Stream.GroupType GroupType { get; set; }
    }

    public enum Outcome
    {
      Yes,
      No
    }

    public enum StreamAction
    {
      PTP,
      PTPReschedule,
      PTPUnchanged,
      PTC,
      PTCUnchanged,
      FollowUp,
      NoAction,
      ForceClose
    }

    public class SaveModel
    {
      public long CaseStreamId { get; set; }
      public long ActionId { get; set; }
      public string UserId { get; set; }
      //public long UserId { get; set; }
      public StreamAction StreamType { get; set; }
      public string Amount { get; set; }
      public DateTime Date { get; set; }
      public int Comment { get; set; }
      public string Note { get; set; }
    }

    public class GetWorkItemsQueryModel
    {
      public string AllocatedUserId { get; set; }
      public long? AllocatedPersonId { get; set; }
      public long? BranchId { get; set; }
      public string IdNumber { get; set; }
      public string AccountReferenceNo { get; set; }
      public long CaseId { get; set; }
      public DateTime? ActionStartDate { get; set; }
      public DateTime? ActionEndDate { get; set; }
      public DateTime? CreateStartDate { get; set; }
      public DateTime? CreateEndDate { get; set; }
      public Stream.Framework.Enumerators.Stream.StreamType StreamType { get; set; }
      public CaseStatus.Type? CaseStatus { get; set; }
      public Stream.Framework.Enumerators.Stream.GroupType GroupType { get; set; }
      public DateTime? CompleteDate { get; set; }
    }

    public class GetNextWorkItemModel
    {
      public string PersonId { get; set; }
      //public long PersonId { get; set; }
      public Stream.Framework.Enumerators.Stream.StreamType StreamType { get; set; }
      public Stream.Framework.Enumerators.Stream.GroupType GroupType { get; set; }
    }
    public class SyncPaymentModel
    {
      public long CaseStreamId { get; set; }
    }


    public class EscalateModel
    {
      public long CaseStreamId { get; set; }
      public Stream.Framework.Enumerators.Stream.EscalationType[] EscalationType { get; set; }
      public string UserId { get; set; }
      //public long UserId { get; set; }
      public int Comment { get; set; }
    }

    public class SaveContactModel
    {
      public long DebitorId { get; set; }
      public General.ContactType ContactType { get; set; }
      public string No { get; set; }
      public string UserId { get; set; }
      //public long UserId { get; set; }
      public int Comment { get; set; }
    }

    public class CaseCommentModel
    {
      public long CaseId { get; set; }
      public string UserId { get; set; }
      //public long UserId { get; set; }
      public string Note { get; set; }
    }

    public class CaseGetCommentsModel
    {
      public long CaseId { get; set; }
    }

    public class GetWorkItemModel
    {
      public long CaseStreamActionId { get; set; }
      public string UserId { get; set; }
    }

    public class GetNoteHistoryModel
    {
      public long CaseId { get; set; }
    }

    public class GetLetterOfDemandModel
    {
      public long CaseStreamId { get; set; }
    }

    public class SaveNotInterested
    {
      public long CaseStreamId { get; set; }
      public string UserId { get; set; }
      public string Note { get; set; }

    }
    public class OutcomeModel
    {
      public long CaseStreamId { get; set; }
      public string UserId { get; set; }
      public Outcome Outcome { get; set; }
      public int Comment { get; set; }
      public string Note { get; set; }
    }
    public class EscalatedItemsModel
    {
      public string AllocatedUserId { get; set; }
      public long BranchId { get; set; }
    }
    public class EscalatedTransferBackModel
    {
      public long CaseStreamId { get; set; }
      public string AllocatedUserId { get; set; }
      public int CommentId { get; set; }
    }

    public class ChangeAllocatedUserModel
    {
      public long CaseStreamId { get; set; }
      public string UserId { get; set; }
      public long CurrentUserId { get; set; }
      public long NewUserId { get; set; }
    }

    public class ChangeMultipleCaseStreamAllocatedUserModel
    {
      public ChangeMultipleCaseStreamAllocatedUserModel()
      {
        ChangeAllocatedUserModels =new List<ChangeAllocatedUserModel>();
      }
      public List<ChangeAllocatedUserModel> ChangeAllocatedUserModels { get; set; }
    }
  }
}