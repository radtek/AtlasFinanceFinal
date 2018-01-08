using Atlas.ThirdParty.ABSA.FileHelpers;
using Atlas.ThirdParty.ABSA.FileStructures.Transmission;
using Atlas.ThirdParty.ABSA.NAEDO.FileStructures;
using Atlas.ThirdParty.ABSA.NAEDO.FileStructures.Reply;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Collection.Engine.Business
{

  public sealed class Reply
  {
    public TransmissionHeader TransmissionHeader { get; set; }
    public TransmissionStatusItem TransmissionStatus { get; set; }
    public List<UserSet> UserSet { get; set; }
  }

  public class UserSet
  {
    public Guid Identifier { get; set; }
    public string GenerationNo { get; set; }
    public UserSetStatusItem UserSetStatus { get; set; }
    public List<Transaction> Transactions { get; set; }
    public List<RejectedMessageItem> RejectedTransaction { get; set; }
  }

  /// <summary>
  /// Used to import the responses from the reply file from NAEDO
  /// </summary>
  public sealed class ReplyImporter
  {
    private string _fileName = string.Empty;
    List<dynamic> Items = new List<dynamic>();

    public ReplyImporter(string fileName)
    {

      if (string.IsNullOrEmpty(fileName))
        throw new Exception("Filename cannot be null.");

      this._fileName = fileName;

    }

    /// <summary>
    /// Process the file structure.
    /// </summary>
    public Reply ProcessFile()
    {
      Guid? guid = null;
      Reply reply = new Reply();
      reply.UserSet = new List<UserSet>();

      foreach (var line in File.ReadLines(this._fileName))
      {
        guid = DetermineLineType(guid, line, ref reply);
      }
      
      return reply;
    }

    private Guid? DetermineLineType(Guid? guidd, string line, ref Reply reply)
    {
      dynamic item = null;
      Guid? guid = guidd;

      switch (line.Substring(0, 3))
      {
        case "000":
          item = Converters.ToObject(new TransmissionHeader(), line);
          reply.TransmissionHeader = item;

          break;
        case "900":
          switch (line.Substring(4, 3))
          {
            case "000":
              item = Converters.ToObject(new TransmissionStatusItem(), line);
              reply.TransmissionStatus = item;
              break;
            case "050":
              item = Converters.ToObject(new UserSetStatusItem(), line);
              guid = Guid.NewGuid();
              reply.UserSet.Add(new UserSet() { Identifier = (Guid)guid, UserSetStatus = item, GenerationNo = item.BankServUserCodeGenerationNo });

              break;
          }
          break;
        case "903":
          line = line.Substring(4, (line.Length - 4));
          line = line.PadRight((line.Length + 5), Convert.ToChar(' '));
          item = Converters.ToObject(new Transaction(), line);

          var transaction = reply.UserSet.First(o => o.Identifier == guid);

          if (transaction.Transactions == null)
            transaction.Transactions = new List<Transaction>();

          transaction.Transactions.Add(item);
          break;
        case "901":
          item = Converters.ToObject(new RejectedMessageItem(), line);

          if (reply.UserSet.Count > 0)
          {
            var rejectedTransaction = reply.UserSet.First(o => o.Identifier == guid);

            if (rejectedTransaction.RejectedTransaction == null)
              rejectedTransaction.RejectedTransaction = new List<RejectedMessageItem>();

            rejectedTransaction.RejectedTransaction.Add(item);
          }
          break;
        case "999":
          item = Converters.ToObject(new TransmissionTrailer(), line);
          break;

        default:
          break;
      }
      return guid;
    }
  }
}
