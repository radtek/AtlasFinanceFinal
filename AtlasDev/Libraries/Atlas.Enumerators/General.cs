using System;
using System.ComponentModel;


namespace Atlas.Enumerators
{
  public class General
  {
    public enum Person
    {
      [Description("System")]
      System = 1
    }


    #region Log4Net

    public enum Log4NetType
    {
      [Description("Error")]
      Error = 1,
      [Description("Fatal")]
      Fatal = 2,
      [Description("Info")]
      Info = 3,
      [Description("Trace")]
      Trace = 4,
      [Description("Warning")]
      Warning = 5
    }

    #endregion


    #region Business Related

    public enum Days
    {
      [Description("Monday")]
      Monday = DayOfWeek.Monday,
      [Description("Tuesday")]
      Tuesday = DayOfWeek.Tuesday,
      [Description("Wednesday")]
      Wednesday = DayOfWeek.Wednesday,
      [Description("Thursday")]
      Thursday = DayOfWeek.Thursday,
      [Description("Friday")]
      Friday = DayOfWeek.Friday,
      [Description("Saturday")]
      Saturday = DayOfWeek.Saturday,
      [Description("Sunday")]
      Sunday = DayOfWeek.Sunday
    }

    #endregion

    public enum PersonType
    {
      [Description("Client")]
      Client = 1,
      [Description("Employee")]
      Employee = 2
    }
    /// <summary>
    /// Defines the current host environment for the record.
    /// </summary>
    public enum Host
    {
      [Description("ASS")]
      ASS = 1,
      [Description("Atlas Online")]
      Atlas_Online = 2,
      [Description("Atlas Call Centre")]
      Atlas_CallCentre = 3,
      [Description("SDC")]
      SDC = 4,
      [Description("Loan Admin System")]
      LAS = 5,
      [Description("Falcon")]
      Falcon = 6,
      [Description("Sales Agent App")]
      SalesAgentApp = 7
    }

    public enum Gender
    {
      [Description("M")]
      Male = 0,
      [Description("F")]
      Female = 1
    }

    public enum Language
    {
      [Description("Not Set")]
      NotSet = 0,
      [Description("English")]
      English = 1,
      [Description("Afrikaans")]
      Afrikaans = 2,
      [Description("Zulu")]
      Zulu = 3,
      [Description("Sotho")]
      Sotho = 4,
      [Description("Xosa")]
      Xosa = 5
    }


    public enum ApplicationIdentifiers
    {
      [Description("Not Set")]
      NotSet = 0,
      [Description("ASS")]
      ASS = 1,
      [Description("Atlas Management System")]
      AtlasManagement = 2,
      [Description("ASS Fingerprint client")]
      ASSFingerprintClient = 3,
      [Description("ASS Data sync client")]
      DataSyncClient = 4,
      [Description("Loan Management System")]
      LMS = 5,
      [Description("Core Database Administration")]
      CoreAdmin = 6,
      [Description("Atlas Automated Server Process")]
      ServerAutoProcess = 7,
      [Description("External")]
      External = 7

    }

    // WCF result function codes
    public enum WCFCallResult
    {
      [Description("Bad parameters")]
      BadParams = -1,
      [Description("Result OK")]
      OK = 1,
      [Description("Service provider communications or logical error")]
      ServiceProviderCommsError = 2,
      [Description("Unexpected error on server")]
      ServerError = 3,
    }

    public enum LogProductRequestResult
    {
      [Description("OK")]
      Successful = 1,
      [Description("No operation was performed")]
      NoOperation = 0,
      [Description("A general error occurred")]
      GeneralError = -1,
      [Description("Custom error message (see ResultText field for description)")]
      CustomError = -2,
      [Description("Product already allocated")]
      CardAlreadyAllocated = -3,
    }

    public enum LogProductRequestType
    {
      [Description("Not Set")]
      NotSet = 0,
      [Description("Allocate product to a person")]
      LinkCard = 1
    }

    public enum HWStatus
    {
      [Description("Not Set")]
      NotSet = 0,
      [Description("Online")]
      Online = 1,
      [Description("Offline")]
      Offline = 2,
      [Description("Unknown")]
      Unknown = 3
    }

