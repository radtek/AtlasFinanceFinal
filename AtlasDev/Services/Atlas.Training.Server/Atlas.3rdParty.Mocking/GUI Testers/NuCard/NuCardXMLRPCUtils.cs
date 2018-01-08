/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     Static utility methods for NuCard XML-RPC
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2012-04-19 - Skeleton created
 *     
 *     2012-06-21 - Updated to NuCard XML-RPC spec V1.8
 *     
 *     2012-08-16 - Time-out longer for Statement request
 *     
 *     2013-12-14 - Time-outs longer- NuCard can be quite slow...
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

#region Using

using System;
using System.Text;

using CookComputing.XmlRpc;

using Atlas.ThirdParty.XMLRPC.Classes;

#endregion


namespace Atlas.ThirdParty.XMLRPC.Utils
{
  public static class NuCardXMLRPCUtils
  {
    /// <summary>
    /// Function to allocate a card to a person
    /// </summary>
    /// <param name="url">The endpoint to use</param>
    /// <param name="key">The key</param>
    /// <param name="input">The xml call input parameters</param>
    /// <param name="xmlSent">The raw xml sent</param>
    /// <param name="xmlRecv">The raw xml received</param>
    /// <param name="error">any error message (call error message details returned in the result</param>
    /// <returns>AllocateCard_Output containing the result</returns>
    public static AllocateCard_Output AllocateCard(string url, string key,
        AllocateCard_Input input, out string xmlSent, out string xmlRecv, out string error)
    {
      error = string.Empty;
      var result = new AllocateCard_Output();
      XMLLogging logging = null;

      try
      {
        INuCard proxy;
        CreateProxy(url, out proxy, out logging);

        result = proxy.AllocateCard(terminalID: input.terminalID, profileNumber: input.profileNumber, cardNumber: input.cardNumber, firstName: input.firstName,
            lastName: input.lastName, idOrPassportNumber: input.idOrPassportNumber, cellPhoneNumber: input.cellPhoneNumber, transactionID: input.transactionID,
            transactionDate: input.transactionDate, checksum: Hash.GetHMACSHA1(key, input.ToString()));
      }
      catch (Exception err)
      {
        error = string.Format("Atlas/Tutuka comms error- {0}", err.Message);
      }

      xmlRecv = (logging != null) ? logging.XMLDataRecv : string.Empty;
      xmlSent = (logging != null) ? logging.XMLDataSent : string.Empty;

      return result;
    }


    /// <summary>
    /// Function to link a card to a specific profile
    /// </summary>
    /// <param name="url">The endpoint to use</param>
    /// <param name="key"></param>
    /// <param name="input">The xml call input parameters</param>
    /// <param name="xmlSent">The raw xml sent</param>
    /// <param name="xmlRecv">The raw xml received</param>
    /// <param name="error"></param>
    /// <returns></returns>
    public static LinkCardCard_Output LinkCard(string url, string key,
        LinkCard_Input input, out string xmlSent, out string xmlRecv, out string error)
    {
      error = string.Empty;
      var result = new LinkCardCard_Output();
      XMLLogging logging = null;

      try
      {
        INuCard proxy;
        CreateProxy(url, out proxy, out logging);

        var hash = Hash.GetHMACSHA1(key, input.ToString());
        result = proxy.LinkCard(terminalID: input.terminalID, profileNumber: input.profileNumber, cardNumber: input.cardNumber,
          transactionID: input.transactionID, transactionDate: input.transactionDate, checksum: hash);
      }
      catch (Exception err)
      {
        error = string.Format("Atlas/Tutuka comms error- {0}", err.Message);
      }

      xmlSent = (logging != null) ? logging.XMLDataSent : string.Empty;
      xmlRecv = (logging != null) ? logging.XMLDataRecv : string.Empty;

      return result;
    }


