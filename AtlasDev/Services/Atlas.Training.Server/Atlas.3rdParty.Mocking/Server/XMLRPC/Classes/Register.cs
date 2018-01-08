/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     XML-RPC Class for the Register method
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2013-12-11- Skeleton created
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

#region Using

using System;
using CookComputing.XmlRpc;

#endregion


namespace Atlas.ThirdParty.XMLRPC.Classes
{
  public struct Register_Input
  {
    public const string MethodName = "Register";

    public string terminalID;

    public string emailAddress;

    public string password;

    public string firstName;

    public string lastName;

    public string idNumber;

    public string contactNumber;

    public string cellPhoneNumber;

    public bool isCompany;

    public string vatNumber;

    public string companyName;

    public string companyCCNumber;

    public string addressLine1;

    public string addressLine2;

    public string city;

    public string postalCode;
    
    public string transactionID;

    public DateTime transactionDate;

    public override string ToString()
    {
      return string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}{17}{18:yyyyMMddTHH:mm:ss}",
          MethodName,
          terminalID,
          emailAddress,
          password,
          firstName,
          lastName,
          idNumber,
          contactNumber,
          cellPhoneNumber,
          isCompany ? "true":"false",
          vatNumber,
          companyName,
          companyCCNumber,
          addressLine1,
          addressLine2,
          city,
          postalCode,
          transactionID, 
          transactionDate
          );
    }
  }


  public struct Register_Output
  {
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string terminalID;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string emailAddress;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string firstName;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string lastName;
        
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string idNumber;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string contactNumber;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string cellPhoneNumber;

    // Their spec says boolean, but returns string containing 'Yes' !?!?
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string isCompany;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string vatNumber;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string companyName;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string companyCCNumber;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string addressLine1;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string addressLine2;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string city;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string postalCode;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string profileNumber;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string message;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string clientTransactionID;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public string serverTransactionID;

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public int? resultCode;

    public string resultText;
  }
}
