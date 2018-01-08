using Atlas.ThirdParty.ABSA.FileHelpers;
using Atlas.ThirdParty.ABSA.FileStructures.Transmission;
using Atlas.ThirdParty.ABSA.NAEDO.FileStructures.Output;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Collection.Engine.Business.Output
{

  public sealed class Output
  {
    public TransmissionHeader TransmissionHeader { get; set; }
    public OutputSetHeader SetHeader { get; set; }
    public List<UserSet> UserSet { get; set; }
  }

  public class UserSet
  {
    public Guid Identifier { get; set; }
    public string GenerationNo { get; set; }
    public OutputUserCodeSetHeader UserCodeSetHeader { get; set; }
    public List<OutputRequestResponse> Transactions { get; set; }
  }
  /// <summary>
  /// Used to import the responses from the output file from NAEDO
  /// </summary>
  public sealed class OutputImporter
  {
    private string _fileName = string.Empty;

    List<dynamic> Items = new List<dynamic>();

    public OutputImporter(string fileName)
    {

      if (string.IsNullOrEmpty(fileName))
        throw new Exception("Filename cannot be null.");

      this._fileName = fileName;

    }

    /// <summary>
    /// Process the file structure.
    /// </summary>
    public dynamic ProcessFile()
    {
      Guid? guid = null;
      Output output = new Output();
      output.UserSet = new List<UserSet>();

      foreach (var line in File.ReadLines(this._fileName))
      {
        guid = DetermineLineType(guid, line, ref output);
      }

      return output;
    }


    private Guid? DetermineLineType(Guid? guidd, string line, ref Output output)
    {

      dynamic item = null;
      Guid? guid = guidd;


      switch (line.Substring(0, 3))
      {
        case "000":
          item = Converters.ToObject(new TransmissionHeader(), line);
          output.TransmissionHeader = item;
          break;
        case "050":
          item = Converters.ToObject(new OutputUserCodeSetHeader(), line);
          guid = Guid.NewGuid();
          output.UserSet.Add(new UserSet() { Identifier = (Guid)guid, UserCodeSetHeader = item, GenerationNo = item.BankServUsercodeGenerationNo });
          break;
        case "051":
          item = Converters.ToObject(new OutputSetHeader(), line);
          output.SetHeader = item;
          break;
        case "052":
          item = Converters.ToObject(new OutputRequestResponse(), line);
          var transaction = output.UserSet.First(o => o.Identifier == guid);

          if (transaction.Transactions == null)
            transaction.Transactions = new List<OutputRequestResponse>();

          transaction.Transactions.Add(item);
          break;
        case "053":
          item = Converters.ToObject(new OutputSetFooter(), line);
          break;
        case "059":
          item = Converters.ToObject(new OutputUserCodeSetFooter(), line);
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