    public static DeLinkCard_Output DeLinkCard(string url, string key,
        DeLinkCard_Input input, out string xmlSent, out string xmlRecv, out string error)
    {
      error = string.Empty;
      var result = new DeLinkCard_Output();
      XMLLogging logging = null;

      try
      {
        INuCard proxy;
        CreateProxy(url, out proxy, out logging);

        var hash = Hash.GetHMACSHA1(key, input.ToString());
        result = proxy.DeLinkCard(terminalID: input.terminalID, profileNumber: input.profileNumber, cardNumber: input.cardNumber,
          transactionID: input.transactionID, transactionDate: input.transactionDate, checksum: hash);
      }
      catch (Exception err)
      {
        error = string.Format("Atlas/Tutuka comms error- {0}", err.Message);
      }

      xmlSent = (logging != null) ? logging.XMLDataSent : string.Empty;
      xmlRecv = (logging != null) ? logging.XMLDataRecv : string.Empty;

      return result;

    }


    /// <summary>
    /// Function to request a card's balance
    /// </summary>
    /// <param name="url">The endpoint to use</param>
    /// <param name="key">The key to use</param>
    /// <param name="input">The xml call input parameters</param>
    /// <param name="xmlSent">The raw xml sent</param>
    /// <param name="xmlRecv">The raw xml received</param>
    /// <param name="error"></param>
    /// <returns></returns>
    public static Balance_Output Balance(string url, string key,
        Balance_Input input, out string xmlSent, out string xmlRecv, out string error)
    {
      error = string.Empty;
      var result = new Balance_Output();
      XMLLogging logging = null;

      try
      {
        INuCard proxy;
        CreateProxy(url, out proxy, out logging);

        result = proxy.Balance(terminalID: input.terminalID, cardNumber: input.cardNumber, profileNumber: input.profileNumber, transactionID: input.transactionID,
            transactionDate: input.transactionDate, checksum: Hash.GetHMACSHA1(key, input.ToString()));
      }
      catch (Exception err)
      {
        error = string.Format("Atlas/Tutuka comms error- {0}", err.Message);
      }

      xmlRecv = (logging != null) ? logging.XMLDataRecv : string.Empty;
      xmlSent = (logging != null) ? logging.XMLDataSent : string.Empty;

      return result;
    }


    /// <summary>
    /// Function  to transfer funds from card and return them to the profile
    /// </summary>
    /// <param name="url"></param>
    /// <param name="key"></param>
    /// <param name="input">The xml call input parameters</param>
    /// <param name="xmlSent">The raw xml sent</param>
    /// <param name="xmlRecv">The raw xml received</param>
    /// <param name="error"></param>
    /// <returns></returns>
    public static DeductCardLoadProfile_Output DeductCardLoadProfile(string url, string key,
        DeductCardLoadProfile_Input input, out string xmlSent, out string xmlRecv, out string error)
    {
      error = string.Empty;
      var result = new DeductCardLoadProfile_Output();
      XMLLogging logging = null;

      try
      {
        INuCard proxy;
        CreateProxy(url, out proxy, out logging);

        result = proxy.DeductCardLoadProfile(terminalID: input.terminalID, cardNumber: input.cardNumber, profileNumber: input.profileNumber,
            requestAmount: input.requestAmount, transactionID: input.transactionID, transactionDate: input.transactionDate,
            checksum: Hash.GetHMACSHA1(key, input.ToString()));

      }
      catch (Exception err)
      {
        error = string.Format("Atlas/Tutuka comms error- {0}", err.Message);
      }

      xmlRecv = (logging != null) ? logging.XMLDataRecv : string.Empty;
      xmlSent = (logging != null) ? logging.XMLDataSent : string.Empty;

      return result;
    }


    /// <summary>
    /// Function to load a card with funds from a profile
    /// </summary>
    /// <param name="url"></param>
    /// <param name="key"></param>
    /// <param name="input">The xml call input parameters</param>
    /// <param name="xmlSent">The raw xml sent</param>
    /// <param name="xmlRecv">The raw xml received</param>
    /// <param name="error"></param>
    /// <returns></returns>
    public static LoadCardDeductProfile_Output LoadCardDeductProfile(string url, string key,
        LoadCardDeductProfile_Input input, out string xmlSent, out string xmlRecv, out string error)
    {
      error = string.Empty;
      var result = new LoadCardDeductProfile_Output();
      XMLLogging logging = null;

      try
      {
        INuCard proxy;
        CreateProxy(url, out proxy, out logging);

        result = proxy.LoadCardDeductProfile(terminalID: input.terminalID, cardNumber: input.cardNumber, profileNumber: input.profileNumber,
           requestAmount: input.requestAmount, transactionID: input.transactionID, transactionDate: input.transactionDate,
           checksum: Hash.GetHMACSHA1(key, input.ToString()));
      }
      catch (Exception err)
      {
        error = string.Format("Atlas/Tutuka comms error- {0}", err.Message);
      }

      xmlRecv = (logging != null) ? logging.XMLDataRecv : string.Empty;
      xmlSent = (logging != null) ? logging.XMLDataSent : string.Empty;

      return result;
    }


