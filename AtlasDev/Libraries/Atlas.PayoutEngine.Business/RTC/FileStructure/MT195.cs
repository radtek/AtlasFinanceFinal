using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.PayoutEngine.Business.RTC.FileStructure
{
  public class MT195
  {
    private const string TRANSACTION_REFERENCE_NUMBER_TAG = ":20:";
    private const string CUSTOMER_SPECIFIED_REFERNCE_TAG = ":21:";
    private const string REASON_TAG = ":75:";
    private const string NARRATIVE_TAG = ":77:";
    private const string DATE_TAG = ":11R:";
    private const string NARRATIVE_DESCRIPTION = ":79:";

    private const string TRANSACTION_END = "-}";
    private const string TRANSACTION_BLOCK_SEPERATOR = "}{";

    private string _fileContent;

    public MT195(string fileContent)
    {
      _fileContent = fileContent.Replace("\r", " ").Replace("\n", " ");
    }

    public List<Response> ConvertToResponse()
    {
      var responses = new List<Response>();

      var transactions = _fileContent.Split(new[] { TRANSACTION_END }, StringSplitOptions.RemoveEmptyEntries);

      foreach (var transaction in transactions)
      {
        if (transaction.Contains(TRANSACTION_REFERENCE_NUMBER_TAG))
        {
          var transactionBlocks = transaction.Split(new[] { TRANSACTION_BLOCK_SEPERATOR },
                                                    StringSplitOptions.RemoveEmptyEntries);
          if (transactionBlocks.Count() >= 3)
          {
            var response = new Response();

            var transmissionBlock = transactionBlocks[(transactionBlocks.Count() == 3 ? 2 : 3)];

            response.TransmissionId = int.Parse(
                transmissionBlock.Substring(
                    transmissionBlock.IndexOf(TRANSACTION_REFERENCE_NUMBER_TAG) + TRANSACTION_REFERENCE_NUMBER_TAG.Length,
                    transmissionBlock.IndexOf(CUSTOMER_SPECIFIED_REFERNCE_TAG) -
                    (transmissionBlock.IndexOf(TRANSACTION_REFERENCE_NUMBER_TAG) + TRANSACTION_REFERENCE_NUMBER_TAG.Length))
                    .Trim());

            response.PayoutId = int.Parse(
                transmissionBlock.Substring(
                    transmissionBlock.IndexOf(CUSTOMER_SPECIFIED_REFERNCE_TAG) + CUSTOMER_SPECIFIED_REFERNCE_TAG.Length,
                    transmissionBlock.IndexOf(REASON_TAG) -
                    (transmissionBlock.IndexOf(CUSTOMER_SPECIFIED_REFERNCE_TAG) + CUSTOMER_SPECIFIED_REFERNCE_TAG.Length))
                    .Trim());

            response.ReplyCode =
                transmissionBlock.Substring(
                    transmissionBlock.IndexOf(REASON_TAG) + REASON_TAG.Length, 4);

            response.Reason =
                transmissionBlock.Substring(
                    transmissionBlock.IndexOf(REASON_TAG) + REASON_TAG.Length,
                    (transmissionBlock.IndexOf(transmissionBlock.IndexOf(NARRATIVE_TAG) >= 0 ? NARRATIVE_TAG : DATE_TAG)) -
                    (transmissionBlock.IndexOf(REASON_TAG) + REASON_TAG.Length)).Trim();

            response.Accepted = response.Reason.StartsWith("A");

            responses.Add(response);
          }
          else
          {
            throw new Exception("Insufficient block in reponse file");
          }
        }
      }

      return responses;
    }
  }
}
