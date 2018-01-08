using System;
using System.ComponentModel;


namespace Atlas.Enumerators
{
  public sealed class Credentials
  {
    /// <summary>
    /// Credential purpose flag
    /// </summary>
    [Flags]
    public enum CredentialPurpose
    {
      [Description("Credential is for reporting")]
      Report = 1,

      [Description("Credential is for viewing")]
      View = 2,

      [Description("Credential is for adding")]
      Add = 4,

      [Description("Credential is for editing")]
      Edit = 8,

      [Description("Credential is for deleting")]
      Delete = 16,

      [Description("Credential is for administration")]
      Administration = 32,
    }
  }
}