    /// <summary>
    /// Function to link a sequence range of cards to a profile
    /// </summary>
    /// <param name="url"></param>
    /// <param name="key"></param>
    /// <param name="input">The xml call input parameters</param>
    /// <param name="xmlSent">The raw xml sent</param>
    /// <param name="xmlRecv">The raw xml received</param>
    /// <param name="error"></param>
    /// <returns></returns>
    public static LinkCardBySequenceRange_Output LinkCardsBySequenceRange(string url, string key,
        LinkCardBySequenceRange_Input input, out string xmlSent, out string xmlRecv, out string error)
    {
      error = string.Empty;
      var result = new LinkCardBySequenceRange_Output();
      XMLLogging logging = null;

      try
      {
        INuCard proxy;
        CreateProxy(url, out proxy, out logging);

        result = proxy.LinkCardsBySequenceRange(terminalID: input.terminalID, profileNumber: input.profileNumber,
            startSequence: input.startSequence, endSequence: input.endSequence,
            transactionID: input.transactionID, transactionDate: input.transactionDate, checksum: Hash.GetHMACSHA1(key, input.ToString()));
      }
      catch (Exception err)
      {
        error = string.Format("Atlas/Tutuka comms error- {0}", err.Message);
      }

      xmlRecv = (logging != null) ? logging.XMLDataRecv : string.Empty;
      xmlSent = (logging != null) ? logging.XMLDataSent : string.Empty;

      return result;
    }


    /// <summary>
    /// Function to get a statement for a card
    /// </summary>
    /// <param name="url"></param>
    /// <param name="key"></param>
    /// <param name="input">The xml call input parameters</param>
    /// <param name="xmlSent">The raw xml sent</param>
    /// <param name="xmlRecv">The raw xml received</param>
    /// <param name="error"></param>
    /// <returns></returns>
    public static Statement_Output Statement(string url, string key,
        Statement_Input input, out string xmlSent, out string xmlRecv, out string error)
    {
      error = string.Empty;
      var result = new Statement_Output();
      XMLLogging logging = null;

      try
      {
        INuCard proxy;
        CreateProxy(url, out proxy, out logging);
        proxy.Timeout = TIMEOUT_STATEMENT_MS; // Longer for a statement...

        result = proxy.Statement(terminalID: input.terminalID, cardNumber: input.cardNumber, profileNumber: input.profileNumber,
           transactionDate: input.transactionDate, transactionID: input.transactionID, checksum: Hash.GetHMACSHA1(key, input.ToString()));
      }
      catch (Exception err)
      {
        error = string.Format("Atlas/Tutuka comms error- {0}", err.Message);
      }

      xmlRecv = (logging != null) ? logging.XMLDataRecv : string.Empty;
      xmlSent = (logging != null) ? logging.XMLDataSent : string.Empty;

      return result;
    }


    /// <summary>
    /// Function to stop a card
    /// </summary>
    /// <param name="url"></param>
    /// <param name="key"></param>
    /// <param name="input">The xml call input parameters</param>
    /// <param name="xmlSent">The raw xml sent</param>
    /// <param name="xmlRecv">The raw xml received</param>
    /// <param name="error"></param>
    /// <returns></returns>
    public static StopCard_Output StopCard(string url, string key,
        StopCard_Input input, out string xmlSent, out string xmlRecv, out string error)
    {
      error = string.Empty;
      var result = new StopCard_Output();
      XMLLogging logging = null;

      try
      {
        INuCard proxy;
        CreateProxy(url, out proxy, out logging);

        result = proxy.StopCard(terminalID: input.terminalID, cardNumber: input.cardNumber, profileNumber: input.profileNumber,
            stopReasonID: input.stopReasonID, transactionID: input.transactionID, transactionDate: input.transactionDate,
            checksum: Hash.GetHMACSHA1(key, input.ToString()));
      }
      catch (Exception err)
      {
        error = string.Format("Atlas/Tutuka comms error- {0}", err.Message);
      }

      xmlRecv = (logging != null) ? logging.XMLDataRecv : string.Empty;
      xmlSent = (logging != null) ? logging.XMLDataSent : string.Empty;

      return result;
    }


