using System;
using System.Text;
using System.Text.RegularExpressions;


namespace Atlas.Common.Utils
{
  /// <summary>
  /// Validates a South African identity number.
  /// </summary>
  public class IDValidator
  {
    // constants
    const int VALID_LENGTH = 13;
    const int CONTROL_DIGIT_LOCATION = 12;
    const int CONTROL_DIGIT_CHECK_VALUE = 10;
    const int CONTROL_DIGIT_CHECK_EXCEPTION_VALUE = 9;
    const string REGEX_ID_PATTERN = "(?<Year>[0-9][0-9])(?<Month>([0][1-9])|([1][0-2]))(?<Day>([0-2][0-9])|([3][0-1]))(?<Gender>[0-9])(?<Series>[0-9]{3})(?<Citizenship>[0-9])(?<Uniform>[0-9])(?<Control>[0-9])";
    const bool VALID = true;
    const bool INVALID = false;

    // member variables
    private readonly string _id;

    // constructor
    public IDValidator(string id)
    {
      _id = id;
    }

    public int GetAge()
    {
      if (isValid())
      {
        var birthDate = DateTime.ParseExact(string.Format("{0}/{1}/{2}", _id.Substring(0, 2), _id.Substring(2, 2), _id.Substring(4, 2)), "yy/MM/dd", System.Globalization.CultureInfo.InvariantCulture);
        var years = DateTime.Now.Year - birthDate.Year;

        if (years < 0)
        {
          birthDate = DateTime.ParseExact(string.Format("19{0}/{1}/{2}", _id.Substring(0, 2), _id.Substring(2, 2), _id.Substring(4, 2)), "yyyy/MM/dd", System.Globalization.CultureInfo.InvariantCulture);
          years = DateTime.Now.Year - birthDate.Year;
        }

        if (DateTime.Now.Month < birthDate.Month ||
          (DateTime.Now.Month == birthDate.Month &&
          DateTime.Now.Day < birthDate.Day))
          years--;

        return years;
      }
      else
      {
        throw new Exception("Invalid ID");
      }
    }


    // SA citizen check
    public bool IsSACitizen()
    {
      if (isValid())
      {
        return int.Parse(_id.Substring(10, 1)) == 0 ? true : false;
      }

      return false;      
    }


    // gender check
    public bool IsFemale()
    {
      if (isValid())
      {
        return int.Parse(_id.Substring(6, 1)) < 5 ? true : false;
      }
      
      return false;      
    }


    // get date of birth
    public string GetDateOfBirth()
    {
      if (isValid())
      {
        var date = DateTime.ParseExact(string.Format("{0}/{1}/{2}", _id.Substring(0, 2), _id.Substring(2, 2),
          _id.Substring(4, 2)), "yy/MM/dd", System.Globalization.CultureInfo.InvariantCulture);
        var years = DateTime.Now.Year - date.Year;
        if (years < 0)
        {
          date = DateTime.ParseExact(string.Format("19{0}/{1}/{2}", _id.Substring(0, 2), _id.Substring(2, 2), 
            _id.Substring(4, 2)), "yyyy/MM/dd", System.Globalization.CultureInfo.InvariantCulture);
        }

        return date.ToShortDateString();
      }
      else
      {
        throw new Exception("Invalid ID");
      }
    }


    public DateTime GetDateOfBirthAsDateTime()
    {
      if (isValid())
      {
        var date = DateTime.ParseExact(string.Format("{0}/{1}/{2}", _id.Substring(0, 2), _id.Substring(2, 2),
          _id.Substring(4, 2)), "yy/MM/dd", System.Globalization.CultureInfo.InvariantCulture);
        var years = DateTime.Now.Year - date.Year;

        if (years < 0)
        {
          date = DateTime.ParseExact(string.Format("19{0}/{1}/{2}", _id.Substring(0, 2), _id.Substring(2, 2), 
            _id.Substring(4, 2)), "yyyy/MM/dd", System.Globalization.CultureInfo.InvariantCulture);
        }

        return date;
      }
      else
      {
        throw new Exception("Invalid ID");
      }
    }
    

    // check whether ID number is valid
    public bool isValid()
    {
      // assume that the id number is invalid
      var isValidPattern = false;
      var isValidLength = false;
      var isValidControlDigit = false;

      // check length
      if (_id.Length == VALID_LENGTH)
      {
        isValidLength = true;
      }

      // match regex pattern, only if length is valid
      if (isValidLength)
      {
        var idPattern = new Regex(REGEX_ID_PATTERN);

        if (idPattern.IsMatch(_id))
        {
          //00 will slip through the regex and checksum
          if (_id.Substring(2, 2) != "00" && _id.Substring(4, 2) != "00")
          {
            isValidPattern = true;
          }
        }
      }
      

      // check control digit, only if previous validations passed
      if (isValidLength && isValidPattern)
      {
        var a = 0;
        var b = 0;
        var c = 0;
        var cDigit = -1;
        var tmp = 0;
        var even = new StringBuilder();
        string evenResult = null;

        // sum odd digits
        for (var i = 0; i < VALID_LENGTH - 1; i = i + 2)
        {
          a = a + int.Parse(_id[i].ToString());
        }

        // build a string containing even digits
        for (var i = 1; i < VALID_LENGTH - 1; i = i + 2)
        {
          even.Append(_id[i]);
        }
        // multipy by 2
        tmp = int.Parse(even.ToString()) * 2;
        // convert to string again
        evenResult = tmp.ToString();
        // sum the digits in evenResult
        for (var i = 0; i < evenResult.Length; i++)
        {
          b = b + int.Parse(evenResult[i].ToString());
        }

        c = a + b;

        try
        {
          cDigit = CONTROL_DIGIT_CHECK_VALUE - int.Parse(c.ToString()[1].ToString());
        }
        catch
        {
          return INVALID;
        }

        if (cDigit == int.Parse(_id[CONTROL_DIGIT_LOCATION].ToString()))
        {
          isValidControlDigit = true;
        }
        else
        {
          if (cDigit > CONTROL_DIGIT_CHECK_EXCEPTION_VALUE)
          {
            if (0 == int.Parse(_id[CONTROL_DIGIT_LOCATION].ToString()))
            {
              isValidControlDigit = true;
            }
          }
        }
      }

      // final check
      return isValidLength && isValidPattern && isValidControlDigit ? VALID : INVALID;
    }

  }
}