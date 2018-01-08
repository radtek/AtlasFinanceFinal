namespace Atlas.Services.DbfGenerate.QuartzTasks.Utils
{
  internal class ConnStrings
  {
    internal static string Atlas_Core
    {
      get { return "XpoProvider=Postgres;Server=10.0.0.244;User Id=postgres;Password=s1DT81ChqlVkPZMlRO8b;Database=atlas_core"; }
    }

    internal static string Ass
    {
      get { return "Server=10.0.0.244;User Id=postgres;Password=s1DT81ChqlVkPZMlRO8b;Database=ass"; }
    }
  }
}
