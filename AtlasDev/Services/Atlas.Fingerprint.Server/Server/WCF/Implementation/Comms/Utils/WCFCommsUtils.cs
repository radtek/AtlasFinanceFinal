using Atlas.Domain.Model;
using Atlas.WCF.FPServer.Interface;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Atlas.WCF.FPServer.WCF.Implementation.Comms.Utils
{
  public static class WCFCommsUtils
  {
    /// <summary>
    /// Utility method to find person on various values and return a BasicPersonDetailsDTO
    /// </summary>
    /// <param name="findByField">Field to search</param>
    /// <param name="findByValue">What value to use for the search</param>
    /// <param name="personDetails">The person details, null if not found</param>
    /// <returns>true if found, false if could not be located</returns>
    public static bool PersonToBasicPersonDetailsDTO(FindPersonByField findByField, string findByValue, out BasicPersonDetailsDTO personDetails)
    {
      personDetails = null;
      using (var unitOfWork = new UnitOfWork())
      {
        #region Try find based on request
        PER_Person person = null;
        switch (findByField)
        {
          case FindPersonByField.ByIdOrPassport:
            person = unitOfWork.Query<PER_Person>().FirstOrDefault(s => s.IdNum == findByValue);// && s.Security.IsActive);
            break;

          case FindPersonByField.ByLegacyOperatorId:
            person = unitOfWork.Query<PER_Person>().FirstOrDefault(s => s.Security != null && s.Security.IsActive && s.Security.LegacyOperatorId == findByValue);
            break;

          case FindPersonByField.ByPersonId:
            person = unitOfWork.Query<PER_Person>().FirstOrDefault(s => s.PersonId == Int64.Parse(findByValue));

            break;

          default:
            throw new Exception(string.Format("Unknown search field: {0}", findByField));
        }

        if (person == null)
        {
          return false;
        }

        #endregion

        personDetails = new BasicPersonDetailsDTO
        {
          PersonId = person.PersonId,
          FirstName = person.Firstname,
          OtherNames = person.Othername,
          LastName = person.Lastname,
          BirthDate = person.DateOfBirth,
          IDOrPassport = person.IdNum,
          Gender = person.Gender,
          PersonType = person.PersonType != null ? (int)person.PersonType.TypeId : (int)Atlas.Enumerators.General.PersonType.Client
        };

        #region Get roles
        personDetails.Roles = new List<PersonRoleDTO>();
        var rolesInDB = unitOfWork.Query<PER_Role>().Where(s => s.Person.PersonId == person.PersonId);
        foreach (var role in rolesInDB)
        {
          personDetails.Roles.Add(new PersonRoleDTO() { RoleTypeId = role.RoleType.RoleTypeId, Description = role.RoleType.Description, Level = role.RoleType.Level });
        }
        #endregion

        #region Get security
        var security = unitOfWork.Query<PER_Security>().FirstOrDefault(s => s.Person.PersonId == person.PersonId);
        if (security != null)
        {
          personDetails.LegacyOperatorId = security.LegacyOperatorId;
          personDetails.SecurityId = security.SecurityId;
        }
        #endregion

        var contacts = person.GetContacts.Select(s => s.Contact);
        if (contacts.Any())
        {
          #region Get Cell
          var cell = contacts.FirstOrDefault(s =>s.ContactType.ContactTypeId == (int)Enumerators.General.ContactType.CellNo && s.IsActive);
          if (cell != null)
          {
            personDetails.CellNumber = cell.Value;
          }
          #endregion

          #region Get e-mail
          var email = contacts.FirstOrDefault(s => s.ContactType.ContactTypeId == (int)Enumerators.General.ContactType.Email && s.IsActive);
          if (email != null)
          {
            personDetails.EMailAddress = email.Value;
          }
          if (string.IsNullOrEmpty(personDetails.EMailAddress) && !string.IsNullOrEmpty(person.Email))
          {
            personDetails.EMailAddress = person.Email;
          }
          #endregion
        }

        return true;
      }
    }


    #region Public enum

    /// <summary>
    /// Search by
    /// </summary>
    public enum FindPersonByField { ByPersonId, ByLegacyOperatorId, ByIdOrPassport };

    #endregion
  }
}
