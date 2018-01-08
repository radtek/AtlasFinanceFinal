using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Common.Extensions;
using Atlas.Common.Utils;
using Atlas.Domain.DTO;
using Atlas.Domain.DTO.Nucard;
using Atlas.Domain.Security;
using Atlas.Enumerators;
using Atlas.NuCard.Repository;
using Atlas.NuCard.WCF.Interface;
using DevExpress.Xpo;


namespace Atlas.WCF.Implementation
{
  public static class WCFUtils
  {
    /// <summary>
    /// Performs minimal checks on request and returns the necessary branch/user DTO's
    /// </summary>
    /// <param name="request">Source request parameters to be checked</param>
    /// <param name="machine">Found machine details</param>
    /// <param name="branch">Found branch</param>
    /// <param name="errorMessage">Any error message while checking</param>    
    /// <param name="user">Found user details</param>
    /// <returns>bool- true if request parameters are accepted, false if request request parameters are rejected</returns>
    public static bool CheckSourceRequest(SourceRequest request,
      out COR_AppUsageDTO application, out PER_SecurityDTO user, out BRN_BranchDTO branch, out string errorMessage)
    {
      user = null;
      application = null;
      branch = null;

      #region Check parameters
      if (string.IsNullOrEmpty(request.MachineUniqueID) || request.MachineUniqueID.Length < 8)
      {
        errorMessage = "MachineUniqueID cannot be empty";
        return false;
      }
      
      if (string.IsNullOrEmpty(request.BranchCode))
      {
        errorMessage = "Branch code cannot be empty";
        return false;
      }
      
      if (string.IsNullOrEmpty(request.BranchCode) && request.BranchCode.Length < 2)
      {
        errorMessage = "Invalid branch code";
        return false;
      }
      
      if (string.IsNullOrEmpty(request.UserIDOrPassport) || request.UserIDOrPassport.Length < 4)
      {
        errorMessage = "User ID cannot be empty";
        return false;
      }

      if (string.IsNullOrEmpty(request.AppName))
      {
        errorMessage = "Application name cannot be empty";
        return false;
      }

      if (string.IsNullOrEmpty(request.AppVer))
      {
        errorMessage = "Application version cannot be empty";
        return false;
      }

      if (string.IsNullOrEmpty(request.MachineIPAddresses))
      {
        errorMessage = "Machine IP cannot be empty";
        return false;
      }

      if (request.MachineDateTime == DateTime.MinValue)
      {
        errorMessage = "Machine date/time cannot be empty";
        return false;
      }

      var ipAddresses = request.MachineIPAddresses.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
      foreach (var ipAddress in ipAddresses)
      {
        System.Net.IPAddress address;
        if (!System.Net.IPAddress.TryParse(ipAddress, out address))
        {
          errorMessage = string.Format("Invalid IP address: {0}", ipAddress);
          return false;
        }
      }

      // Machine name should adhere to format: 000-IT-000  -> BBB-DD-WWW -> Branch / Designation / Workstation ID
      if (string.IsNullOrEmpty(request.MachineName) || request.MachineName.Length < 8)
      {
        errorMessage = "Invalid/missing machine name";
        return false;
      }

      if (request.MachineName != "HO-SRV-TS-2")
      {
        if (!System.Text.RegularExpressions.Regex.IsMatch(request.MachineName, "((^0[A-Z]{0,1}\\d{1,2})|(^[A-Z]{1,1}\\d{1})|(^\\d{2,3}))\\-00\\-\\d{2,2}$"))
        {
          errorMessage = string.Format("Your machine's name does not conform to Atlas naming standards: '{0}'", request.MachineName);
          return false;
        }
      }
      
      var minsDiff = Math.Abs(request.MachineDateTime.Subtract(DateTime.Now).TotalMinutes);
      if (minsDiff > 10)
      {
        errorMessage = string.Format("Your local machine's time is incorrect by {0} minutes, your machine: {1:yyyy-MM-dd HH:mm:ss}, server: {2:yyyy-MM-dd HH:mm:ss}",
          minsDiff, request.MachineDateTime, DateTime.Now);
        return false;
      }
      #endregion

      #region Only specific systems have access to any NuCard services
      General.ApplicationIdentifiers sourceApp = request.AppName.FromStringToEnum<General.ApplicationIdentifiers>();
      if (sourceApp == General.ApplicationIdentifiers.NotSet)
      {
        errorMessage = string.Format("Unknown application: {0}", request.AppName);
        return false;
      }

      if (sourceApp != General.ApplicationIdentifiers.ASS && 
        sourceApp != General.ApplicationIdentifiers.AtlasManagement &&
        sourceApp != General.ApplicationIdentifiers.CoreAdmin)
      {
        errorMessage = string.Format("Application '{0}' is not authorised for NuCard functionality", request.AppName);
        return false;
      }
      #endregion
      
      using (var unitOfWork = new UnitOfWork())
      {
        branch = Atlas.Data.Repository.AtlasData.FindBranch(request.BranchCode);
        if (branch == null)
        {
          errorMessage = "Failed to locate branch";
          return false;
        }

        var softwareDb = SecurityDb.FindOrAddSoftware(unitOfWork, request.AppName, request.AppVer, request.MachineName);
        var machineDb = SecurityDb.FindOrAddMachine(unitOfWork, request.MachineUniqueID, request.MachineIPAddresses, request.MachineName);
        user = SecurityDb.FindByIdOrLegacyOperator(request.UserIDOrPassport);
        // If we only have an ID number, we can add them to the person table, but not the Person security table, 
        // as we need the 'legacy user code' for this
        if (user == null)
        {
          errorMessage = string.Format("Failed to locate user with ID/operator number '{0}' to system- your login has not been created", request.UserIDOrPassport);
          return false;
        }
        
        var applicationDb = SecurityDb.FindOrAddAppUsage(unitOfWork, machineDb.MachineId, softwareDb.AtlasSoftwareId, user.SecurityId, branch.BranchId);
        application = AutoMapper.Mapper.Map<COR_AppUsage, COR_AppUsageDTO>(applicationDb);                        
      }
      
      errorMessage = string.Empty;
      return true;
    }

    
    /// <summary>
    /// Performs checks against a NuCard card number
    /// </summary>
    /// <param name="cardNo">The full NuCard card number</param>
    /// <param name="requiredStatus">The status the card must be in, null to skip this check</param>
    /// <param name="mustBeLinked">Whether the card must be linked, null to skip the check</param>
    /// <param name="mustBeAllocated">Whether the card must be allocate to a profile, null to skip the check</param>
    /// <param name="checkCardExpirySoon">Check whether the card is expired or expiring within next 30 days</param>
    /// <param name="errorMessage">Error message to display to the user/to log</param>
    /// <returns>NuCardDTO of found card if passed all requested checks, else null</returns>
    public static NUC_NuCardDTO CheckCard(string cardNo, List<Enumerators.NuCard.NuCardStatus> requiredStatus, 
      bool? mustBeLinked, bool? mustBeAllocated, bool checkCardExpirySoon, out string errorMessage)
    {
      errorMessage= null;

      if (!Atlas.Common.Utils.CCardValidator.Validate(CCardValidator.CardType.NuCard, cardNo))
      {
        errorMessage = string.Format("Invalid NuCard number: {0}", cardNo);
        return null;
      }

      // Ensure we have the card in our DB
      var cardInDB = NuCardDb.FindCard(cardNo);
      if (cardInDB == null)
      {
        errorMessage = "The NuCard has not been captured in Atlas NuCard stock system- please import the card into the NuCard stock system";        
        return null;
      }

      if (requiredStatus != null)
      {
        if (cardInDB.Status == null)
        {
          errorMessage = "The specified NuCard does not have a status. Please have Atlas operations correct the status of this card.";
          return null;
        }

        if (!requiredStatus.Any(s => s == cardInDB.Status.Type))
        {
          errorMessage = string.Format("The NuCard is not currently in the expected state- card status is: '{0}', required: '{1}'",
            EnumExtension.ToStringEnum(cardInDB.Status.Type), 
            string.Join(", ", requiredStatus.Select(s => s.ToStringEnum())));
          return null;
        }
      }

      #region Check card profile link
      if (mustBeLinked.HasValue)
      {
        // Ensure card is linked to a profile
        if (cardInDB.NuCardProfile == null && mustBeLinked.Value)
        {
          errorMessage = "The NuCard has not been linked to a profile";          
          return null;
        }
        if (cardInDB.NuCardProfile != null && !mustBeLinked.Value)
        {
          errorMessage = "The NuCard has already been linked to a profile";
          return null;
        }
      }
      #endregion

      #region Check card allocation
      if (mustBeAllocated.HasValue)
      {        
        if (cardInDB.AllocatedPerson == null && mustBeAllocated.Value)
        {
          errorMessage = "The NuCard has not been allocated to anybody";
          return null;
        }

        if (cardInDB.AllocatedPerson != null && !mustBeAllocated.Value)
        {
          errorMessage = string.Format("The NuCard has already been linked to person: '{0} {1}'", 
            cardInDB.AllocatedPerson.Firstname, cardInDB.AllocatedPerson.Lastname);
          return null;
        }
      }
      #endregion

      if (checkCardExpirySoon)
      {        
        if (cardInDB.ExpiryDT < DateTime.Now)
        {
          errorMessage = string.Format("This NuCard expired on '{0:dd MMM yyyy}'- please issue the customer with a replacement NuCard", cardInDB.ExpiryDT);
          return null;
        }
        // Card must be valid for at least 1 month still
        else if (cardInDB.ExpiryDT < DateTime.Now.AddMonths(1))
        {
          errorMessage = string.Format("This NuCard will expire within the next few days ({0:dd MMM yyyy})- " +
            "please issue the customer with a replacement NuCard to avoid expiry complications", cardInDB.ExpiryDT);
          return null;
        }
      }

      return cardInDB;
    }
    

