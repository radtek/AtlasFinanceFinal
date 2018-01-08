/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Credit card validation   
 * 
 * 
 *  Author:
 *  ------------------
 *     Source: http://jlcoady.net/c-sharp/credit-card-validation-in-c-sharp
 *     Modifications by Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2012-06-20 - Obtained from Web
 *     
 * 
 * 
 *  Notes:
 *  ------------------
 
 *     
 * ----------------------------------------------------------------------------------------------------------------- */

using System;


namespace Atlas.Common.Utils
{

  public sealed class CCardValidator
  {
    public enum CardType { NuCard, MasterCard, BankCard, Visa, AmericanExpress, Discover, DinersClub, JCB, Other };


    public static bool Validate(CardType cardType, string cardNumber)
    {
      var number = new byte[16]; // number to validate

      // Remove non-digits
      var len = 0;
      for (var i = 0; i < cardNumber.Length; i++)
      {
        if (char.IsDigit(cardNumber, i))
        {
          if (len == 16) return false; // number has too many digits
          number[len++] = byte.Parse(cardNumber.Substring(i, 1));
        }
      }

      // Validate based on card type, first if tests length, second tests prefix
      switch (cardType)
      {
        // KB
        case CardType.NuCard:
          if (len != 16)
            return false;
          // NuCard starts with 533892 (BIN)
          if (number[0] != 5 || number[1] != 3 || number[2] != 3 || number[3] != 8 || number[4] != 9 || number[5] != 2)
            return false;
          break;

        case CardType.MasterCard:
          if (len != 16)
            return false;
          if (number[0] != 5 || number[1] == 0 || number[1] > 5)
            return false;
          break;

        case CardType.BankCard:
          if (len != 16)
            return false;
          if (number[0] != 5 || number[1] != 6 || number[2] > 1)
            return false;
          break;

        case CardType.Visa:
          if (len != 16 && len != 13)
            return false;
          if (number[0] != 4)
            return false;
          break;

        case CardType.AmericanExpress:
          if (len != 15)
            return false;
          if (number[0] != 3 || (number[1] != 4 && number[1] != 7))
            return false;
          break;

        case CardType.Discover:
          if (len != 16)
            return false;
          if (number[0] != 6 || number[1] != 0 || number[2] != 1 || number[3] != 1)
            return false;
          break;

        case CardType.DinersClub:
          if (len != 14)
            return false;
          if (number[0] != 3 || (number[1] != 0 && number[1] != 6 && number[1] != 8)
             || number[1] == 0 && number[2] > 5)
            return false;
          break;

        case CardType.JCB:
          if (len != 16 && len != 15)
            return false;
          if (number[0] != 3 || number[1] != 5)
            return false;
          break;
      }

      // Use Luhn's Algorithm to validate
      var sum = 0;
      for (var i = len - 1; i >= 0; i--)
      {
        if (i % 2 == len % 2)
        {
          var n = number[i] * 2;
          sum += (n / 10) + (n % 10);
        }
        else
          sum += number[i];
      }
      return (sum % 10 == 0);
    }

  }
}

