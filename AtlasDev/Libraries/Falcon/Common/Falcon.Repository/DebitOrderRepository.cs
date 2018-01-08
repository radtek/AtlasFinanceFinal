using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Common.Extensions;
using Atlas.Enumerators;
using Atlas.RabbitMQ.Messages.DebitOrder;
using AutoMapper;
using Falcon.Common.Structures.DebitOrder;
using Falcon.Common.Interfaces.Repositories;
using Falcon.Common.Interfaces.Structures;
using Magnum;
using MassTransit;

namespace Falcon.Common.Repository
{
  public class DebitOrderRepository : IDebitOrderRepository
  {
    private readonly IServiceBus _bus;
    private readonly IMappingEngine _mappingEngine;

    public DebitOrderRepository(IServiceBus bus, IConfiguration mappingConfiguration, IMappingEngine mappingEngine)
    {
      _bus = bus;
      _mappingEngine = mappingEngine;

      // Setup up mappers.
      mappingConfiguration.CreateMap<NaedoBatch, DebitOrderBatch>();
      mappingConfiguration.CreateMap<NaedoBatchTransaction, IDebitOrderTransaction>();
      mappingConfiguration.CreateMap<DebitOrderBatch, IDebitOrderBatch>();
      mappingConfiguration.CreateMap<DebitOrderControl, IDebitOrderControl>();
    }

    public IList<IDebitOrderBatch> GetBatches(long? batchId, long? branchId, Debit.BatchStatus? batchStatus, DateTime? startRange, DateTime? endRange, bool queryBatchOnly)
    {
      List<IDebitOrderBatch> batch = null;

      if (_bus == null) throw new NullReferenceException("Service bus cannot be null, possible injection problem");

      var queryDebitOrder = new QueryNaedoBatch(CombGuid.Generate())
      {
        BatchId = batchId,
        BatchOnly = queryBatchOnly,
        BatchStatus = batchStatus,
        EndRange = endRange,
        StartRange = startRange
      };

      _bus.PublishRequest(queryDebitOrder, x =>
      {
        x.Handle<ResponseNaedoBatch>(a =>
        {
          batch = _mappingEngine.Map<List<DebitOrderBatch>, List<IDebitOrderBatch>>(_mappingEngine.Map<List<NaedoBatch>, List<DebitOrderBatch>>(a.NaedoBatches));
        });
        x.SetTimeout(TimeSpan.FromSeconds(20));
      });

      return batch;
    }


    public IDebitOrderControl GetControl(long controlId, int? specificRepetition)
    {
      IDebitOrderControl response = new DebitOrderControl();

      var queryDebitOrder = new QueryDebitOrder(CombGuid.Generate())
      {
        ControlId = controlId,
        SpecifiedRepetition = specificRepetition
      };

      _bus.PublishRequest(queryDebitOrder, x =>
      {
        x.Handle<ResponseMessage>(a =>
        {
          response = CreateResponse(a.Response, a.HasError, a.ErrorMessage);
        }); x.SetTimeout(TimeSpan.FromSeconds(89));
      });

      return response;
    }

    public IList<IDebitOrderControl> GetControls(General.Host? host, long? branchId, DateTime? startRange, DateTime? endRange, bool controlOnly)
    {

      IList<IDebitOrderControl> responses = new List<IDebitOrderControl>();

      var queryDebitOrder = new MultiQueryDebitOrder(CombGuid.Generate())
      {
        Host = host,
        BranchId = branchId,
        StartRange = startRange,
        EndRange = endRange,
        ControlOnly = controlOnly
      };

      _bus.PublishRequest(queryDebitOrder, x =>
      {
        x.Handle<MultiResponseMessage>(a =>
        {
          responses = CreateMultiResponse(a.Responses);
        });
        x.SetTimeout(TimeSpan.FromSeconds(89));
      });

      return responses;
    }

    public IDebitOrderControl AddAdditionalDebitOrder(long controlId, decimal instalment, DateTime actionDate, DateTime instalmentDate)
    {
      IDebitOrderControl response = new DebitOrderControl();

      var addAdditionalDebitOrder = new AddAdditionalDebitOrder(CombGuid.Generate())
      {
        ControlId = controlId,
        Instalment = instalment,
        ActionDate = actionDate,
        InstalmentDate = instalmentDate
      };

      _bus.PublishRequest(addAdditionalDebitOrder, x =>
      {
        x.Handle<ResponseMessage>(a =>
        {
          response = CreateResponse(a.Response, a.HasError, a.ErrorMessage);
        }); x.SetTimeout(TimeSpan.FromSeconds(89));
      });

      return response;
    }

    public IDebitOrderControl CancelAdditionalDebitOrder(long controlId, long transactionId)
    {
      IDebitOrderControl response = new DebitOrderControl();

      var cancelAdditionalDebitOrder = new CancelAdditionalDebitOrder(CombGuid.Generate())
      {
        ControlId = controlId,
        TransactionId = transactionId
      };

      _bus.PublishRequest(cancelAdditionalDebitOrder, x =>
      {
        x.Handle<ResponseMessage>(a =>
        {
          response = CreateResponse(a.Response, a.HasError, a.ErrorMessage);
        }); x.SetTimeout(TimeSpan.FromSeconds(89));
      });

      return response;
    }