    /// <summary>
    /// Function to transfer funds between two cards
    /// </summary>
    /// <param name="url"></param>
    /// <param name="key"></param>
    /// <param name="input">The xml call input parameters</param>
    /// <param name="xmlSent">The raw xml sent</param>
    /// <param name="xmlRecv">The raw xml received</param>
    /// <param name="error"></param>
    /// <returns></returns>
    public static TransferFunds_Output TransferFunds(string url, string key,
      TransferFunds_Input input, out string xmlSent, out string xmlRecv, out string error)
    {
      error = string.Empty;
      var result = new TransferFunds_Output();
      XMLLogging logging = null;

      try
      {
        INuCard proxy;
        CreateProxy(url, out proxy, out logging);

        result = proxy.TransferFunds(terminalID: input.terminalID, profileNumber: input.profileNumber,
          cardNumberFrom: input.cardNumberFrom, cardNumberTo: input.cardNumberTo, transactionID: input.transactionID,
          requestAmount: input.requestAmount, transactionDate: input.transactionDate, checksum: Hash.GetHMACSHA1(key, input.ToString()));
      }
      catch (Exception err)
      {
        error = string.Format("Atlas/Tutuka comms error- {0}", err.Message);
      }
      xmlRecv = (logging != null) ? logging.XMLDataRecv : string.Empty;
      xmlSent = (logging != null) ? logging.XMLDataSent : string.Empty;

      return result;
    }


    /// <summary>
    /// Function to update the cell phone number for a card
    /// </summary>
    /// <param name="url"></param>
    /// <param name="key"></param>
    /// <param name="input">The xml call input parameters</param>
    /// <param name="xmlSent">The raw xml sent</param>
    /// <param name="xmlRecv">The raw xml received</param>
    /// <param name="error"></param>
    /// <returns></returns>
    public static UpdateAllocatedCard_Output UpdateAllocatedCard(string url, string key,
      UpdateAllocatedCard_Input input, out string xmlSent, out string xmlRecv, out string error)
    {
      error = string.Empty;
      var result = new UpdateAllocatedCard_Output();
      XMLLogging logging = null;

      try
      {
        INuCard proxy;
        CreateProxy(url, out proxy, out logging);

        result = proxy.UpdateAllocatedCard(terminalID: input.terminalID, profileNumber: input.profileNumber,
          cardNumber: input.cardNumber, cellphoneNumber: input.cellphoneNumber, transactionID: input.transactionID,
          transactionDate: input.transactionDate, checksum: Hash.GetHMACSHA1(key, input.ToString()));
      }
      catch (Exception err)
      {
        error = string.Format("Atlas/Tutuka comms error- {0}", err.Message);
      }
      xmlRecv = (logging != null) ? logging.XMLDataRecv : string.Empty;
      xmlSent = (logging != null) ? logging.XMLDataSent : string.Empty;

      return result;
    }


    /// <summary>
    /// Function to get the status information for a card
    /// </summary>
    /// <param name="url"></param>
    /// <param name="key"></param>
    /// <param name="input">The xml call input parameters</param>
    /// <param name="xmlSent">The raw xml sent</param>
    /// <param name="xmlRecv">The raw xml received</param>
    /// <param name="error"></param>
    /// <returns></returns>
    public static Status_Output Status(string url, string key,
      Status_Input input, out string xmlSent, out string xmlRecv, out string error)
    {
      error = string.Empty;
      var result = new Status_Output();
      XMLLogging logging = null;

      try
      {
        INuCard proxy;
        CreateProxy(url, out proxy, out logging);

        result = proxy.Status(terminalID: input.terminalID, profileNumber: input.profileNumber,
          cardNumber: input.cardNumber, transactionID: input.transactionID,
          transactionDate: input.transactionDate, checksum: Hash.GetHMACSHA1(key, input.ToString()));
      }
      catch (Exception err)
      {
        error = string.Format("Atlas/Tutuka comms error- {0}", err.Message);
      }
      xmlRecv = (logging != null) ? logging.XMLDataRecv : string.Empty;
      xmlSent = (logging != null) ? logging.XMLDataSent : string.Empty;

      return result;
    }


