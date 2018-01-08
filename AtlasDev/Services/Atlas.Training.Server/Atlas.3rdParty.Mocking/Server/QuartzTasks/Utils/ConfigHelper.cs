using System;
using System.Configuration;


namespace Atlas.Server.Training.QuartzTasks
{
  public static class ConfigHelper
  {
    /// <summary>
    /// The source PostgreSQL host
    /// </summary>
    /// <returns></returns>
    public static string PSQLSourceHost()
    {
      return ConfigurationManager.AppSettings["PSQLSourceHost"] ?? "10.0.1.244";
    }

    /// <summary>
    /// The destination PostgreSQL host
    /// </summary>
    /// <returns></returns>
    public static string PSQLDestHost()
    {
      return ConfigurationManager.AppSettings["PSQLDestHost"] ?? "10.0.0.245";
    }


    /// <summary>
    /// The destination NPGSQL connection string
    /// </summary>
    /// <returns></returns>
    public static string PSQLDestConnectionString()
    {
      return ConfigurationManager.ConnectionStrings["Destination"].ConnectionString ?? "";
    }

    internal static string GetTempPath()
    {
      return ConfigurationManager.AppSettings["TempDir"] ?? System.IO.Path.GetTempPath();
    }


    /// <summary>
    /// The destination NPGSQL connection string
    /// </summary>
    /// <returns></returns>
    public static string PSQLSourceConnectionString()
    {
      return ConfigurationManager.ConnectionStrings["Source"].ConnectionString ?? "";
    }
    

    public static string PSQLSourceUser()
    {
      return ConfigurationManager.AppSettings["PSQLSourceUser"] ?? "postgres";
    }

    public static string PSQLDestUser()
    {
      return ConfigurationManager.AppSettings["PSQLDestUser"] ?? "postgres";
    }


    public static string PSQLSourcePass()
    {
      return ConfigurationManager.AppSettings["PSQLSourcePass"] ?? "s1DT81ChqlVkPZMlRO8b";
    }

    public static string PSQLDestPass()
    {
      return ConfigurationManager.AppSettings["PSQLDestPass"] ?? "s1DT81ChqlVkPZMlRO8b";
    }
  }
}
