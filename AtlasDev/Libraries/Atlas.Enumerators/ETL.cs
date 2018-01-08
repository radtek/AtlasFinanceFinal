using System.ComponentModel;

namespace Atlas.Enumerators
{
  public class ETL
  {
    public enum Stage
    {
      [Description("Importing File")]
      ImportingFile = 1,
      [Description("New")]
      New = 2,
      [Description("Loading")]
      Loading = 3,
      [Description("Completed")]
      Completed = 4,
      [Description("Completed - Contains Errors")]
      CompletedContainsErrors = 5,
      [Description("Error")]
      Error = 6
    }
  }
}