    /// <summary>
    /// Function to 'unstop' a stopped card
    /// </summary>
    /// <param name="url"></param>
    /// <param name="key"></param>
    /// <param name="input">The xml call input parameters</param>
    /// <param name="xmlSent">The raw xml sent</param>
    /// <param name="xmlRecv">The raw xml received</param>
    /// <param name="error"></param>
    /// <returns></returns>
    public static CancelStopCard_Output CancelStopCard(string url, string key,
      CancelStopCard_Input input, out string xmlSent, out string xmlRecv, out string error)
    {
      error = string.Empty;
      var result = new CancelStopCard_Output();
      XMLLogging logging = null;

      try
      {
        INuCard proxy;
        CreateProxy(url, out proxy, out logging);

        result = proxy.CancelStopCard(terminalID: input.terminalID, profileNumber: input.profileNumber,
          cardNumber: input.cardNumber, transactionID: input.transactionID,
          transactionDate: input.transactionDate, checksum: Hash.GetHMACSHA1(key, input.ToString()));
      }
      catch (Exception err)
      {
        error = string.Format("Atlas/Tutuka comms error- {0}", err.Message);
      }
      xmlRecv = (logging != null) ? logging.XMLDataRecv : string.Empty;
      xmlSent = (logging != null) ? logging.XMLDataSent : string.Empty;

      return result;
    }


    /// <summary>
    /// Check if the specified amount was deducted from a profile
    /// </summary>
    /// <param name="url"></param>
    /// <param name="key"></param>
    /// <param name="input">The xml call input parameters</param>
    /// <param name="xmlSent">The raw xml sent</param>
    /// <param name="xmlRecv">The raw xml received</param>
    /// <param name="error"></param>
    /// <returns></returns>
    public static CheckAuthorisation_Output CheckAuthorisation(string url, string key,
      CheckAuthorisation_Input input, out string xmlSent, out string xmlRecv, out string error)
    {
      error = string.Empty;
      var result = new CheckAuthorisation_Output();
      XMLLogging logging = null;

      try{
        INuCard proxy;
        CreateProxy(url, out proxy, out logging);

        result = proxy.CheckAuthorisation(terminalID: input.terminalID, profileNumber: input.profileNumber, cardNumber: input.cardNumber,
          requestAmount: input.requestAmount, transactionID: input.transactionID,
          transactionDate: input.transactionDate, checksum: Hash.GetHMACSHA1(key, input.ToString()));
      }
      catch (Exception err)
      {
        error = string.Format("Atlas/Tutuka comms error- {0}", err.Message);
      }

      xmlRecv = (logging != null) ? logging.XMLDataRecv : string.Empty;
      xmlSent = (logging != null) ? logging.XMLDataSent : string.Empty;

      return result;
    }


    /// <summary>
    /// Checks if the specified amount was loaded on a card
    /// </summary>
    /// <param name="url"></param>
    /// <param name="key"></param>
    /// <param name="input">The xml call input parameters</param>
    /// <param name="xmlSent">The raw xml sent</param>
    /// <param name="xmlRecv">The raw xml received</param>
    /// <param name="error"></param>
    /// <returns></returns>
    public static CheckLoad_Output CheckLoad(string url, string key,
      CheckLoad_Input input, out string xmlSent, out string xmlRecv, out string error)
    {
      error = string.Empty;
      var result = new CheckLoad_Output();
      XMLLogging logging = null;

      try{
        INuCard proxy;
        CreateProxy(url, out proxy, out logging);

        result = proxy.CheckLoad(terminalID: input.terminalID, profileNumber: input.profileNumber,
          cardNumber: input.cardNumber, requestAmount: input.requestAmount, transactionID: input.transactionID,
          transactionDate: input.transactionDate, checksum: Hash.GetHMACSHA1(key, input.ToString()));
      }
      catch (Exception err)
      {
        error = string.Format("Atlas/Tutuka comms error- {0}", err.Message);
      }

      xmlRecv = (logging != null) ? logging.XMLDataRecv : string.Empty;
      xmlSent = (logging != null) ? logging.XMLDataSent : string.Empty;

      return result;
    }