    public enum DeviceType
    {
      [Description("Not Set")]
      NotSet = 0,
      [Description("TCC Terminal")]
      TCCTerminal = 1
    }

    public enum BankAccountType
    {
      [Description("Not Set")]
      NotSet = 0,
      [Description("Cheque / Current")]
      Cheque = 1,
      [Description("Savings")]
      Savings = 2,
      [Description("Transmission")]
      Transmission = 3
    }

    public enum TCCLogRequestType
    {
      [Description("Not Set")]
      NotSet = 0,
      [Description("Find all terminals for a branch")]
      FindAllBranchTerminals = 1,
      [Description("Find free terminal for branch")]
      FindFreeBranchTerminals = 2,
      [Description("Get terminal description")]
      GetTerminalDescription = 3,
      [Description("TCC UpfrontBinCheck")]
      UpfrontBinCheck = 4,
      [Description("TCC aedo_naedo_auth_req")]
      AEDONAEDOAuthorise = 5,
      [Description("TCC genericDataCaptureConfirm")]
      CaptureData = 6,
      [Description("TCC displayMessage")]
      FlashMessage = 7,
      [Description("TCC handshake")]
      Handshake = 8,
      [Description("TCC TermStatusCheck")]
      TermStatusCheck = 9,
      [Description("TCC tranIDQuery")]
      TranIDQuery = 10,
      [Description("TCC Check card")]
      CheckCard = 11
    }

    public enum TCCLogRequestResult
    {
      [Description("Not Set")]
      NotSet = 0,
      [Description("Successful")]
      Successful = 1,
      [Description("Unsuccessful")]
      Unsuccessful = 2,
      [Description("Communications error")]
      CommunicationsError = 3,
      [Description("Application error")]
      ApplicationError = 4
    }

    public enum SalaryFrequency
    {
      [Description("Not Set")]
      NotSet = 0,
      [Description("Monthly")]
      Monthly = 1,
      [Description("Bi-Weekly")]
      Bi_Weekly = 2,
      [Description("Weekly")]
      Weekly = 3,
      [Description("Fortnightly")]
      Fortnightly = 4
    }

    public enum ContactType
    {
      [Description("Not Set")]
      NotSet = 0,
      [Description("Email")]
      Email = 1,
      [Description("Tel No Home")]
      TelNoHome = 2,
      [Description("Tel No Work")]
      TelNoWork = 3,
      [Description("Tel No Work Fax")]
      TelNoWorkFax = 4,
      [Description("Cell No")]
      CellNo = 6,
      [Description("Cell Work")]
      CellWork = 7,
      [Description("Work Email")]
      WorkEmail = 8,
      [Description("Personal Email")]
      PersonalEmail = 9,
      [Description("Fax No")]
      FaxNo = 10
    }

    public enum TerminalStatus
    {
      [Description("Ready")]
      Ready = 0,
      [Description("In Use")]
      InUse = 1,
      [Description("Unresponsive")]
      Unresponsive = 2,
      [Description("Uncommissioned")]
      Uncommissioned = 99
    }

    public enum BankPeriod
    {
      [Description("Not Set")]
      NotSet = 0,
      [Description("Less than three months")]
      LessThanThreeMonths = 1,
      [Description("Greater than three months")]
      GreaterThanThreeMonths = 2,
      [Description("Greater than twelve months")]
      GreaterThanTwelveMonths = 3
    }

    public enum BankName
    {
      [Description("Not Set")]
      NotSet = 1000, // Required change from 0, postgres does not allow 0 id's,
      [Description("Required")]
      Required = 0,
      [Description("Standard Bank")]
      STD = 1,
      [Description("First National Bank")]
      FNB = 2,
      [Description("Nedbank")]
      NED = 3,
      [Description("Perm")]
      PER = 4,
      [Description("Boland Bank")]
      BOL = 5,
      [Description("Bank of Lisbon")]
      LIS = 6,
      [Description("Saambou")]
      SAM = 7,
      [Description("NBS")]
      NBS = 8,
      [Description("Telebank")]
      TEL = 9,
      [Description("ABSA")]
      ABS = 10,
      [Description("Unibank")]
      UNB = 11,
      [Description("Mercantile Lisbon")]
      MER = 12,
      [Description("Post Bank")]
      POS = 13,
      [Description("PEP Bank")]
      PEP = 14,
      [Description("Pick 'N Pay")]
      P_N_P = 15,
      [Description("Capitec")]
      CAP = 16,
      [Description("Bank of Athens")]
      BOA = 17,
      [Description("African Bank")]
      AFR = 18,
      [Description("Investec Bank")]
      INV = 19,
      [Description("UBank")]
      UBA = 20,
      [Description("Bidvest Bank")]
      BID = 21
    }

