/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012-2016 Atlas Finance (Pty) Ltd.
 * 
 *  Description:
 *  ------------------
 *     General data repository for handling the 'Per_Person' table
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2013-12-11 -  Updates    
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Domain.Model;
using Atlas.Domain.DTO;
using Atlas.Common.Utils;
using Atlas.Enumerators;


namespace Atlas.Data.Repository
{
  /// <summary>
  /// General database repository routines
  /// </summary>
  public static class PersonData
  {

    /// <summary>
    /// Find person by identity/passport/surname/etc (best to poorest match) and optionally add if not found
    /// </summary>
    /// <param name="unitOfWork">The XPO unit of work to use </param>
    /// <param name="idOrPassport">ID or passport number</param>
    /// <param name="firstName">First name of person</param>
    /// <param name="lastName">Last name of person</param>
    /// <param name="cellNumber">Cellular number of person (optional)</param>
    /// <param name="user">The user carrying out this action</param>
    /// <param name="host">The host system carrrying out this action</param>
    /// <param name="addIfNotFound">Add the person/person cellular contact if not found</param>
    /// <returns>A Per_Person of best match, else NULL if not found and not added</returns>
    public static PER_Person FindPerson(UnitOfWork unitOfWork, string idOrPassport, string firstName, string lastName, string cellNumber,
      PER_SecurityDTO user, General.Host host, bool addIfNotFound = true)
    {
      #region Try find person
      // First try full match on all fields
      var person = unitOfWork.Query<PER_Person>().FirstOrDefault(s => s.IdNum == idOrPassport && s.Firstname.ToUpper() == firstName.ToUpper() && s.Lastname.ToUpper() == lastName.ToUpper());
      if (person == null)
      {
        // Second, try find with ID number and surname
        person = unitOfWork.Query<PER_Person>().FirstOrDefault(s => s.IdNum == idOrPassport && s.Lastname.ToUpper() == lastName.ToUpper());
      }
      if (person == null)
      {
        // Final, try find by ID number
        person = unitOfWork.Query<PER_Person>().FirstOrDefault(s => s.IdNum == idOrPassport);
      }
      #endregion

      if (addIfNotFound)
      {
        #region Add person if does not exist
        if (person == null)
        {
          #region Try determine gender/age from ID
          var idValidator = new IDValidator(idOrPassport);
          var dateofBirth = DateTime.Now;
          var gender = "M";
          if (idValidator.isValid())
          {
            dateofBirth = idValidator.GetDateOfBirthAsDateTime();
            gender = idValidator.IsFemale() ? "F" : "M";
          }
          #endregion

          var userPerson = user != null && user.Person != null ? unitOfWork.Query<PER_Person>().First(s => s.PersonId == user.Person.PersonId) : null;
          var hostDb = unitOfWork.Query<Host>().First(s => s.Type == host);

          person = new PER_Person(unitOfWork)
          {
            Firstname = firstName,
            Lastname = lastName,
            IdNum = idOrPassport,
            DateOfBirth = dateofBirth,
            Gender = gender,

            CreatedDT = DateTime.Now,
            CreatedBy = userPerson,

            Host = hostDb,

            PersonType = (user != null) ? unitOfWork.Query<PER_Type>().First(s => s.Type == General.PersonType.Employee) :
                                          unitOfWork.Query<PER_Type>().First(s => s.Type == General.PersonType.Client)
          };
        }
        #endregion

        #region Add cell number
        if (!string.IsNullOrEmpty(cellNumber))
        {
          var cellType = unitOfWork.Query<ContactType>().First(s => s.ContactTypeId == (int)Atlas.Enumerators.General.ContactType.CellNo);
          var foundContact = person.GetContacts.FirstOrDefault(s => s.Contact.ContactType == cellType && s.Contact.Value == cellNumber);
          if (foundContact == null)
          {
            var contact = new Contact(unitOfWork)
            {
              CreatedDT = DateTime.Now,
              ContactType = cellType,
              Value = cellNumber,
              IsActive = true
            };

            var perContact = new PER_Contacts(unitOfWork)
            {
              Contact = contact,
              Person = person,
            };
            //person.Contacts.Add(contact);
          }
        }
        #endregion

        unitOfWork.CommitChanges();
      }

      return person;
    }


    /// <summary>
    /// Find person by identity/passport/surname/etc (best to poorest match) and optionally add if not found
    /// </summary>
    /// <param name="unitOfWork">The XPO unit of work to use </param>
    /// <param name="idOrPassport">ID or passport number</param>
    /// <param name="firstName">First name of person</param>
    /// <param name="lastName">Last name of person</param>
    /// <param name="cellNumber">Cellular number of person (optional)</param>
    /// <param name="user">The user carrying out this action</param>
    /// <param name="host">The host system carrrying out this action</param>
    /// <param name="addIfNotFound">Add the person/person cellular contact if not found</param>
    /// <returns>A Per_PersonDto of best match, else NULL if not found and not added</returns>
    public static PER_PersonDTO FindPersonDTO(string idOrPassport, string firstName, string lastName, string cellNumber,
      PER_SecurityDTO user, General.Host host, bool addIfNotFound = true)
    {
      using (var unitOfWork = new UnitOfWork())
      {
        return AutoMapper.Mapper.Map<PER_PersonDTO>(FindPerson(unitOfWork, idOrPassport, firstName, lastName, cellNumber, user, host, addIfNotFound));
      }
    }


    /// <summary>
    /// Gets active cellular number for a person
    /// </summary>
    /// <param name="personId"></param>
    /// <returns></returns>
    public static string GetCellNumForPerson(Int64 personId)
    {
      using (var unitOfWork = new UnitOfWork())
      {
        var contact = unitOfWork.Query<PER_Person>()
          .FirstOrDefault(s => s.PersonId == personId)
          .GetContacts.FirstOrDefault(c => c.Contact.ContactType == unitOfWork.Query<ContactType>().First(t => t.ContactTypeId == (int)General.ContactType.CellNo) &&
           c.Contact.IsActive);

        return contact?.Contact?.Value;
      }
    }

  }
}