    #region Private Methods

    private IList<IDebitOrderControl> CreateMultiResponse(IEnumerable<ResponseControl> responseControls)
    {
      return responseControls.Select(responseControl => CreateResponse(responseControl, false, string.Empty)).ToList();
    }


    private IDebitOrderControl CreateResponse(ResponseControl responseControl, bool hasError, string errorMessage)
    {
      IDebitOrderControl response = new DebitOrderControl();

      if ((int)responseControl.AVSCheckType > 0)
      {
        response.AvsCheckType = responseControl.AVSCheckType;
        response.AvsCheckTypeDescription = responseControl.AVSCheckType.ToStringEnum();
      }

      response.Bank = responseControl.Bank;
      response.BankId = responseControl.BankId;
      response.BankBranchCode = responseControl.BankBranchCode;
      response.BankAccountNo = responseControl.BankAccountNo;
      response.BankAccountName = responseControl.BankAccountName;
      response.BankAccountTypeId = responseControl.BankAccountTypeId;
      response.BankAccountType = responseControl.BankAccountType;
      response.BankStatementReference = responseControl.BankStatementReference;
      response.ControlId = responseControl.ControlId;
      response.IdNumber = responseControl.IdNumber;
      if ((int)responseControl.ControlStatus > 0)
      {
        response.ControlStatus = responseControl.ControlStatus;
        response.ControlStatusDescription = responseControl.ControlStatus.ToStringEnum();
      }
      if ((int)responseControl.ControlType > 0)
      {
        response.ControlType = responseControl.ControlType;
        response.ControlTypeDescription = responseControl.ControlType.ToStringEnum();
      }
      response.CurrentRepetition = responseControl.CurrentRepetition;
      response.ErrorMessage = errorMessage;
      if ((int)responseControl.FailureType > 0)
      {
        response.FailureType = responseControl.FailureType;
        response.FailureTypeDescription = responseControl.FailureType.ToStringEnum();
      }
      response.FirstInstalmentDate = responseControl.FirstInstalmentDate;
      if ((int)responseControl.Frequency > 0)
      {
        response.Frequency = responseControl.Frequency;
        response.FrequencyDescription = responseControl.Frequency.ToStringEnum();
      }
      response.HasError = hasError;
      response.Instalment = responseControl.Instalment;
      response.LastInstalmentUpdate = responseControl.LastInstalmentUpdate;
      response.PayDateDayOfMonth = responseControl.PayDateDayOfMonth;
      response.PayDateDayOfWeek = responseControl.PayDateDayOfWeek;
      if ((int)responseControl.PayDateType > 0)
      {
        response.PayDateType = responseControl.PayDateType;
        response.PayDateTypeDescription = responseControl.PayDateType.ToStringEnum();
      }
      if ((int)responseControl.PayRule > 0)
      {
        response.PayRule = responseControl.PayRule;
        response.PayRuleDescription = responseControl.PayRule.ToStringEnum();
      }
      response.Repetitions = responseControl.Repetitions;
      response.ThirdPartyReference = responseControl.ThirdPartyReference;
      response.TrackingDays = responseControl.TrackingDay;
      response.TrackingDaysDescription = responseControl.TrackingDay.ToStringEnum();
      response.ValidationErrors = new List<Debit.ValidationType>();
      response.ValidationErrors.AddRange(responseControl.ValidationErrors);

      if (responseControl.ResponseTransactions != null && responseControl.ResponseTransactions.Count > 0)
        response.ResponseTransactions = new List<IDebitOrderTransaction>();

      if (responseControl.ResponseTransactions != null)
        foreach (var responseTransaction in responseControl.ResponseTransactions)
        {
          response.ResponseTransactions.Add(new DebitOrderResponseTransaction
          {
            ActionDate = responseTransaction.ActionDate,
            Amount = responseTransaction.Amount,
            CancelDate = responseTransaction.CancelDate,
            InstalmentDate = responseTransaction.InstalmentDate,
            OverrideActionDate = responseTransaction.OverrideActionDate,
            OverrideAmount = responseTransaction.OverrideAmount,
            OverrideTrackingDays = responseTransaction.OverrideTrackingDays,
            Repetition = responseTransaction.Repetition,
            ReplyCode = responseTransaction.ReplyCode,
            ReplyCodeDescription = responseTransaction.ReplyCodeDescription,
            ResponseCode = responseTransaction.ResponseCode,
            ResponseCodeDescription = responseTransaction.ResponseCodeDescription,
            Status = responseTransaction.Status,
            StatusDescription = responseTransaction.Status.ToStringEnum(),
            TransactionNo = responseTransaction.TransactionNo,
            ControlId = response.ControlId,
            Bank = response.Bank,
            BankAccountNo = response.BankAccountNo,
            BankAccountName = response.BankAccountName,
            BankStatementReference = response.BankStatementReference,
            IdNumber = response.IdNumber
          });
        }

      return response;
    }

    #endregion

  }
}
