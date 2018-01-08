/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2015 Atlas Finance (Pty) Ltd.
 * 
 *  Description:
 *  ------------------
 *    Better South African ID validation routines
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2015-12-28: Created
 *
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.ComponentModel;


namespace Atlas.Common.Utils
{
  public class IdValidator2
  {
    public IdValidator2(string idNumber)
    {
      _idNumber = idNumber;
      Process();
    }


    #region Public static methods

    /// <summary>
    /// Checks if a South African ID is valid
    /// </summary>
    /// <param name="idNumber">The South African ID</param>
    /// <returns>true if valid, else false</returns>
    public static bool IsValidSouthAfricanId(string idNumber)
    {
      var instance = new IdValidator2(idNumber);
      return (instance.Error == ErrorTypes.None);
    }

    /// <summary>
    /// Checks if a South African ID is valid and returns what caused failure
    /// </summary>
    /// <param name="idNumber">The South African ID</param>
    /// <param name="error">What failed</param>
    /// <returns>true if valid, else false</returns>
    public static bool IsValidSouthAfricanId(string idNumber, out ErrorTypes error)
    {
      var instance = new IdValidator2(idNumber);
      error = instance.Error;
      return (instance.Error == ErrorTypes.None);
    }


    /// <summary>
    /// Checks if a South African ID is valid and returns decoded details
    /// </summary>
    /// <param name="idNumber">The South African ID</param>
    /// <param name="dateOfBirth">If ID valid, tHe date of birth, else DateTime.MinValue</param>
    /// <param name="gender">If ID valid, tHe gender type</param>
    /// <param name="citizenType">If ID valid, tHe citizen type</param>
    /// <param name="error">If error,  where parsing failed</param>
    /// <returns>true if valid, else false</returns>
    public static bool IsValidSouthAfricanId(string idNumber, out DateTime dateOfBirth, out GenderTypes gender, out CitizenTypes citizenType, out ErrorTypes error)
    {
      var instance = new IdValidator2(idNumber);
      dateOfBirth = instance.DateOfBirth;
      gender = instance.Gender;
      citizenType = instance.CitizenState;
      error = instance.Error;
      return instance.Error == ErrorTypes.None;
    }


    /// <summary>
    /// Checks if a South African ID is valid and returns the error code
    /// </summary>
    /// <param name="idNumber">The South African ID</param>
    /// <returns>where the parsing failed, else ErrorTypes.None</returns>
    public static ErrorTypes CheckSouthAfricanId(string idNumber)
    {
      var instance = new IdValidator2(idNumber);
      return instance.Error;
    }

    #endregion


    #region Private methods

    private void Process()
    {
      #region Basic digits and length
      if (!Regex.IsMatch(_idNumber, "^[0-9]{13}$"))
      {
        Error = ErrorTypes.CharsBad;
        return;
      }
      #endregion

      var digits = Encoding.ASCII.GetBytes(_idNumber).Select(s => s - 48).ToList(); // Convert ASCII value character '1', to byte value 1;
      
      #region Cannot be all the same digit i.e. 1111111111111
      var uniqueDigits = digits.Distinct().Count();
      if (uniqueDigits == 1)
      {
        Error = ErrorTypes.CharsBad;
        return;
      }
      #endregion

      #region Date of birth
      DateTime dob;
      try
      {
        dob = new DateTime(1900 + digits[0] * 10 + digits[1], digits[2] * 10 + digits[3], digits[4] * 10 + digits[5]);
      }
      catch
      {
        Error = ErrorTypes.InvalidDOB;
        return;
      }  
      DateOfBirth = dob;
      #endregion

      #region Citizen type      
      switch (digits[10])
      {
        case 0:
          CitizenState = CitizenTypes.RSACitizen;
          break;

        case 1:
          CitizenState = CitizenTypes.PermanentResident;
          break;

        case 2:
          CitizenState = CitizenTypes.Foreigner;
          break;

        default:
          Error = ErrorTypes.BadCitizenType;
          return;
      }
      #endregion

      Gender = (digits[6] < 5) ? GenderTypes.Female : GenderTypes.Male;

      #region Checksum
      var checkSum = 0;
      for (var digit = 0; digit < 12; digit++)
      {
        if (digits[digit] > 0)
        {
          if (digit % 2 == 0) // digit 0,2,4 are actually odd positions, simply add
          {
            checkSum += digits[digit];
          }
          else // even digit, multiply by 2 and use each digit, i.e. 5 = 10 (1+0), 6 = 12 - add 3 (1+2), 7 = 14 (1+4)- add 5, 
          {
            digits[digit] *= 2;
            checkSum += (digits[digit] > 9) ? digits[digit] - 9 : digits[digit];
          }
        }
      }
      var lowerDigit = (checkSum % 10);
      lowerDigit = (lowerDigit == 0) ? 10 : lowerDigit;
      if (10 - lowerDigit != digits[12])
      {
        Error = ErrorTypes.FailedChecksum;
        return;
      }
      #endregion

      Error = ErrorTypes.None;
    }

    #endregion


    #region Public enums

    public enum ErrorTypes
    {
      [Description("Not processed")]
      NotSet,

      [Description("Contains illegal characters/invalid length")]
      CharsBad,

      [Description("Invalid date of birth")]
      InvalidDOB,

      [Description("Invalid citizen type")]
      BadCitizenType,

      [Description("Failed checksum calculation")]
      FailedChecksum,

      [Description("Valid")]
      None
    };

    public enum CitizenTypes { NotSet, RSACitizen, PermanentResident, Foreigner };

    public enum GenderTypes { NotSet, Male, Female };

    #endregion


    #region Public properties

    public DateTime DateOfBirth { get; private set; }

    public ErrorTypes Error { get; private set; }

    public CitizenTypes CitizenState { get; private set; }

    public GenderTypes Gender { get; private set; }

    #endregion


    #region Private fields

    private readonly string _idNumber;

    #endregion

  }
}