    public enum Province
    {
      [Description("Not Set")]
      NotSet = 0,
      [Description("Eastern Cape")]
      EC = 1,
      [Description("Gauteng")]
      GAU = 2,
      [Description("Kwazulu Natal")]
      KZN = 3,
      [Description("Limpopo")]
      LIM = 4,
      [Description("Mpumalanga")]
      MPL = 5,
      [Description("Northern Cape")]
      NC = 6,
      [Description("North West")]
      NW = 7,
      [Description("Western Cape")]
      WC = 8,
      [Description("Free State")]
      FS = 9
    }

    public enum NucardBatchStatus
    {
      [Description("Waiting for collection")]
      Waiting_Collection = 0,
      [Description("In Transit")]
      In_Transit = 1,
      [Description("Delivered")]
      Delivered = 2,
      [Description("Lost in transit")]
      Lost_In_Transit = 3,
      [Description("Partially Receipted")]
      Partially_Receipted = 4
    }

    public enum ProductBatchStatus
    {
      [Description("New")]
      New = 0,
      [Description("Waiting for collection")]
      Waiting_Collection = 1,
      [Description("In Transit")]
      In_Transit = 2,
      [Description("Delivered")]
      Delivered = 3,
      [Description("Lost in transit")]
      Lost_In_Transit = 4,
      [Description("Partially Receipted")]
      Partially_Receipted = 5
    }

    public enum ProductType
    {
      [Description("Sim Card")]
      Sim_Card = 1
    }

    public enum AddressType
    {
      [Description("Not Set")]
      NotSet = 0,
      [Description("Postal")]
      Postal = 1,
      [Description("Residential")]
      Residential = 2,
      [Description("Employer")]
      Employer = 3,
      [Description("Work")]
      Work = 4
    }

    public enum BranchConfigDataType
    {
      [Description("TCC Merchant ID")]
      Merchant_Id = 1,
      [Description("AEDO Merchant ID")]
      AEDO_Merchant_Id = 2,
      [Description("NAEDO Merchant ID")]
      NAEDO_Merchant_Id = 3,
      [Description("FP Enabled")]
      FP_Enabled = 4,
      [Description("TCC Terminal ID")]
      Terminal_Id = 30000,
      [Description("TCC Profile Number")]
      Profile_Num = 30001,
      [Description("TCC Terminal Password")]
      Terminal_Password = 30002,
    }

    public enum TransactionType
    {
      [Description("Not Set")]
      NotSet = 0,
      [Description("NuCard")]
      NuCard = 1,
      [Description("NAEDO")]
      NAEDO = 2,
      [Description("AEDO")]
      AEDO = 3
    }

    public enum RoleType
    {
      [Description("Administrator")]
      Administrator = 0,

      #region NuCard related
      [Description("NuCard Payout Approver 1st")]
      NuCard_Payout_Approver_One = 1,
      [Description("NuCard Payout Approver 2nd")]
      NuCard_Payout_Approver_Two = 2,
      [Description("NuCard Allocate Card")]
      NuCard_Allocate_Card = 3,
      [Description("NuCard Load Funds")]
      NuCard_Load_Funds = 4,
      [Description("NuCard Stock Capture")]
      NuCard_Stock_Capture = 5,
      [Description("NuCard Stock Receive")]
      NuCard_Stock_Receive = 6,
      [Description("NuCard Process Approval")]
      NuCard_Process_Approval = 7,
      [Description("NuCard Pending Batch")]
      NuCard_Pending_Batch = 8,
      [Description("NuCard Stop Card")]
      NuCard_Stop_Card = 9,
      [Description("NuCard Statement")]
      NuCard_Statement = 10,
      #endregion

