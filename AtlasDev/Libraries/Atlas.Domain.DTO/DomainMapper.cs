using Atlas.Domain.DTO;
using Atlas.Domain.DTO.Nucard;
using Atlas.Domain.Model;
using Atlas.Domain.Model.Biometric;
using Atlas.Domain.Security;


namespace Atlas.Domain
{
  public static class DomainMapper
  {
    public static void Map()
    {
      //// Core mappings for different model types      
      AutoMapper.Mapper.CreateMap<BRN_Branch, BRN_BranchDTO>();
      AutoMapper.Mapper.CreateMap<BRN_Config, BRN_ConfigDTO>();
      AutoMapper.Mapper.CreateMap<Contact, ContactDTO>();
      AutoMapper.Mapper.CreateMap<CPY_Company, CPY_CompanyDTO>()
        .ForMember(d => d.Addresses, opt => opt.Ignore())
        .ForMember(d => d.BankDetails, opt => opt.Ignore())
        .ForMember(d => d.Contacts, opt => opt.Ignore())
        .ForMember(d => d.Branches, opt => opt.Ignore())
        .ForMember(d => d.Persons, opt => opt.Ignore());
      AutoMapper.Mapper.CreateMap<Region, RegionDTO>();
      AutoMapper.Mapper.CreateMap<ContactType, ContactTypeDTO>();
      AutoMapper.Mapper.CreateMap<Province, ProvinceDTO>();
      AutoMapper.Mapper.CreateMap<RoleType, RoleTypeDTO>();
      AutoMapper.Mapper.CreateMap<TransactionSource, TransactionSourceDTO>();
      AutoMapper.Mapper.CreateMap<TCCTerminal, TCCTerminalDTO>();
      AutoMapper.Mapper.CreateMap<PublicHoliday, PublicHolidayDTO>();
      AutoMapper.Mapper.CreateMap<BUR_Service, BUR_ServiceDTO>();
      AutoMapper.Mapper.CreateMap<BUR_Enquiry, BUR_EnquiryDTO>();
      AutoMapper.Mapper.CreateMap<BUR_Storage, BUR_StorageDTO>();
      AutoMapper.Mapper.CreateMap<NTE_Note, NTE_NoteDTO>();

      #region Security
      AutoMapper.Mapper.CreateMap<COR_Machine, COR_MachineDTO>();
      AutoMapper.Mapper.CreateMap<COR_Software, COR_SoftwareDTO>();
      AutoMapper.Mapper.CreateMap<COR_AppUsage, COR_AppUsageDTO>();
      #endregion

      #region ASS

      //AutoMapper.Mapper.CreateMap<ASS_CiReport, ASS_CiReportDTO>();
      //AutoMapper.Mapper.CreateMap<ASS_CiReportCompuscanProduct, ASS_CiReportCompuscanProductDTO>();
      //AutoMapper.Mapper.CreateMap<ASS_CiReportHandoverInfo, ASS_CiReportHandoverInfoDTO>();
      //AutoMapper.Mapper.CreateMap<ASS_CiReportPossibleHandover, ASS_CiReportPossibleHandoverDTO>();
      //AutoMapper.Mapper.CreateMap<ASS_CiReportScore, ASS_CiReportScoreDTO>();
      //AutoMapper.Mapper.CreateMap<ASS_CiReportVersion, ASS_CiReportVersionDTO>();
      AutoMapper.Mapper.CreateMap<ASS_BranchServer, ASS_BranchServerDTO>();
      AutoMapper.Mapper.CreateMap<ASS_DbUpdateScript, ASS_DbUpdateScriptDTO>();
      AutoMapper.Mapper.CreateMap<COR_AppUsage, COR_AppUsageDTO>();

      #endregion

      #region Bureau

      AutoMapper.Mapper.CreateMap<BUR_Accounts, BUR_AccountsDTO>();
      AutoMapper.Mapper.CreateMap<BUR_AccountStatusCode, BUR_AccountStatusCodeDTO>();
      AutoMapper.Mapper.CreateMap<BUR_AccountTypeCode, BUR_AccountTypeCodeDTO>();
      AutoMapper.Mapper.CreateMap<BUR_Batch, BUR_BatchDTO>();
      AutoMapper.Mapper.CreateMap<BUR_Band, BUR_BandDTO>();
      AutoMapper.Mapper.CreateMap<BUR_BandRange, BUR_BandRangeDTO>();

      #endregion

      #region General Ledger

      AutoMapper.Mapper.CreateMap<LGR_Fee, LGR_FeeDTO>();
      AutoMapper.Mapper.CreateMap<LGR_FeeRangeType, LGR_FeeRangeTypeDTO>();
      AutoMapper.Mapper.CreateMap<LGR_Transaction, LGR_TransactionDTO>();
      AutoMapper.Mapper.CreateMap<LGR_TransactionType, LGR_TransactionTypeDTO>();
      AutoMapper.Mapper.CreateMap<LGR_TransactionTypeGroup, LGR_TransactionTypeGroupDTO>();
      AutoMapper.Mapper.CreateMap<LGR_Type, LGR_TypeDTO>();

      #endregion

      #region AVS

      AutoMapper.Mapper.CreateMap<AVS_BankAccountPeriod, AVS_BankAccountPeriodDTO>();
      AutoMapper.Mapper.CreateMap<AVS_Batch, AVS_BatchDTO>();
      AutoMapper.Mapper.CreateMap<AVS_ResponseCode, AVS_ResponseCodeDTO>();
      AutoMapper.Mapper.CreateMap<AVS_ResponseGroup, AVS_ResponseGroupDTO>();
      AutoMapper.Mapper.CreateMap<AVS_ResponseResult, AVS_ResponseResultDTO>();
      AutoMapper.Mapper.CreateMap<AVS_Result, AVS_ResultDTO>();
      AutoMapper.Mapper.CreateMap<AVS_Service, AVS_ServiceDTO>();
      AutoMapper.Mapper.CreateMap<AVS_ServiceSchedule, AVS_ServiceScheduleDTO>();
      AutoMapper.Mapper.CreateMap<AVS_ServiceScheduleBank, AVS_ServiceScheduleBankDTO>();
      AutoMapper.Mapper.CreateMap<AVS_ServiceType, AVS_ServiceTypeDTO>();
      AutoMapper.Mapper.CreateMap<AVS_Status, AVS_StatusDTO>();
      AutoMapper.Mapper.CreateMap<AVS_Transaction, AVS_TransactionDTO>();

      #endregion

      #region Bank

      AutoMapper.Mapper.CreateMap<BNK_Detail, BankDetailDTO>();
      AutoMapper.Mapper.CreateMap<BNK_Bank, BankDTO>();
      AutoMapper.Mapper.CreateMap<BNK_Branch, BankBranchDTO>();
      AutoMapper.Mapper.CreateMap<BNK_CDV, BankCDVDTO>();
      AutoMapper.Mapper.CreateMap<BNK_AccountType, BankAccountTypeDTO>();

      #endregion

      #region Address

      AutoMapper.Mapper.CreateMap<ADR_Address, AddressDTO>();
      AutoMapper.Mapper.CreateMap<ADR_Type, AddressTypeDTO>();
      AutoMapper.Mapper.CreateMap<ADR_Type, AddressDTO>().ReverseMap();

      #endregion

      #region Person

      AutoMapper.Mapper.CreateMap<PER_Person, PER_PersonDTO>()
        .ForMember(d => d.AddressDetails, opt => opt.Ignore())
        .ForMember(d => d.BankDetails, opt => opt.Ignore())
        .ForMember(d => d.Contacts, opt => opt.Ignore())
        .ForMember(d => d.Cards, opt => opt.Ignore())
        .ForMember(d => d.Relations, opt => opt.Ignore())
        .ForMember(d => d.EmploymentHistory, opt => opt.Ignore()); 
      AutoMapper.Mapper.CreateMap<PER_Role, PER_RoleDTO>();
      AutoMapper.Mapper.CreateMap<PER_Security, PER_SecurityDTO>();
      AutoMapper.Mapper.CreateMap<PER_Relation, PER_RelationDTO>();
      AutoMapper.Mapper.CreateMap<PER_RelationType, PER_RelationTypeDTO>();
      AutoMapper.Mapper.CreateMap<PER_Type, PER_TypeDTO>();

      #endregion

      #region NuCard

      AutoMapper.Mapper.CreateMap<NUC_NuCard, NUC_NuCardDTO>();
      AutoMapper.Mapper.CreateMap<NUC_NuCardBatch, NUC_NuCardBatchDTO>();
      AutoMapper.Mapper.CreateMap<NUC_NuCardProcess, NUC_NuCardProcessDTO>();
      AutoMapper.Mapper.CreateMap<NUC_NuCardProfile, NUC_NuCardProfileDTO>();
      AutoMapper.Mapper.CreateMap<NUC_NuCardStatus, NUC_NuCardStatusDTO>();
      AutoMapper.Mapper.CreateMap<NUC_Transaction, NUC_TransactionDTO>();

      #endregion

      #region Product

      AutoMapper.Mapper.CreateMap<PRD_ProductBatch, ProductBatchDTO>();
      AutoMapper.Mapper.CreateMap<PRD_Product, ProductDTO>();
      AutoMapper.Mapper.CreateMap<PRD_ProductType, ProductTypeDTO>();

      #endregion

      #region NAEDO Report Import

      AutoMapper.Mapper.CreateMap<NAEDOReportBatch, NAEDOReportBatchDTO>();
      AutoMapper.Mapper.CreateMap<NAEDOReportCancelled, NAEDOReportCancelledDTO>();
      AutoMapper.Mapper.CreateMap<NAEDOReportCancelled.ReportCancelledKey, NAEDOReportCancelledDTO.ReportCancelledKey>();
      AutoMapper.Mapper.CreateMap<NAEDOReportDisputed, NAEDOReportDisputedDTO>();
      AutoMapper.Mapper.CreateMap<NAEDOReportDisputed.ReportDisputedKey, NAEDOReportDisputedDTO.ReportDisputedKey>();
      AutoMapper.Mapper.CreateMap<NAEDOReportFailed, NAEDOReportFailedDTO>();
      AutoMapper.Mapper.CreateMap<NAEDOReportFailed.ReportFailedKey, NAEDOReportFailedDTO.ReportFailedKey>();
      AutoMapper.Mapper.CreateMap<NAEDOReportFuture, NAEDOReportFutureDTO>();
      AutoMapper.Mapper.CreateMap<NAEDOReportFuture.ReportFutureKey, NAEDOReportFutureDTO.ReportFutureKey>();
      AutoMapper.Mapper.CreateMap<NAEDOReportInProcess, NAEDOReportInProcessDTO>();
      AutoMapper.Mapper.CreateMap<NAEDOReportInProcess.ReportInProcessKey, NAEDOReportInProcessDTO.ReportInProcessKey>();
      AutoMapper.Mapper.CreateMap<NAEDOReportSuccess, NAEDOReportSuccessDTO>();
      AutoMapper.Mapper.CreateMap<NAEDOReportSuccess.ReportSuccessKey, NAEDOReportSuccessDTO.ReportSuccessKey>();
      AutoMapper.Mapper.CreateMap<NAEDOReportTransactionUploaded, NAEDOReportTransactionUploadedDTO>();
      AutoMapper.Mapper.CreateMap<NAEDOReportTransactionUploaded.ReportTransactionUploadedKey, NAEDOReportTransactionUploadedDTO.ReportTransactionUploadedKey>();

      #endregion

      #region AEDO Report Import

      AutoMapper.Mapper.CreateMap<AEDOReportBatch, AEDOReportBatchDTO>();
      AutoMapper.Mapper.CreateMap<AEDOReportCancelled, AEDOReportCancelledDTO>();
      AutoMapper.Mapper.CreateMap<AEDOReportCancelled.ReportCancelledKey, AEDOReportCancelledDTO.ReportCancelledKey>();
      AutoMapper.Mapper.CreateMap<AEDOReportFailed, AEDOReportFailedDTO>();
      AutoMapper.Mapper.CreateMap<AEDOReportFailed.ReportFailedKey, AEDOReportFailedDTO.ReportFailedKey>();
      AutoMapper.Mapper.CreateMap<AEDOReportFuture, AEDOReportFutureDTO>();
      AutoMapper.Mapper.CreateMap<AEDOReportFuture.ReportFutureKey, AEDOReportFutureDTO.ReportFutureKey>();
      AutoMapper.Mapper.CreateMap<AEDOReportNewTransaction, AEDOReportNewTransactionDTO>();
      AutoMapper.Mapper.CreateMap<AEDOReportNewTransaction.ReportNewTransactionKey, AEDOReportNewTransactionDTO.ReportNewTransactionKey>();
      AutoMapper.Mapper.CreateMap<AEDOReportRetry, AEDOReportRetryDTO>();
      AutoMapper.Mapper.CreateMap<AEDOReportRetry.ReportRetryKey, AEDOReportRetryDTO.ReportRetryKey>();
      AutoMapper.Mapper.CreateMap<AEDOReportSettled, AEDOReportSettledDTO>();
      AutoMapper.Mapper.CreateMap<AEDOReportSettled.ReportSettledKey, AEDOReportSettledDTO.ReportSettledKey>();
      AutoMapper.Mapper.CreateMap<AEDOReportSuccess, AEDOReportSuccessDTO>();
      AutoMapper.Mapper.CreateMap<AEDOReportSuccess.ReportSuccessKey, AEDOReportSuccessDTO.ReportSuccessKey>();
      AutoMapper.Mapper.CreateMap<AEDOReportUnmatched, AEDOReportUnmatchedDTO>();
      AutoMapper.Mapper.CreateMap<AEDOReportUnsettled, AEDOReportUnsettledDTO>();
      AutoMapper.Mapper.CreateMap<AEDOReportUnsettled.ReportUnsettledKey, AEDOReportUnsettledDTO.ReportUnsettledKey>();

      #endregion

      #region Account

      AutoMapper.Mapper.CreateMap<ACC_Account, ACC_AccountDTO>();
      AutoMapper.Mapper.CreateMap<ACC_AccountFee, ACC_AccountFeeDTO>();
      AutoMapper.Mapper.CreateMap<ACC_AccountType, ACC_AccountTypeDTO>();
      AutoMapper.Mapper.CreateMap<ACC_AccountNote, ACC_AccountNoteDTO>();
      AutoMapper.Mapper.CreateMap<ACC_AccountPayRule, ACC_AccountPayRuleDTO>();
      AutoMapper.Mapper.CreateMap<ACC_AccountStatus, ACC_AccountStatusDTO>();
      AutoMapper.Mapper.CreateMap<ACC_AccountTypeFee, ACC_AccountTypeFeeDTO>();
      AutoMapper.Mapper.CreateMap<ACC_Affordability, ACC_AffordabilityDTO>();
      AutoMapper.Mapper.CreateMap<ACC_AffordabilityCategory, ACC_AffordabilityCategoryDTO>();
      AutoMapper.Mapper.CreateMap<ACC_AffordabilityCategoryType, ACC_AffordabilityCategoryTypeDTO>();
      AutoMapper.Mapper.CreateMap<ACC_AffordabilityOption, ACC_AffordabilityOptionDTO>();
      AutoMapper.Mapper.CreateMap<ACC_AffordabilityOptionFee, ACC_AffordabilityOptionFeeDTO>();
      AutoMapper.Mapper.CreateMap<ACC_AffordabilityOptionStatus, ACC_AffordabilityOptionStatusDTO>();
      AutoMapper.Mapper.CreateMap<ACC_AffordabilityOptionType, ACC_AffordabilityOptionTypeDTO>();
      AutoMapper.Mapper.CreateMap<ACC_Arrearage, ACC_ArrearageDTO>();
      AutoMapper.Mapper.CreateMap<ACC_PayDate, ACC_PayDateDTO>();
      AutoMapper.Mapper.CreateMap<ACC_PayDateType, ACC_PayDateTypeDTO>();
      AutoMapper.Mapper.CreateMap<ACC_PayRule, ACC_PayRuleDTO>();
      AutoMapper.Mapper.CreateMap<ACC_PeriodFrequency, ACC_PeriodFrequencyDTO>();
      AutoMapper.Mapper.CreateMap<ACC_QuickQuote, ACC_QuickQuoteDTO>();
      AutoMapper.Mapper.CreateMap<ACC_Quotation, ACC_QuotationDTO>();
      AutoMapper.Mapper.CreateMap<ACC_QuotationStatus, ACC_QuotationStatusDTO>();
      AutoMapper.Mapper.CreateMap<ACC_Settlement, ACC_SettlementDTO>();
      AutoMapper.Mapper.CreateMap<ACC_SettlementStatus, ACC_SettlementStatusDTO>();
      AutoMapper.Mapper.CreateMap<ACC_SettlementType, ACC_SettlementTypeDTO>();
      AutoMapper.Mapper.CreateMap<ACC_Status, ACC_StatusDTO>();
      AutoMapper.Mapper.CreateMap<ACC_StatusReason, ACC_StatusReasonDTO>();
      AutoMapper.Mapper.CreateMap<ACC_StatusSubReason, ACC_StatusSubReasonDTO>();
      AutoMapper.Mapper.CreateMap<ACC_ThirdPartyPayout, ACC_ThirdPartyPayoutDTO>();
      //AutoMapper.Mapper.CreateMap<ACC_ScoreCard, ACC_ScoreCardDTO>();
      //AutoMapper.Mapper.CreateMap<ACC_ScoreRiskLevel, ACC_ScoreRiskLevelDTO>();
      AutoMapper.Mapper.CreateMap<ACC_TopUp, ACC_TopUpDTO>();
      AutoMapper.Mapper.CreateMap<ACC_TopUpStatus, ACC_TopUpStatusDTO>();
      AutoMapper.Mapper.CreateMap<ACC_Policy, ACC_PolicyDTO>();

      #endregion

      #region Host

      AutoMapper.Mapper.CreateMap<Host, HostDTO>();

      #endregion

      #region Payout

      AutoMapper.Mapper.CreateMap<PYT_Batch, PYT_BatchDTO>();
      AutoMapper.Mapper.CreateMap<PYT_BatchStatus, PYT_BatchStatusDTO>();
      AutoMapper.Mapper.CreateMap<PYT_HostService, PYT_HostServiceDTO>();
      AutoMapper.Mapper.CreateMap<PYT_Payout, PYT_PayoutDTO>();
      AutoMapper.Mapper.CreateMap<PYT_PayoutStatus, PYT_PayoutStatusDTO>();
      AutoMapper.Mapper.CreateMap<PYT_PayoutValidation, PYT_PayoutValidationDTO>();
      AutoMapper.Mapper.CreateMap<PYT_ResultCode, PYT_ResultCodeDTO>();
      AutoMapper.Mapper.CreateMap<PYT_ResultQualifier, PYT_ResultQualifierDTO>();
      AutoMapper.Mapper.CreateMap<PYT_ReplyCode, PYT_ReplyCodeDTO>();
      AutoMapper.Mapper.CreateMap<PYT_ReplyCodeType, PYT_ReplyCodeTypeDTO>();
      AutoMapper.Mapper.CreateMap<PYT_Service, PYT_ServiceDTO>();
      AutoMapper.Mapper.CreateMap<PYT_ServiceSchedule, PYT_ServiceScheduleDTO>();
      AutoMapper.Mapper.CreateMap<PYT_ServiceScheduleBank, PYT_ServiceScheduleBankDTO>();
      AutoMapper.Mapper.CreateMap<PYT_ServiceType, PYT_ServiceTypeDTO>();
      AutoMapper.Mapper.CreateMap<PYT_Transmission, PYT_TransmissionDTO>();
      AutoMapper.Mapper.CreateMap<PYT_TransmissionSet, PYT_TransmissionSetDTO>();
      AutoMapper.Mapper.CreateMap<PYT_TransmissionTransaction, PYT_TransmissionTransactionDTO>();
      AutoMapper.Mapper.CreateMap<PYT_Validation, PYT_ValidationDTO>();

      #endregion

      #region Workflow

      AutoMapper.Mapper.CreateMap<WFL_BusinessDay, WFL_BusinessDayDTO>();
      AutoMapper.Mapper.CreateMap<WFL_ConditionClass, WFL_ConditionClassDTO>();
      AutoMapper.Mapper.CreateMap<WFL_ConditionClassProperty, WFL_ConditionClassPropertyDTO>();
      AutoMapper.Mapper.CreateMap<WFL_Condition, WFL_ConditionDTO>();
      AutoMapper.Mapper.CreateMap<WFL_ConditionGroup, WFL_ConditionGroupDTO>();
      AutoMapper.Mapper.CreateMap<WFL_ConditionPrimaryKey, WFL_ConditionPrimaryKeyDTO>();
      AutoMapper.Mapper.CreateMap<WFL_Decision, WFL_DecisionDTO>();
      AutoMapper.Mapper.CreateMap<WFL_EscalationGroup, WFL_EscalationGroupDTO>();
      AutoMapper.Mapper.CreateMap<WFL_EscalationGroupPerson, WFL_EscalationGroupPersonDTO>();
      AutoMapper.Mapper.CreateMap<WFL_EscalationLevel, WFL_EscalationLevelDTO>();
      AutoMapper.Mapper.CreateMap<WFL_EscalationTemplate, WFL_EscalationTemplateDTO>();
      AutoMapper.Mapper.CreateMap<WFL_JobState, WFL_JobStateDTO>();
      AutoMapper.Mapper.CreateMap<WFL_PeriodFrequency, WFL_PeriodFrequencyDTO>();
      AutoMapper.Mapper.CreateMap<WFL_Process, WFL_ProcessDTO>();
      AutoMapper.Mapper.CreateMap<WFL_ProcessJob, WFL_ProcessJobDTO>();
      AutoMapper.Mapper.CreateMap<WFL_ProcessStep, WFL_ProcessStepDTO>();
      AutoMapper.Mapper.CreateMap<WFL_ProcessStepEscalation, WFL_ProcessStepEscalationDTO>();
      AutoMapper.Mapper.CreateMap<WFL_ProcessStepEscalationGroup, WFL_ProcessStepEscalationGroupDTO>();
      AutoMapper.Mapper.CreateMap<WFL_ProcessStepJob, WFL_ProcessStepJobDTO>();
      AutoMapper.Mapper.CreateMap<WFL_ProcessStepJobAccount, WFL_ProcessStepJobAccountDTO>();
      AutoMapper.Mapper.CreateMap<WFL_ProcessStepJobEscalation, WFL_ProcessStepJobEscalationDTO>();
      AutoMapper.Mapper.CreateMap<WFL_ProcessStepJobEscalationNotification, WFL_ProcessStepJobEscalationNotificationDTO>();
      AutoMapper.Mapper.CreateMap<WFL_ProcessStepUserGroup, WFL_ProcessStepUserGroupDTO>();
      AutoMapper.Mapper.CreateMap<WFL_ScheduleFrequency, WFL_ScheduleFrequencyDTO>();
      AutoMapper.Mapper.CreateMap<WFL_ScheduleProcess, WFL_ScheduleProcessDTO>();
      AutoMapper.Mapper.CreateMap<WFL_ScheduleProcessDay, WFL_ScheduleProcessDayDTO>();
      AutoMapper.Mapper.CreateMap<WFL_ScheduleProcessStatus, WFL_ScheduleProcessStatusDTO>();
      AutoMapper.Mapper.CreateMap<WFL_ScheduleProcessStep, WFL_ScheduleProcessStepDTO>();
      AutoMapper.Mapper.CreateMap<WFL_Trigger, WFL_TriggerDTO>();
      AutoMapper.Mapper.CreateMap<WFL_UserGroup, WFL_UserGroupDTO>();
      AutoMapper.Mapper.CreateMap<WFL_UserGroupLink, WFL_UserGroupLinkDTO>();
      AutoMapper.Mapper.CreateMap<WFL_Workflow, WFL_WorkflowDTO>();
      AutoMapper.Mapper.CreateMap<WFL_WorkflowHost, WFL_WorkflowHostDTO>();

      #endregion

      #region Notification

      AutoMapper.Mapper.CreateMap<NTF_Group, NTF_GroupDTO>();
      AutoMapper.Mapper.CreateMap<NTF_GroupTemplate, NTF_GroupTemplateDTO>();
      AutoMapper.Mapper.CreateMap<NTF_Notification, NTF_HistoryDTO>();
      AutoMapper.Mapper.CreateMap<NTF_Priority, NTF_PriorityDTO>();
      AutoMapper.Mapper.CreateMap<NTF_Status, NTF_StatusDTO>();
      AutoMapper.Mapper.CreateMap<NTF_Template, NTF_TemplateDTO>();
      AutoMapper.Mapper.CreateMap<NTF_TemplateType, NTF_TemplateTypeDTO>();
      AutoMapper.Mapper.CreateMap<NTF_Type, NTF_TypeDTO>();

      #endregion

      #region Biometric

      AutoMapper.Mapper.CreateMap<BIO_Setting, BIO_SettingDTO>();

      #endregion

      #region Debit

      AutoMapper.Mapper.CreateMap<DBT_AVSCheckType, DBT_AVSCheckTypeDTO>();
      AutoMapper.Mapper.CreateMap<DBT_Batch, DBT_BatchDTO>();
      AutoMapper.Mapper.CreateMap<DBT_BatchStatus, DBT_BatchStatusDTO>();
      AutoMapper.Mapper.CreateMap<DBT_Control, DBT_ControlDTO>();
      AutoMapper.Mapper.CreateMap<DBT_ControlStatus, DBT_ControlStatusDTO>();
      AutoMapper.Mapper.CreateMap<DBT_ControlType, DBT_ControlTypeDTO>();
      AutoMapper.Mapper.CreateMap<DBT_ControlValidation, DBT_ControlValidationDTO>();
      AutoMapper.Mapper.CreateMap<DBT_DebitType, DBT_DebitTypeDTO>();
      AutoMapper.Mapper.CreateMap<DBT_FailureType, DBT_FailureTypeDTO>();
      AutoMapper.Mapper.CreateMap<DBT_HostService, DBT_HostServiceDTO>();
      AutoMapper.Mapper.CreateMap<DBT_ResponseCode, DBT_ResponseCodeDTO>();
      AutoMapper.Mapper.CreateMap<DBT_Service, DBT_ServiceDTO>();
      AutoMapper.Mapper.CreateMap<DBT_ServiceMessage, DBT_ServiceMessageDTO>();
      AutoMapper.Mapper.CreateMap<DBT_ServiceSchedule, DBT_ServiceScheduleDTO>();
      AutoMapper.Mapper.CreateMap<DBT_ServiceScheduleBank, DBT_ServiceScheduleBankDTO>();
      AutoMapper.Mapper.CreateMap<DBT_Status, DBT_StatusDTO>();
      AutoMapper.Mapper.CreateMap<DBT_Transaction, DBT_TransactionDTO>();
      AutoMapper.Mapper.CreateMap<DBT_Transmission, DBT_TransmissionDTO>();
      AutoMapper.Mapper.CreateMap<DBT_TransmissionSet, DBT_TransmissionSetDTO>();
      AutoMapper.Mapper.CreateMap<DBT_TransmissionTransaction, DBT_TransmissionTransactionDTO>();
      AutoMapper.Mapper.CreateMap<DBT_Validation, DBT_ValidationDTO>();

      #endregion

      #region Document
      
      AutoMapper.Mapper.CreateMap<DOC_Category, DOC_CategoryDTO>();
      AutoMapper.Mapper.CreateMap<DOC_FileFormatType, DOC_FileFormatTypeDTO>();      
      AutoMapper.Mapper.CreateMap<DOC_TemplateType, DOC_TemplateTypeDTO>();

      AutoMapper.Mapper.CreateMap<DOC_TemplateStore, DOC_TemplateStoreDTO>();
      AutoMapper.Mapper.CreateMap<DOC_FileStore, DOC_FileStoreDTO>();

      AutoMapper.Mapper.CreateMap<LNG_Language , LNG_LanguageDTO>();  
          
      #endregion

      #region Target

      AutoMapper.Mapper.CreateMap<TAR_DailySale, TAR_DailySaleDTO>();
      AutoMapper.Mapper.CreateMap<TAR_HandoverTarget, TAR_HandoverTargetDTO>();
      AutoMapper.Mapper.CreateMap<TAR_BranchCIMonthly, TAR_BranchCIMonthlyDTO>();

      #endregion

      #region Stream

      //AutoMapper.Mapper.CreateMap<STR_Account, STR_AccountDTO>();
      //AutoMapper.Mapper.CreateMap<STR_AccountNote, STR_AccountNoteDTO>();
      //AutoMapper.Mapper.CreateMap<STR_AccountNoteType, STR_AccountNoteTypeDTO>();
      //AutoMapper.Mapper.CreateMap<STR_Case, STR_CaseDTO>();
      //AutoMapper.Mapper.CreateMap<STR_CaseNotification, STR_CaseNotificationDTO>();
      //AutoMapper.Mapper.CreateMap<STR_CaseStatus, STR_CaseStatusDTO>();
      //AutoMapper.Mapper.CreateMap<STR_CaseStream, STR_CaseStreamDTO>();
      //AutoMapper.Mapper.CreateMap<STR_CaseStreamAction, STR_CaseStreamActionDTO>();
      //AutoMapper.Mapper.CreateMap<STR_Category, STR_CategoryDTO>();
      //AutoMapper.Mapper.CreateMap<STR_CategoryHost, STR_CategoryHostDTO>();
      //AutoMapper.Mapper.CreateMap<STR_Comment, STR_CommentDTO>();
      //AutoMapper.Mapper.CreateMap<STR_Debtor, STR_DebtorDTO>();
      //AutoMapper.Mapper.CreateMap<STR_DebtorContact, STR_DebtorContactDTO>();
      //AutoMapper.Mapper.CreateMap<STR_Priority, STR_PriorityDTO>();
      //AutoMapper.Mapper.CreateMap<STR_ActionType, STR_ActionTypeDTO>();
      //AutoMapper.Mapper.CreateMap<STR_Stream, STR_StreamDTO>();
      //AutoMapper.Mapper.CreateMap<STR_CommentGroup, STR_CommentGroupDTO>();
      //AutoMapper.Mapper.CreateMap<STR_Transaction, STR_TransactionDTO>();
      //AutoMapper.Mapper.CreateMap<STR_TransactionStatus, STR_TransactionStatusDTO>();
      //AutoMapper.Mapper.CreateMap<STR_TransactionType, STR_TransactionTypeDTO>();
      //AutoMapper.Mapper.CreateMap<STR_Group, STR_GroupDTO>();
      //AutoMapper.Mapper.CreateMap<STR_GroupHost, STR_GroupHostDTO>();
      //AutoMapper.Mapper.CreateMap<STR_GroupStream, STR_GroupStreamDTO>();
      //AutoMapper.Mapper.CreateMap<STR_SubCategory, STR_SubCategoryDTO>();

      #endregion

    }
  }
}