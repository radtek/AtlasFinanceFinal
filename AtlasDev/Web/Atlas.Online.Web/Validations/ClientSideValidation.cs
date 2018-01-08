using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atlas.Online.Web.Validations
{
  public enum ClientSideValidationType
  {
    /// <summary>
    /// Check validity remotely
    /// </summary>
    RemoteValidity,
    /// <summary>
    /// Remote validity not yet checked
    /// </summary>
    RemoteValidityUnchecked,
    /// <summary>
    /// Error has occurred when checking validity remotely
    /// </summary>
    RemoteValidityFailed

  }

  public class ClientSideValidation
  {
    public string ErrorMessage { get; set; }

    public string ResourceName { get; set; }
    public Type ResourceType { get; set; }

    public ClientSideValidationType ValidationType { get; set; }    
  }
}