    /// <summary>
    /// Gets Tutuka terminal/profile/password based on branch (SourceRequest)
    /// </summary>
    /// <param name="sourceRequest"></param>
    /// <param name="terminalID"></param>
    /// <param name="profileNumber"></param>
    /// <param name="terminalPassword"></param>
    /// <returns></returns>
    /// 
    public static CheckSourceRequestResult GetTutukaFromRequest(SourceRequest sourceRequest,
      out string terminalID, out string profileNumber, out string terminalPassword)
    {
      terminalID = null;
      profileNumber = null;
      terminalPassword = null;

      var nuCardProfile = NuCardDb.GetBranchProfile(sourceRequest.BranchCode);
      if (nuCardProfile == null)
      {
        return WCFUtils.CheckSourceRequestResult.TerminalIDNotConfigured;
      }

      // TODO: Decrypt these values?
      terminalID = nuCardProfile.TerminalId;
      profileNumber = nuCardProfile.ProfileNum;
      terminalPassword = nuCardProfile.Password;

      return CheckSourceRequestResult.ParamsOK;
    }

    /// <summary>
    /// Returns an error string for operator 
    /// </summary>
    /// <param name="requestResult">The request result enum</param>
    /// <returns>Error string</returns>
    public static string SourceRequestErrorString(CheckSourceRequestResult requestResult)
    {
      switch (requestResult)
      {
        case CheckSourceRequestResult.ParamsOK:
          return "";

        case CheckSourceRequestResult.MssingBranchCode:
          return "Missing branch code";

        case CheckSourceRequestResult.BranchNotConfigured:
          return "Branch has not been configured for NuCard";

        case CheckSourceRequestResult.BadClientDateTime:
          return "You machine's date and time appear to be incorrect- please correct";

        case CheckSourceRequestResult.TerminalIDNotConfigured:
          return "The Altech TermimalID has not been configured for your branch- please contact Atlas IT to correct";

        case CheckSourceRequestResult.ProfileNumberNotConfigured:
          return "The Altech Profile number has not been configured for your branch- please contact Atlas IT to correct";

        case CheckSourceRequestResult.TerminalPasswordNotConfigured:
          return "The Altech terminal password has not been configured for your branch- please contact Atlas IT to correct";

        case CheckSourceRequestResult.MachineNotAuthorisedForBranch:
          return "The machine you are using has not been configured for this branch- please contact Atlas IT to correct";

        case CheckSourceRequestResult.MachineNotAuthorisedForFunction:
          return "The machine you are using has not been configured for NuCard functionality- please contact Atlas IT to correct";

        case CheckSourceRequestResult.MissingMachineName:
          return "The machine name is missing, or does not adhere to Atlas standards (BBB-DD-WWW)";

        case CheckSourceRequestResult.UnknownOperatorPerson:
          return "Unable to locate the operator in the person database (operator ID not configured in users and/or missing from Person table";

        default:
          return "Unknown error";
      }
    }


