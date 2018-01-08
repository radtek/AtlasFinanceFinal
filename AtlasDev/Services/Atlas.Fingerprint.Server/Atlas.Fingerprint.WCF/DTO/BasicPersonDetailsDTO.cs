using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace Atlas.WCF.FPServer.Interface
{
  [DataContract]
  public sealed class BasicPersonDetailsDTO
  {
    #region Constructors

    public BasicPersonDetailsDTO()
    {
    }

    public BasicPersonDetailsDTO(Int64 personId, Int64 securityId, string firstName, string otherNames, string lastName, DateTime birthDate, string idOrPassport,
      string cellNumber, string emailAddress, string gender,
      List<PersonRoleDTO> userRoles, string legacyOperatorID, int personType)
    {
      PersonId = personId;
      SecurityId = securityId;
      FirstName = firstName;
      OtherNames = otherNames;
      LastName = lastName;
      BirthDate = birthDate;
      IDOrPassport = idOrPassport;
      CellNumber = cellNumber;
      EMailAddress = emailAddress;
      LegacyOperatorId = legacyOperatorID;
      Gender = gender;
      PersonType = personType;

      Roles = new List<PersonRoleDTO>();
      if (userRoles != null)
      {
        Roles.AddRange(userRoles);
      }
    }

    #endregion


    #region Properties

    [DataMember]
    public Int64 PersonId { get; set; }

    [DataMember]
    public Int64 SecurityId { get; set; }

    [DataMember]
    public string FirstName { get; set; }

    [DataMember]
    public string OtherNames { get; set; }

    [DataMember]
    public string LastName { get; set; }

    [DataMember]
    public DateTime BirthDate { get; set; }

    [DataMember]
    public string IDOrPassport { get; set; }

    [DataMember]
    public string EMailAddress { get; set; }

    [DataMember]
    public string CellNumber { get; set; }

    [DataMember]
    public List<PersonRoleDTO> Roles { get; set; }

    [DataMember]
    public string LegacyOperatorId { get; set; }

    [DataMember]
    public string Gender { get; set; }

    [DataMember]
    public int PersonType { get; set; }

    #endregion

  }
}