    /// <summary>
    /// Request PIN reset
    /// </summary>
    /// <param name="url"></param>
    /// <param name="key"></param>
    /// <param name="input">The xml call input parameters</param>
    /// <param name="xmlSent">The raw xml sent</param>
    /// <param name="xmlRecv">The raw xml received</param>
    /// <param name="error"></param>
    /// <returns></returns>
    public static ResetPin_Output ResetPin(string url, string key,
      ResetPin_Input input, out string xmlSent, out string xmlRecv, out string error)
    {
      error = string.Empty;
      var result = new ResetPin_Output();
      XMLLogging logging = null;

      try
      {
        INuCard proxy;
        CreateProxy(url, out proxy, out logging);

        result = proxy.ResetPin(terminalID: input.terminalID, profileNumber: input.profileNumber,
          cardNumber: input.cardNumber, transactionID: input.transactionID,
          transactionDate: input.transactionDate, checksum: Hash.GetHMACSHA1(key, input.ToString()));
      }
      catch (Exception err)
      {
        error = string.Format("Atlas/Tutuka comms error- {0}", err.Message);
      }

      xmlRecv = (logging != null) ? logging.XMLDataRecv : string.Empty;
      xmlSent = (logging != null) ? logging.XMLDataSent : string.Empty;

      return result;
    }


    /// <summary>
    /// Registers a new profile
    /// </summary>
    /// <param name="url"></param>
    /// <param name="key"></param>
    /// <param name="input"></param>
    /// <param name="xmlSent"></param>
    /// <param name="xmlRecv"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    public static Register_Output Register(string url, string key,
      Register_Input input, out string xmlSent, out string xmlRecv, out string error)
    {
      error = string.Empty;
      var result = new Register_Output();
      XMLLogging logging = null;

      try
      {
        INuCard proxy;
        CreateProxy(url, out proxy, out logging);

        proxy.Timeout = 120000;
        result = proxy.Register(terminalID: input.terminalID, emailAddress: input.emailAddress, password: input.password,
          firstName: input.firstName, lastName: input.lastName, idNumber: input.idNumber, contactNumber: input.contactNumber,
          cellPhoneNumber: input.cellPhoneNumber, isCompany: input.isCompany, vatNumber: input.vatNumber, companyName: input.companyName,
          companyCCNumber: input.companyCCNumber, addressLine1: input.addressLine1, addressLine2: input.addressLine2, city: input.city,
          postalCode: input.postalCode, transactionID: input.transactionID, transactionDate: input.transactionDate, 
          checksum: Hash.GetHMACSHA1(key, input.ToString()));
      }
      catch (Exception err)
      {
        error = string.Format("Atlas/Tutuka comms error- {0}", err.Message);
      }

      xmlRecv = (logging != null) ? logging.XMLDataRecv : string.Empty;
      xmlSent = (logging != null) ? logging.XMLDataSent : string.Empty;

      return result;
    }

