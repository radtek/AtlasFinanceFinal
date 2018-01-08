using System;


namespace Atlas.Evolution.Server.Code.Utils
{
  /// <summary>
  /// Class to map ass to Evolution equivalent
  /// </summary>
  public static class EvoMapping
  {
    /// <summary>
    /// Convert an ASS loan reason (loans.lr_popup -> popup.popup_type to Evolution loan reason
    /// </summary>
    /// <param name="assLoanReason">The loans.lr_popup value</param>
    /// <returns>THe equivalent Evolution </returns>
    internal static string LoanReasonAssToEvolution(string assLoanReason)
    {
      switch (assLoanReason.ToUpper().Trim())
      {
        case "HOU": // Housing 
          return "H"; // New property acquisition or upgrades to existing property

        case "FUR": // Furniture
          return "F"; // Financing of fixed or moveable asset other than property

        case "SML": // Small Business    
          return "J"; // A loan to a sole proprietor

        case "EDU": // Education 
          return "S"; // Loan to fund formal studies at a recognised institution

        case "CON": // Debt Consolidation
          return "R"; // A loan resulting from the Debt Consolidation

        case "DED": // Death/Funeral
          return "D"; // Loan granted to overcome client cash flow problems during unforeseen circumstances: Death / Funeral

        case "MED": // Medical
          return "E"; // Loan granted to overcome client cash flow problems during unforeseen circumstances: Medical

        case "INC": // Income loss
          return "G"; // Loan granted to overcome client cash flow problems during unforeseen circumstances: Income Loss

        case "THE": // Theft or Fire
          return "I"; // Loan granted to overcome client cash flow problems during unforeseen circumstances: Loss - Theft or Fire

        case "EME": // Emergency
          return "C"; // Loan granted to overcome client cash flow problems during unforeseen circumstances: Other Emergency

        case "SER": // Service
        case "OTH": // Other Reason
        default:
          return "O"; // A loan other than the ones stipulated above
      }
    }
    

    /// <summary>
    /// Freqency to Evolution
    /// </summary>
    /// <param name="loanmeth"></param>
    /// <returns></returns>
    internal static string FrequencyAssToEvolution(string loanmeth)
    {
      switch (loanmeth.Trim())
      {
        case "W":
          return "W";

        case "B":
          return "F";

        case "M":
          return "M";

        default:
          return "M ";
      }
    }


    /// <summary>
    /// for closed ass (outamt less than R100)
    /// </summary>
    /// <param name="assLoanStatus"></param>
    /// <returns></returns>
    internal static string ClosedLoanStatusAssToEvoStatusCode(string assLoanStatus)
    {
      switch (assLoanStatus.Trim())
      {
        case "A": //  A = Adjustment - check outamt
          return "C"; // C- Account fully paid and has been closed

        case "C": //  C = Cancelled
          return "V"; // Loan settled within the 5 day cooling off period

        case "D": // D = Discounted
          return "C"; // C- Account fully paid and has been closed

        case "E": // E = Early Settled
          return "T"; // T- Outstanding balance settled before agreed term"

        case "F":  // F = Fully paid up loan and closed
          return "C"; // C- Account fully paid and has been closed

        case "H":  // H = Handed over
          return "L"; // L- Account handed over to attorney or collection agency for recovery

        case "J":  // J = Journalised - check outamt
          return "C"; // C- Account fully paid and has been closed

        case "N":  //  N = New Loan without any transactions
          return "C"; // C- Account fully paid and has been closed !!!!!!!!!!!!!!!???????????????????????

        case "P":  // P = Part Payment
          return "C"; // C- Account fully paid and has been closed !!!!!!!!!!!!!!!???????????????????????

        case "R":  // R = Refunded
          return "C"; // C- Account fully paid and has been closed

        case "S":  // S = ReScheduled
          return "C"; // C- Account fully paid and has been closed

        case "W":  //  W = Written Off - see trans.reason for specifics
          return "T"; // T- Early settlement

        default:
          return "C";
      }
   
      //[Description("Repayment terms have been extended")]
      //[ShortCode("E")]
            
      //  [Description(")]
      //[ShortCode("W")]
      //WrittenOff = 10,

      //  [Description("Where a consumer has been confirmed as deceased")]
      //[ShortCode("Z")]
      //Deceased = 11
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="payFrequency"></param>
    /// <returns></returns>
    internal static uint FrequencyAssToEvoRepayFreq(string payFrequency)
    {
      switch (payFrequency.Trim())
      {
        case "W":
          return 1;

        case "B":
          return 2;

        default:
          return 3;
      }
    }
    

    /// <summary>
    /// 
    /// </summary>
    /// <param name="frequency"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    internal static uint InstallmentPerMonth(string frequency, decimal amount)
    {
      switch (frequency.Trim())
      {
        case "W":
          return (uint)amount * 4;

        case "B":
          return (uint)amount * 2;

        default:
          return (uint)amount;
      }
    }
    
  }
}