      #region Fingeprint related rights
      [Description("Person may enroll fingerprints onto the system")]
      CanEnrollFingerprints = 100,

      [Description("Person may add new/edit staff members details")]
      CanAddEditStaff = 101,

      [Description("Person may set the core capture settings to be used")]
      FPAdminCoreCaptureSettings = 102,

      [Description("Person may set the over-ride capture settings for a person")]
      FPAdminPersonCaptureOverrides = 103,

      [Description("Person may review raw fingerprint images")]
      FPReviewImages = 104,
      #endregion

      #region ASS Data management - data sync/etc
      [Description("Can administer ASS master tables (BRANCH, ASSTMAST, etc.)")]
      ASSMasterAdmin = 200,

      [Description("Can upload DBF files as master tables")]
      ASSMasterUpload = 201,

      [Description("Can request a branches DBF")]
      ASSGetBranchDBF = 202,

      [Description("May administer the database version to run in a specific branch")]
      ASSBranchDBVersionAdmin = 203,

      [Description("May review the branch server upload status")]
      ASSBranchServerReview = 204,
      #endregion

      #region Core Atlas database configuration
      [Description("May administer Atlas regions")]
      DBAdminRegions = 300,

      [Description("May administer Atlas companies")]
      DBAdminCompanies = 301,

      [Description("May administer Atlas branches")]
      DBAminBranches = 302,

      [Description("May administer Atlas config (Config table)")]
      DBAdminConfigs = 303,

      [Description("May administer branch config (BRN_Config table)")]
      DBAdminBranchConfigs = 304,

      [Description("May configure alerts")]
      DBAdminAlerts = 305,
      #endregion

      #region Machine administration
      [Description("May administer HO machines")]
      MachineAdminHO = 400,

      [Description("May administer branch servers")]
      MachineAdminBranchServers = 401,

      [Description("May administer branch machines")]
      MachineAdminBranchMachines = 402,
      #endregion

      #region TCC Administration
      [Description("May adminiter the TCC machines")]
      TCCAdminMachines = 500,

      [Description("May review TCC logs")]
      TCCReviewLogs = 501,

      [Description("May administer TCC display templates")]
      TCCAdminTemplates = 502,
      #endregion

      #region Company Hierachy.

      [Description("Branch Manager")]
      Branch_Manager = 601,

      [Description("Admin Manager")]
      Admin_Manager = 602,

      [Description("Regional Manager")]
      Regional_Manager = 603,

      [Description("Operation Executive")]
      Operation_Executive = 604,

      [Description("Director")]
      Director = 605

      #endregion
    }

    public enum RelationType
    {
      [Description("Not Set")]
      NotSet = 0,
      [Description("Father")]
      Father = 1,
      [Description("Mother")]
      Mother = 2,
      [Description("Sister")]
      Sister = 3,
      [Description("Brother")]
      Brother = 4,
      [Description("Friend")]
      Friend = 5,
      [Description("Spouse")]
      Spouse = 6,
      [Description("Relative")]
      Relative = 7
    }

    public enum AuditStatusType
    {
      [Description("Failure")]
      Failure = 1,
      [Description("Warning")]
      Warning = 2,
      [Description("Fatal")]
      Fatal = 3,
      [Description("Invalid")]
      Invalid = 4,
      [Description("Unauthorised")]
      Unauthorised = 5,
      [Description("Authorised")]
      Authorised = 6,
      [Description("Update")]
      Update = 7,
      [Description("Logged out")]
      LoggedOut = 10,
      [Description("Automatically logged out")]
      AutoLoggedOut = 11
    }

    public enum CompanyType
    {
      [Description("Not Set")]
      NotSet = 0,
      [Description("Courier")]
      Courier = 1
    }

    public enum PaymentType
    {
      [Description("Commissions")]
      COM = 0,
      [Description("Standard")]
      STD = 1
    }

    public enum EthnicGroup
    {
      [Description("Not Set")]
      NotSet = 0,
      [Description("Asian")]
      Asian = 1,
      [Description("Black")]
      Black = 2,
      [Description("Coloured")]
      Coloured = 3,
      [Description("Indian")]
      Indian = 4,
      [Description("White")]
      White = 5,
      [Description("Other")]
      Other = 6
    }
  }
}