    /// <summary>
    /// Get 'clean' error string
    /// </summary>
    /// <param name="resultCode">The function result code</param>
    /// <param name="resultText">The function result message</param>
    /// <returns>string formatted with necessary information</returns>
    public static string GetNuCardErrorString(int? resultCode, string resultText)
    {
      if (!resultCode.HasValue && string.IsNullOrEmpty(resultText))
      {
        return "No response received from supplier";
      }

      string errCode = "*";
      if (resultCode.HasValue)
      {
        switch (resultCode)
        {
          case 0:
            errCode = "No operation was performed";
            break;

          case 1:
            errCode = "OK";
            break;

          case -1:
            errCode = "Tutuka- Invalid merchant number- please contact Atlas IT";
            break;

          case -2:
            errCode = "Tutuka- Invalid card number- please check the card";
            break;

          case -3:
            errCode = "Tutuka- Invalid merchant for card- this card belongs to another region/vendor";
            break;

          case -4:
            errCode = "Tutuka- Card already has been authorised";
            break;

          case -5:
            errCode = "Tutuka- The card has no value / Cancellation has already been requested";
            break;

          case -6:
            errCode = "Tutuka- The card has expired";
            break;

          case -7:
            errCode = "Tutuka- Invalid amount";
            break;

          // Less than minimum load/exceeds max balance

          case -8:
            errCode = "Tutuka- TerminalID not valid or checksum";
            break;

          case -9:
            errCode = "Tutuka- A general error occurred";
            break;

          case -10:
            errCode = "Tutuka- Custom error";
            break;

          case -11:
            errCode = "Tutuka- Source transaction could not be found";
            break;

          case -12:
            errCode = "Tutuka- Card has been designated as 'Lost'";
            break;

          case -13:
            errCode = "Tutuka- Card has been designated as 'Stolen'";
            break;

          case -14:
            errCode = "Tutuka- Insufficient funds";
            break;

          case 15:
            errCode = "Tutuka- Invalid transaction- Electronic transactions only";
            break;

          case -16:
            errCode = "Tutuka- Profile not linked to this terminal";
            break;

          case -17:
            errCode = "Tutuka- Card not linked to this profile";
            break;

          case -18:
            errCode = "Tutuka- Card is not allocated";
            break;

          case -19:
            errCode = "Tutuka- Card already allocated";
            break;

          case -20:
            errCode = "Tutuka- Duplicate transaction";
            break;

          case -21:
            errCode = "Tutuka- The card was stopped";
            break;

          case -22:
            errCode = "Tutuka- The Account already exists/Card already linked";
            break;
                        
          default:
            errCode = string.Format("Tutuka- {0}{1}", resultCode, !string.IsNullOrEmpty(resultText) ? string.Format("- '{0}'", resultText) : string.Empty);
            break;
        }
      }
      else
      {
        errCode = string.Format("Tutuka- {0}", !string.IsNullOrEmpty(resultText) ? resultText : "No information");
      }

      return errCode;
    }
          
    

    #region Private methods

    /// <summary>
    /// Creates a proxy, so request/response logging can be done
    /// </summary>
    /// <param name="url">The endpoint</param>
    /// <param name="proxy">The proxy to create</param>
    /// <param name="xmlLogging">The logging class</param>
    private static void CreateProxy(string url, out INuCard proxy, out XMLLogging xmlLogging)
    {
      proxy = XmlRpcProxyGen.Create<INuCard>();
      proxy.Timeout = TIMEOUT_DEFAULT_MS; // milliseconds      
      //proxy.UseStringTag = false; // !!!!NO!!!!
      proxy.Url = url;
      proxy.UseIndentation = false;

      // Tracing- get the XML...
      xmlLogging = new XMLLogging();
      var tracer = new Tracer(ref xmlLogging);
      tracer.SubscribeTo(proxy);
    }


    #endregion


    #region Private members

    public static readonly int TIMEOUT_DEFAULT_MS = 30000;
    public static readonly int TIMEOUT_STATEMENT_MS = 60000;

    #endregion

  }

  #region Class to store XML between tracing class and calling class

  public class XMLLogging
  {
    public string XMLDataSent { get; set; }
    public string XMLDataRecv { get; set; }
  }

  #endregion


  #region XML tracing

  public class Tracer : XmlRpcLogger
  {
    private XMLLogging _xmlLogging;

    public Tracer()
    {
      throw new Exception("Cannot instantiate without a logger class!");
    }

    public Tracer(ref XMLLogging logging)
    {
      _xmlLogging = logging;
    }

    protected override void OnRequest(object sender, XmlRpcRequestEventArgs e)
    {
      // Make copy of stream to avoid any errors
      byte[] buffer = new byte[e.RequestStream.Length];
      e.RequestStream.Read(buffer, 0, (int)e.RequestStream.Length);

      _xmlLogging.XMLDataSent = Encoding.ASCII.GetString(buffer, 0, buffer.Length);
    }

    protected override void OnResponse(object sender, XmlRpcResponseEventArgs e)
    {
      // Make copy of stream to avoid any errors
      byte[] buffer = new byte[e.ResponseStream.Length];
      e.ResponseStream.Read(buffer, 0, (int)e.ResponseStream.Length);

      _xmlLogging.XMLDataRecv = Encoding.ASCII.GetString(buffer, 0, buffer.Length);
    }
  }

  #endregion
}
