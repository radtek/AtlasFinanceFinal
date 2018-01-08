using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Atlas.Enumerators;


namespace Atlas.Evolution.Server.Code.Utils
{
  public static class EvoStringUtils
  {
    /// <summary>
    /// Cleans name/surname to Evolution specification
    /// </summary>
    /// <param name="name"></param>
    /// <param name="maxLength"></param>
    /// <returns></returns>
    public static string CleanPersonName(string name, int maxLength, int minLength = 0, bool checkVowel = false, bool firstWordOnly = false)
    {
      if (string.IsNullOrWhiteSpace(name))
      {
        return null;
      }

      var cleaned = _nameClean.Replace(name, string.Empty).Trim();

      //AND or CO in the[FORNAME 1] field causes the name to be identified as a company name and subsequently rejected.
      cleaned = Regex.Replace(cleaned, @"\b(AND|CO|MR)\b", string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
      if (string.IsNullOrEmpty(cleaned))
      {
        return null;
      }

      // Only single spaces between words
      cleaned = SingleSpaced(cleaned);
     
      if (firstWordOnly)
      {
        var words = cleaned.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (words.Length > 1)
        {
          cleaned = words.FirstOrDefault(s => s.Length > 2);
          if (string.IsNullOrEmpty(cleaned))
          {
            return null;
          }
        }
      }
      
      // Check contains a vowel
      if (checkVowel && !cleaned.ToUpper().Any(s => s == 'A' || s == 'E' || s == 'I' || s == 'O' || s == 'U' || s == 'Y'))
      {
        return null;
      }
      
      // Check minimum length (2)
      if (minLength > 0 && cleaned.Length < minLength)
      {
        return null;
      }

      return (cleaned.Length > maxLength) ? cleaned.Substring(0, maxLength) : cleaned;
    }


    /// <summary>
    /// Cleans passport to Evolution specification
    /// </summary>
    /// <param name="passport"></param>
    /// <param name="maxLength"></param>
    /// <returns></returns>
    public static string CleanPassport(string passport, int maxLength)
    {
      if (passport != null)
      {
        var cleaned = _passportClean.Replace(passport, string.Empty);
        return (cleaned.Length > maxLength) ? cleaned.Substring(0, maxLength) : cleaned;
      }

      return null;
    }


    /// <summary>
    /// Ensures title matches one of the allowed Evolution titles
    /// </summary>
    /// <param name="title"></param>
    /// <param name="gender"></param>
    /// <returns></returns>
    public static string CleanTitle(string title, General.Gender gender)
    {
      if (title != null)
      {
        var cleaned = _titleClean.Replace(title, string.Empty).Trim();
        if (_noGenderTitles.Contains(cleaned))
        {
          return cleaned;
        }

        switch (gender)
        {
          case General.Gender.Female:
            return _femaleTitles.Contains(cleaned) ? cleaned : "MS";

          case General.Gender.Male:
            return _maleTitles.Contains(cleaned) ? cleaned : "MR";
        }
      }

      return (gender == General.Gender.Male) ? "MR" : "MS";
    }


    /// <summary>
    /// Ensure address line meets Evolution requirements
    /// </summary>
    /// <param name="address"></param>
    /// <param name="maxLength"></param>
    /// <returns></returns>
    public static string CleanAddress(string address, int maxLength, int minLength = -1)
    {
      if (address != null)
      {
        var cleaned = _addressClean.Replace(address, string.Empty).Trim();
        if (minLength > 0 && cleaned.Length < minLength)
        {
          return null;
        }

        // Must not contain words 'TA' 'NA'
        cleaned = Regex.Replace(cleaned, @"\b(NA|TA)\b", string.Empty);

        // Only single spaces between words
        cleaned = SingleSpaced(cleaned);
                        
        return (cleaned.Length > maxLength) ? cleaned.Substring(0, maxLength) : cleaned;
      }
      return null;
    }


    /// <summary>
    /// Clean postal address code
    /// </summary>
    /// <param name="postalCode"></param>
    /// <param name="maxLength"></param>
    /// <returns></returns>
    public static string CleanPostalCode(string postalCode, int maxLength = 6)
    {
      if (postalCode != null)
      {
        var cleaned = _postalCodeClean.Replace(postalCode, string.Empty).Trim();
        if (cleaned.Length < 4)
        {
          return null;
        }

        if (cleaned == "0000")
        {
          return null;
        }

        return (cleaned.Length > maxLength) ? cleaned.Substring(0, maxLength) : cleaned;
      }

      return null;
    }


    /// <summary>
    /// Clean telephone number
    /// </summary>
    /// <param name="telephone"></param>
    /// <returns></returns>
    public static string CleanTelephone(string telephone)
    {
      if (telephone != null)
      {
        var cleaned = _telephoneClean.Replace(telephone, string.Empty);

        if (cleaned.Length == 10) // check for 7 repeated digits, i.e. 011 555-5555
        {
          var uniqueChars = cleaned.Substring(3).ToArray().Distinct().Count();
          if (uniqueChars == 1) // all duplicates
          {
            return null;
          }
        }
        return (cleaned.Length == 10) ? cleaned : null;
      }

      return null;
    }


    public static string CleanCompanyName(string company, int maxLength)
    {
      if (company != null)
      {
        var cleaned = _companyNameClean.Replace(company, string.Empty);

        // Common errors: (PTY0 9PTY)
        cleaned = cleaned.Replace("(PTY0", " (PTY) ").Replace("9PTY)", " (PTY) ").Replace("(PTY LTD", " (PTY) LTD ");

        // Check parenthesis are well-formed/balanced...
        if (!CheckIsWellFormed(cleaned))
        {
          cleaned = cleaned.Replace(")", " ").Replace("(", " ");
        }

        // Must not start with 'TA '
        if (cleaned.StartsWith("TA ", StringComparison.OrdinalIgnoreCase))
        {
          cleaned = cleaned.Substring(3);
        }

        // Only single spaces between words
        //cleaned = Regex.Replace(cleaned, @"(\s)\s+", "$1");
        cleaned = SingleSpaced(cleaned);

        return (cleaned.Length > maxLength) ? cleaned.Substring(0, maxLength) : cleaned;
      }

      return null;
    }


    public static bool IsPositiveOutcome(string evoClosedStatusCode)
    {
      var code = evoClosedStatusCode.Trim().ToUpper();
      return _positiveOutcomeResultCodes.Contains(evoClosedStatusCode);
    }


    public static string CleanOccupation(string occupation, int maxLength)
    {
      if (string.IsNullOrWhiteSpace(occupation))
      {
        return null;
      }

      var cleaned = _occupationClean.Replace(occupation, string.Empty);
      return (cleaned.Length > maxLength) ? cleaned.Substring(0, maxLength) : cleaned;
    }

    /// <summary>
    /// Checks string contains balanced parenthesis
    /// </summary>
    /// <param name="input">string to check</param>
    /// <returns>true if string contains balance parenthesis, or no parenthesis, or is an empty string. 
    /// false if string contains unbalanced parenthesis, i.e. '...)...(...' or '...(...'</returns>
    //http://codereview.stackexchange.com/questions/67602/check-for-balanced-parentheses

    private static bool CheckIsWellFormed(string input)
    {
      if (string.IsNullOrEmpty(input))
      {
        return true;
      }

      var stack = new Stack<char>();
      // dictionary of matching starting and ending pairs
      var allowedChars = new Dictionary<char, char>() { { '(', ')' } };

      var wellFormated = true;
      foreach (var chr in input)
      {
        if (allowedChars.ContainsKey(chr))
        {
          // if starting char then push on stack
          stack.Push(chr);
        }
        // ContainsValue is linear but with a small number is faster than creating another object
        else if (allowedChars.ContainsValue(chr))
        {
          // make sure something to pop if not then know it's not well formated
          wellFormated = stack.Any();
          if (wellFormated)
          {
            // hit ending char grab previous starting char
            var startingChar = stack.Pop();
            // check it is in the dictionary
            wellFormated = allowedChars.Contains(new KeyValuePair<char, char>(startingChar, chr));
          }
          // if not well-formatted: exit loop no need to continue
          if (!wellFormated)
          {
            break;
          }
        }
      }

      if (stack.Count > 0) // We have one or more  opening delimiters without a closing match... not well-formatted
      {
        wellFormated = false;
      }

      return wellFormated;
    }


    private static string SingleSpaced(string input)
    {
      if (string.IsNullOrWhiteSpace(input))
      {
        return string.Empty;
      }

      return string.Join(" ", input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
    }

    // <summary>
    // Account numbers regular expression
    // </summary>
    //private readonly static Regex _accountValid = new Regex(@"^[A-Za-z0-9\\/\-]*$");


    // <summary>
    // Name fields regular expression
    // </summary>
    //
    //private readonly static Regex _nameValid = new Regex(@"^[A-Za-z \'\-]*$");

    /// <summary>
    /// Characters to remove from names
    /// </summary>
    private readonly static Regex _nameClean = new Regex(@"[^A-Za-z \'\-]");

    /// <summary>
    /// Characters to remove from person title field
    /// </summary>
    private readonly static Regex _titleClean = new Regex(@"[^A-Za-z]");

    // <summary>
    // Address lines regular expression
    // </summary>
    //private readonly static Regex _addressValid = new Regex(@"^[A-Za-z0-9 \'\-]*$");

    /// <summary>
    /// Characters to remove from address lines
    /// </summary>
    private readonly static Regex _addressClean = new Regex(@"[^A-Za-z0-9 \'\-]");

    // <summary>
    // Characters to remove from Company name
    // </summary>
    private readonly static Regex _companyNameClean = new Regex(@"[^A-Za-z\d,\.\-() ;!%\+&#=]");

    //private readonly static Regex _companyNameDefault = new Regex(@"^[A-Za-z\d,\.'\-() ;!%\+&#=]+$");

    /// <summary>
    /// Characters to remove from postal codes
    /// </summary>
    private readonly static Regex _postalCodeClean = new Regex(@"[^0-9]");

    /// <summary>
    /// Characters to remove from passports
    /// </summary>
    private readonly static Regex _passportClean = new Regex(@"[^0-9A-Za-z]");

    /// <summary>
    /// Characters to remove from telephones
    /// </summary>
    private readonly static Regex _telephoneClean = new Regex(@"[^0-9]");

    /// <summary>
    /// Characters to remove from occupation
    /// </summary>
    private readonly static Regex _occupationClean = new Regex(@"[^A-Za-z\d'\-]");
   
    /// <summary>
    /// Allowed titles
    /// </summary>
    /// 
    private readonly static List<string> _noGenderTitles = new List<string> {
      "ADV", "CAPT", "COL", "DR", "DS", "JUDGE", "KAPT", "KOL", "LT", "MAJ", "PAST", "PROF", "REV", "SERS", "SGT" };

    private readonly static List<string> _femaleTitles = new List<string> { "ME", "MEJ", "MEV", "MISS", "MRS", "MS", "LADY" };

    private readonly static List<string> _maleTitles = new List<string> { "MNR", "MR", "LORD", "SIR" };

    private readonly static List<string> _positiveOutcomeResultCodes = new List<string> {
       "C", "F", "G", "H", "K", "P", "S", "T", "V", "Z"};
  }
}