    /// <summary>
    /// 'Unescapes' a URI formatted string - can handle null strings, unlike Uri.UnescapeDataString
    /// </summary>
    /// <param name="uriString">URI escaped string</param>
    /// <returns>Unescaped string- null if exception or uriString is null</returns>
    public static string UnescapeUriString(string uriString)
    {
      try
      {
        return !string.IsNullOrEmpty(uriString) ? Uri.UnescapeDataString(uriString).Trim() : null;
      }
      catch
      {
        return null;
      }
    }


    #region Enum

    public enum CheckSourceRequestResult
    {
      // Parameters appear valid
      ParamsOK = 1,

      // A valid branch code not passed
      MssingBranchCode = 2,

      // Branch has not been configured  for this functionality
      BranchNotConfigured = 3,

      // Machine not configured for this functionality
      MissingMachineName = 4,

      // Client's date/time is too far out of current date/time
      BadClientDateTime = 5,

      // Terminal ID not configured for this branch
      TerminalIDNotConfigured = 6,

      // Profile number not configured for this branch
      ProfileNumberNotConfigured = 7,

      // Machine not authorised for this branch
      MachineNotAuthorisedForBranch = 8,

      // Terminal pasdword not configured for this branch
      TerminalPasswordNotConfigured = 9,

      // OperatorID unknown
      UnknownOperatorPerson = 10,

      // Machine not authorised
      MachineNotAuthorisedForFunction = 11
    }

    #endregion
    
  }
}
