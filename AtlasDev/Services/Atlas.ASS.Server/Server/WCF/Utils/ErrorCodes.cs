using System;

using Atlas.Common.Interface;


namespace Atlas.Server.WCF.Utils
{
  internal static class ErrorCodes
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    internal static string GetUpfrontBinCheckErrorString(ILogging log, string code)
    {
      switch (code)
      {
        case "00":
          return string.Empty;

        case "01":
          return "Merchant number and terminalID supplied do not match";

        //case "05":
        //    return "Terminal is busy with another function- either terminal is currently in use, or needs to be reset.";

        case "06":
          return "Unable to connect to server to registration request";

        case "08":
          return "Terminal not connected";

        case "05":
        case "11":
          return "The card is NOT in the BIN table and is not eligible";

        default:
          log.Warning("GetUpfrontBinCheckErrorString called with unexpected code {Code}", code);
          return string.Format("Unknown/unexpected result- '{0}'", code);
      }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    internal static string GetHandShakeErrorString(ILogging log, string code)
    {
      code = code.Trim();
      if (code.Length > 2)
      {
        code = code.Substring(0, 2);
      }

      switch (code)
      {
        case "01":
          return "There appears to be a database mismatch- the terminal/merchant code does not match. Please contact Atlas Head Office Operations department, to have them correct the problem!";

        case "05":
          return "The TCC terminal is busy with another function- either terminal is currently in use, or needs to be reset.";

        case "06":
          return "NuPay TCC terminal communications error. Please contact Atlas Head Office Operations department, to have them attend to the problem with supplier!";

        case "08":
          return "The NuPay TCC terminal does not appear to be online. Please contact the Atlas IT department, to have them assist you.";

        default:
          log.Warning("GetHandShakeErrorString called with unexpected code {Cpde}", code);
          return string.Format("The 'NuPay Terminal control' service returned an unknown response code of: '{0}'. Please contact Atlas IT department with this code.", code);
      }
    }


    /// <summary>
    /// Get English version of TCC error code
    /// </summary>
    /// <param name="code">Code from TCC</param>
    /// <returns>English error message</returns>
    internal static string GetAEDOTerminalAuthoriseErrorString(ILogging log, string code)
    {
      code = code.Trim();
      if (!string.IsNullOrEmpty(code) && code.Length > 1)
      {
        if (code.ToUpper().Contains("TRANSACTION CANCELLED"))
        {
          return "The Transaction was cancelled by the terminal operator";
        }

        // Get first word
        code = code.Trim(new char[] { ' ', '[', ']' });
        var words = code.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        if (words.Length > 0)
        {
          switch (words[0].Trim())
          {
            case "01":
              return "[01]- Refer to card issuer- some kind of unexpected banking error occurred. Please contact Atlas IT";

            case "02":
              return "[02]- Refer to card issuer- some kind of unexpected banking error occurred. Please contact Atlas IT";

            case "03":
              return "[03]- Communication between the TCC terminal and Altech timed out- reset the TCC terminal";

            case "04":
              return "[04]- Client to collect new card from bank";

            case "05":
              return "[05]- TCC terminal error or bank card damaged- reset TCC & if error persists, replace card";

            case "06":
              return "[06]- TCC Terminal appears to be offline- please reset TCC terminal";

            //case "06": ???????????????????????????????
            //  return "[06]- Client to collect new card from bank";

            case "08":
              return "[08]- Terminal not connected- please reset the TCC terminal";

            case "09":
              return "[09]- Wrong card was swiped and/or bank details do not match those entered in ASS";

            case "12":
              return "[12]- Card is chipped- only NAEDO transactions are allowed";

            case "13":
              return "[13]- Invalid amount";

            case "14":
              return "[14]- Invalid card number";

            case "30":
              return "[30]- Data format error- Contact Atlas HO- there appears to be database corruption or network issues";

            case "35":
              return "[35]- PIN tries exceeded";

            case "37":
              return "[37]- PIN tries exceeded";

            case "41":
              return "[41]- Card reported lost";

            case "43":
              return "[43]- Card reported stolen";

            case "52":
              return "[52]- Incorrect account type";

            case "53":
              return "[53]- Incorrect account type";

            case "54":
              return "[54]- Card has expired";

            case "57":
              return "[57]- Transaction not permitted to card holder- card cannot be used for debit orders";

            case "62":
              return "[62]- Card restricted by issuing bank, or account type *may* be wrong. If you selected 'Savings' account type, please try using 'Cheque'. If the error persists, please contact the issuing bank for more information as to why this card is restricted";

            case "68":
              return "[68]- Response received too late. Please retry.";

            case "75":
              return "[75]- PIN tries exceeded";

            case "91":
              return "[91]- Altech/ABSA communication problem (issuer or switch inoperative)- please retry in 5 minutes";

            case "99":
              return "[99]- General TCC exception- please see TCC";

            case "101":
              return "[101]- NAEDO error- The transaction cannot be loaded for this action date and frequency combination- please check the payment plan schedule";

            case "102":
              return "[102]- NAEDO error- The transaction failed the CDV check- please check the following in ASS are correct: card number/bank account/bank branch";

            case "103":
              return "[103]- No account number supplied to web service- check your ASS is up-to-date and run database corruption checks";

            case "104":
              return "[103]- AEDO and NAEDO merchant numbers do not match- please contact Atlas IT department";

            case "105":
              return "[105]- Duplicate transaction found- please contact Atlas IT/logistics";

            case "106":
              return "[106]- TCC Terminal requires replacement- please contact Atlas IT ";

            case "107":
              return "[107]- Instalment exceeded R5 000- please contact Atlas IT, this error should not occur";

            case "108":
              return "[108]- Card number mismatch- please check the following in ASS are correct: card number/bank account/bank branch";

            case "112":
              return "[112]- Terminal not responding to handshake. Please reset the TCC terminal and retry and if problem persists, contact Atlas IT";

            case "113":
              return "[113]- NAEDO Error- Invalid number of instalments for Frequency";

            case "114":
              return "[114]- NAEDO Error- Invalid Tracking code";

            case "115":
              return "[115]- NAEDO Error- Invalid merchant account";

            case "116":
              return "[116]- Invalid bank account type- please change the bank account type for the client in ASS";

            case "117":
              return "[117]- Printer out of paper and not changed within timeout";

            case "118":
              return "[118]- User did not complete transaction- no terminal interaction within timeout";

            case "199":
              return "[199]- NuPay NAEDO time-out (Altech NuPay database error)- please try again and use manual swipe system if error persists";

            case "1001":
              return "[1001]- Merchant/TerminalId not authorised";

            default:
              log.Warning("GetAEDOTerminalAuthoriseErrorString called with unexpected result '{Code}'", code);
              return string.Format("[{0}]- Unknown/unexpected result from the Altech TCC system", code.Substring(0, Math.Min(code.Length, 100)));
          }
        }
      }

      return "No code";
    }


    /// <summary>
    /// Returns error string for GenericDataCapture
    /// </summary>
    /// <param name="code">TCC Result code</param>
    /// <returns>english error description</returns>
    internal static string GetGenericDataCapture(ILogging log, string code)
    {
      switch (code)
      {
        case "00":
          return string.Empty;

        case "04":
          return "User cancelled transaction";

        case "05":
          return "Terminal currently busy with another function";

        default:
          log.Warning("GetGenericDataCapture called with unexpected code {Code}", code);
          return string.Format("Unknown result code {0}", code.Substring(0, Math.Min(code.Length, 100)));
      }
    }


    /// <summary>
    /// Returns error code for AEDO result
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    internal static string GetAEDOErrorString(string code)
    {
      if (string.IsNullOrEmpty(code))
      {
        return "Empty result- no result!";
      }

      if (code == "0" || code == "00")
      {
        return "Successful";
      }

      code = code.TrimStart('0');
      switch (code)
      {
        case "1":
          return "[1]- Check file type in filename";

        case "2":
          return "[2]- Check record type in file";

        case "3":
          return "[3]- Check hash total";

        case "4":
          return "[4]- File error";

        case "5":
          return "[5]- Check all record lengths";

        case "6":
          return "[6]- Check record a number field has invalid characters in";

        case "7":
          return "[7]- Phone the Altech NuPay call centre";

        case "8":
          return "[8]- Check file a date field has invalid data";

        case "9":
          return "[9]- Check file a time field has invalid data";

        case "11":
          return "[11]- Check file a employer code field has invalid data";

        case "12":
          return "[12]- Phone the Altech NuPay call centre";

        case "13":
          return "[13]- Phone the Altech NuPay call centre";

        case "14":
          return "[14]- Check that the file is in the zip file";

        case "15":
          return "[15]- Phone the Altech NuPay call centre";

        case "16":
          return "[16]- Please login and retry";

        case "17":
          return "[17]- Please wait";

        case "18":
          return "[18]- Please wait";

        case "19":
          return "[19]- Retry upload or phone call centre";

        case "20":
          return "[20]- Check merchant/group number in record";

        case "21":
          return "[21]- Phone the Altech NuPay call centre";

        case "22":
          return "[22]- Phone the Altech NuPay call centre";

        case "23":
          return "[23]- Phone the Altech NuPay call centre";

        case "24":
          return "[24]- Phone the Altech NuPay call centre";

        case "25":
          return "[25]- Login and retry";

        case "26":
          return "[26]- Check transaction id in record";

        case "27":
          return "[27]- Check transaction id in record";

        case "28":
          return "[28]- No rights to cancel transaction or transaction in run";

        case "29":
          return "[29]- Maximum resubmissions exceeded for transaction";

        case "30":
          return "[30]- Last success date is more than three months ago";

        case "31":
          return "[31]- User has no rights to do transaction maintenance";

        case "32":
          return "[32]- Transaction maintenance sql processing error";

        case "33":
          return "[33]- Day of month 1/2";

        case "34":
          return "[34]- Day of week";

        case "35":
          return "[35]- Month of year";

        case "36":
          return "[36]- Day of month rule";

        case "37":
          return "[37]- Date adjustment rule";

        case "38":
          return "[38]- Re-presentment frequency rule";

        case "39":
          return "[39]- Day of month 1 > 2";

        case "40":
          return "[40]- Invalid frequency";

        case "41":
          return "[41]- Timeout awaiting response from bankserv";

        case "42":
          return "[42]- Invalid header";

        case "43":
          return "[43]- ADO error from BankServ";

        case "44":
          return "[44]- Invalid record length (header)";

        case "45":
          return "[45]- Invalid record length (default data fields)";

        case "46":
          return "[46]- Undefined";

        case "47":
          return "[47]- Invalid contract type";

        case "48":
          return "[48]- Invalid acquiring bank";

        case "49":
          return "[49]- Invalid cps code";

        case "50":
          return "[50]- Invalid request / modify date";

        case "51":
          return "[51]- Timeout";

        case "52":
          return "[52]- Frequency error";

        case "53":
          return "[53]- Default error";

        case "54":
          return "[54]- Instalment amount <> last instalment amount";

        case "55":
          return "[55]- Incomplete";

        case "56":
          return "[56]- Contract amount mismatched";

        case "57":
          return "[57]- Service type mismatched";

        case "58":
          return "[58]- No instalment for specified date";

        case "59":
          return "[59]- Instalment already processed";

        case "60":
          return "[60]- Invalid employer code";

        case "61":
          return "[61]- Transaction not found (ma_tran_amounts)";

        case "62":
          return "[62]- Transaction processed (ma_failed_tran, ma_success_tran)";

        case "63":
          return "[63]- Negative response form BankServ";

        case "64":
          return "[64]- No action taken";

        case "65":
          return "[65]- Contract registered at BankServ, no record at NuPay. action - cancel at BankServ";

        case "66":
          return "[66]- Invalid option for service type";

        case "67":
          return "[67]- Invalid service type";

        case "68":
          return "[68]- Cancellation - ISO 25 - transaction not found, not own by card acceptor/super user";

        case "69":
          return "[69]- Cancellation - ISO 28 - owner request invalid rights, tran locked";

        case "70":
          return "[70]- Transaction in online run/batch run";

        case "71":
          return "[71]- Invalid AEDO date (date out of valid range < start date) instalment";

        case "72":
          return "[72]- Invalid AEDO date (date out of valid range > end date) instalment";

        case "73":
          return "[73]- Date out of valid range (SQL)";

        case "74":
          return "[74]- Cannot reschedule contract instalments that already run/failed";

        case "75":
          return "[75]- Contract already active";

        case "76":
          return "[76]- Maximum retries (original code 83)";

        case "77":
          return "[77]- Maximum last success date (original code 84)";

        case "78":
          return "[78]- Contract not active";

        case "79":
          return "[79]- Invalid tracking parameter";

        case "80":
          return "[80]- Frequency change applied - date change failed";

        case "81":
          return "[81]- Frequency change timeout pending response from BS - date change not attempted";

        case "82":
          return "[82]- Frequency change applied - date change timed out (pending response)";

        case "83":
          return "[83]- Invalid reason code (specific reason code cannot be resubmitted";

        case "84":
          return "[84]- Instalment already resubmitted";

        case "85":
          return "[85]- Resubmission logged";

        case "86":
          return "[86]- Resubmission log failed";

        case "87":
          return "[87]- Multiple resubmissions on one instalment";

        case "88":
          return "[88]- Unknown type (resubmission error)";

        case "89":
          return "[89]- Resubmission failed";

        case "90":
          return "[90]- Service temporarily unavailable";

        case "91":
          return "[91]- Transaction locked pending response from bankserv";

        case "92":
          return "[92]- ABSA AEDO 12:00 cutoff";

        case "93":
          return "[93]- Cancellation of resubmitted AEDO transaction held by bankserv not allowed";

        case "95":
          return "[95]- Change type not available on resubmissions";

        case "96":
          return "[96]- Too many instalments in payment period";

        case "97":
          return "[97]- Logged for AEDO batch processing";

        case "98":
          return "[98]- No changes allowed past time limit";

        case "94":
          return "[94]- This feature is currently unavailable";

        case "99":
          return "[99]- Next Date for Collection too far in future ";

        case "R0601":
          return "[R0601]- BankServ- Contract not found";

        case "A0":
          return "Issuing Bank is not a 7 day processor";

        case "A1":
          return "Date Adjusted instalment date in contravention of PCH rule that allows only 2 payment instructions per payment cycle";

        case "A2":
          return "Instalment not At Issuer / Instalment not In Tracking";

        case "A3":
          return "Duplicate Recall Request";

        case "A4":
          return "Instalment has already been Recalled";

        case "A5":
          return "Recall Request has been logged";

        case "E8":
          return "​Successful recall";

        case "E9":
          return "​Unsuccessful recall";

        default:
          return string.Format("[{0}]- Unknown/undocumented AEDO result code", code);
      }
    }


    /// <summary>
    /// Returns error code for NAEDO result
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    internal static string GetNAEDOErrorString(string code)
    {
      code = code.TrimStart('0');
      switch (code)
      {
        case "0":
          return "[0]-Success";

        case "2":
          return "[2]-Not Provided For";

        case "3":
          return "[3]-Debit not allowed to this account";

        case "4":
          return "[4]-Payment stopped by Account Holder";

        case "5":
          return "[5]-Account Dormant";

        case "6":
          return "[6]-Account frozen";

        case "7":
          return "[7]-Phone Nupay Call Centre";

        case "8":
          return "[8]-Account in sequestration";

        case "9":
          return "[9]-Phone Nupay Call Centre";

        case "10":
          return "[10]-Account In Liquidation";

        case "12":
          return "[12]-Account closed";

        case "16":
          return "[16]-Check Username and Password";

        case "17":
          return "[17]-Cancellation by merchant";

        case "18":
          return "[18]-Account holder deceased";

        case "20":
          return "[20]-Check Card Acceptor / Group Id / Subgroup Id";

        case "22":
          return "[22]-Account effects not cleared";

        case "25":
          return "[25]-Check Transaction Id and Card Acceptor";

        case "26":
          return "[26]-No Such Account";

        case "27":
          return "[27]-Check Transaction Id";

        case "30":
          return "[30]-No authority to debit";

        case "31":
          return "[31]-Phone Nupay Call Centre";

        case "32":
          return "[32]-Debit in contravention of payers authority";

        case "34":
          return "[34]-Authorisation canceled..";

        case "36":
          return "[36]-Previously stopped via stop payment advice";

        case "40":
          return "[40]-Item limit exceeded";

        case "42":
          return "[42]-AEDO MAC verification failure";

        case "44":
          return "[44]-Unable to process";

        case "46":
          return "[46]-Account in advance";

        case "48":
          return "[48]-Account number fails CDV routine";

        case "50":
          return "[50]-Rejected on load";

        case "54":
          return "[54]-Invalid Instalment Amount (> 10000.00)";

        case "56":
          return "[56]-Not FICA Compliant";

        case "58":
          return "[58]-no instalment for specified date";

        case "59":
          return "[59]-instalment already processed";

        case "60":
          return "[60]-instalment not pending";

        case "61":
          return "[61]-AEDO Conversion Error";

        case "70":
          return "[70]-Transaction in online run";

        case "71":
          return "[71]-invalid date- submit_date < start_date";

        case "75":
          return "[75]-Contract Already Active";

        case "76":
          return "[76]-no pending instalments";

        case "78":
          return "[78]-invalid frequency";

        case "79":
          return "[79]-Invalid Tracking Indicator";

        case "92":
          return "[92]-invalid submit_date, submit_date < next valid processing date";

        case "97":
          return "[97]-Upfront rejection";

        case "99":
          return "[99]-Held for Representment";

        case "E1":
          return "[E1]-Payer request to stop presentations";

        case "E8":
          return "[E8]-Successful Recall";

        case "E9":
          return "[E9]-Unsuccessful Recall";

        case "F0":
          return "[F0]-Transaction Failed in Validation";

        default:
          return string.Format("[{0}]- Unknown/undocumented result", code);
      }
    }


    /// <summary>
    /// Gets english description for TCC TransIDQuery web method
    /// </summary>
    /// <param name="code">Code from web method</param>
    /// <returns>English description of error</returns>
    internal static string GetTranIDQueryErrorString(ILogging log, string code)
    {
      switch (code)
      {
        case "00":
          return string.Empty;

        case "80":
          return string.Empty;      /// No transaction ID exists

        default:
          log.Warning("GetTranIDQueryErrorString called with unexpected code {Code}", code);
          return string.Format("Unknown/unexpected result- '{0}'", code);
      }
    }


    /// <summary>
    /// Determines if TCC error code indicates that the terminal is now probably off-line
    /// </summary>
    /// <param name="code">TCC return code from Aedo request</param>
    /// <returns>true if terminal is probably offline/needs to be reset</returns>
    internal static bool DoesAEDOErrCodeMeanTerminalOffline(string code)
    {
      code = code != null ? code.TrimEnd(new char[] { ' ', '0' }): null;
      if (!string.IsNullOrEmpty(code) && code.Length > 1)
      {
        if (code.Equals("Transaction Cancelled"))
        {
          return false;
        }

        switch (code)
        {
          case "03":
          case "05":
          case "06":
          case "08":
          //case "91":
          case "106":
          case "112":
          case "118":
            return true;

          default:
            return false;
        }
      }
      else
      {
        return false; // ???
      }
    }

  }
}
