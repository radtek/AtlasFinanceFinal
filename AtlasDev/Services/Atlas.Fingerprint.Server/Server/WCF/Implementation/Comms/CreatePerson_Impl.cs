using System;
using System.Linq;
using System.Text.RegularExpressions;

using DevExpress.Xpo;

using Atlas.Enumerators;
using Atlas.WCF.FPServer.Common;
using Atlas.Domain.Model;
using Atlas.WCF.FPServer.Interface;
using Atlas.WCF.FPServer.Security.Interface;
using Atlas.Common.Interface;


namespace Atlas.WCF.FPServer.WCF.Implementation.Comms
{
  public static class CreatePerson_Impl
  { 
    public static int Execute(ILogging log, SourceRequest sourceRequest, BasicPersonDetailsDTO personDetails,
      out Int64 personId, out string errorMessage)
    {
      var methodName = "CreatePerson";
      log.Information("{MethodName} starting: {@Request}", methodName, new { sourceRequest, personDetails });

      personId = 0;
      errorMessage = string.Empty;

      #region Check request
      Machine machine;
      User user;
      Int64 branchId;
      if (!WCFUtils.CheckSourceRequest(log, sourceRequest, out branchId, out machine, out user, out errorMessage))
      {
        log.Warning(new Exception(errorMessage), methodName);
        return (int)General.WCFCallResult.BadParams;
      }

      if (personDetails.BirthDate == DateTime.MinValue)
      {
        errorMessage = "BirthDate cannot be blank";
        log.Warning(new Exception(errorMessage), methodName);
        return (int)General.WCFCallResult.BadParams;
      }

      var age = DateTime.Today.Subtract(personDetails.BirthDate).TotalDays / 365.25;
      if (age < 18 || age > 100)
      {
        errorMessage = string.Format("BirthDate contains an invalid value- person is {0} years old", age);
        log.Warning(new Exception(errorMessage), methodName);
        return (int)General.WCFCallResult.BadParams;
      }

      if (string.IsNullOrWhiteSpace(personDetails.FirstName))
      {
        errorMessage = "firstName parameter missing a value";
        log.Warning(new Exception(errorMessage), methodName);
        return (int)General.WCFCallResult.BadParams;
      }

      if (string.IsNullOrWhiteSpace(personDetails.LastName))
      {
        errorMessage = "lastName parameter missing a value";
        log.Warning(new Exception(errorMessage), methodName);
        return (int)General.WCFCallResult.BadParams;
      }

      if (string.IsNullOrWhiteSpace(personDetails.IDOrPassport))
      {
        errorMessage = "idOrPassport parameter missing a value";
        log.Warning(new Exception(errorMessage), methodName);
        return (int)General.WCFCallResult.BadParams;
      }

      if (personDetails.IDOrPassport.Length < 5)
      {
        errorMessage = "idOrPassport cannot be less than 5 characters!";
        log.Warning(new Exception(errorMessage), methodName);
        return (int)General.WCFCallResult.BadParams;
      }

      if (personDetails.IDOrPassport.Length == 13)
      {
        var validator = new Atlas.Common.Utils.IDValidator(personDetails.IDOrPassport);
        if (!validator.isValid())
        {
          errorMessage = "ID number is invalid";
          log.Warning(new Exception(errorMessage), methodName);
          return (int)General.WCFCallResult.BadParams;
        }
      }

      if (string.IsNullOrEmpty(personDetails.Gender))
      {
        errorMessage = "gender parameter missing a value";
        log.Warning(new Exception(errorMessage), methodName);
        return (int)General.WCFCallResult.BadParams;
      }

      var gender = personDetails.Gender.Trim().ToUpper();
      if (gender != "M" && gender != "F")
      {
        errorMessage = "gender parameter must be either 'M' or 'F'";
        log.Warning(new Exception(errorMessage), methodName);
        return (int)General.WCFCallResult.BadParams;
      }

      if (!string.IsNullOrWhiteSpace(personDetails.LegacyOperatorId))
      {
        if (personDetails.LegacyOperatorId.Length != 4 || personDetails.LegacyOperatorId == "0000" ||
          personDetails.LegacyOperatorId == "1111" || !Regex.IsMatch(personDetails.LegacyOperatorId, "[0-9]{4,4}"))
        {
          errorMessage = "Invalid operator code- must be 4 digits and cannot be admin code";
          log.Warning(new Exception(errorMessage), methodName);
          return (int)General.WCFCallResult.BadParams;
        }
      }

      if (!string.IsNullOrWhiteSpace(personDetails.EMailAddress))
      {
        if (!Atlas.Common.Utils.Validation.IsValidEmail(personDetails.EMailAddress))
        {
          errorMessage = string.Format("eMailAddress invalid parameter: '{0}'", personDetails.EMailAddress);
          log.Warning(new Exception(errorMessage), methodName);
          return (int)General.WCFCallResult.BadParams;
        }
      }

      string cellPhoneNumber = null;
      if (!string.IsNullOrWhiteSpace(personDetails.CellNumber))
      {
        cellPhoneNumber = Regex.Replace(personDetails.CellNumber, @"[^\d]", string.Empty);
        if (cellPhoneNumber.Length != 10)
        {
          errorMessage = string.Format("cellPhoneNumber invalid parameter: '{0}'", personDetails.CellNumber);
          log.Warning(new Exception(errorMessage), methodName);
          return (int)General.WCFCallResult.BadParams;
        }
      }

      if (personDetails.PersonType == 0)
      {
        personDetails.PersonType = string.IsNullOrEmpty(personDetails.LegacyOperatorId) ?
          (int)General.PersonType.Client : (int)General.PersonType.Employee;
      }
      #endregion

      try
      {
        #region Add/edit person
        PER_Person person = null;
        using (var unitOfWork = new UnitOfWork())
        {
          #region If provided a Legacy Operator, ensure that this is not already in use by another active person
          if (!string.IsNullOrEmpty(personDetails.LegacyOperatorId))
          {
            var securityDb = unitOfWork.Query<PER_Security>().FirstOrDefault(s => s.LegacyOperatorId == personDetails.LegacyOperatorId);
            if (securityDb != null && securityDb.Person != null && securityDb.Person.IdNum != personDetails.IDOrPassport)
            {
              errorMessage = string.Format("The operator ID of '{0}' already in use by '{1} {2}'. Please remove this operator ID from this person " +
                "before re-assigning.", personDetails.LegacyOperatorId, securityDb.Person.Firstname, securityDb.Person.Lastname);
              log.Warning(new Exception(errorMessage), methodName);
              return (int)General.WCFCallResult.BadParams;
            }
          }
          #endregion

          person = unitOfWork.Query<PER_Person>().FirstOrDefault(s => s.IdNum == personDetails.IDOrPassport);

          #region Add person
          if (person == null)
          {
            #region User PersonId
            var userPersonId = sourceRequest.UserPersonId;
            if (userPersonId == 0 && !string.IsNullOrEmpty(sourceRequest.UserIDOrPassport))
            {
              var userInDb = unitOfWork.Query<PER_Security>().FirstOrDefault(s => s.Person.IdNum == sourceRequest.UserIDOrPassport) ??
                unitOfWork.Query<PER_Security>().FirstOrDefault(s => s.LegacyOperatorId == sourceRequest.UserIDOrPassport);
              if (userInDb != null && userInDb.Person != null)
              {
                userPersonId = userInDb.Person.PersonId;
              }
            }
            #endregion

            person = new PER_Person(unitOfWork)
            {
              IdNum = personDetails.IDOrPassport,
              Host = unitOfWork.Query<Host>().First(s => s.HostId == (int)General.Host.ASS),
              Branch = !string.IsNullOrEmpty(sourceRequest.BranchCode) ?
                unitOfWork.Query<BRN_Branch>().FirstOrDefault(s => s.LegacyBranchNum.PadLeft(3, '0') == sourceRequest.BranchCode.PadLeft(3, '0')) : null,
              CreatedDT = DateTime.Now,
              CreatedBy = userPersonId > 0 ?
                unitOfWork.Query<PER_Person>().FirstOrDefault(s => s.PersonId == userPersonId) : null
            };
          }
          #endregion

          if (personDetails.BirthDate != DateTime.MinValue)
          {
            person.DateOfBirth = personDetails.BirthDate;
          }
          person.Firstname = personDetails.FirstName.Trim();
          person.Lastname = personDetails.LastName.Trim();
          person.Middlename = personDetails.OtherNames.Trim();
          person.Gender = gender;
          person.Email = personDetails.EMailAddress.Trim();
          person.PersonType = personDetails.PersonType == (int)General.PersonType.Employee || person.Security != null ?
            unitOfWork.Query<PER_Type>().First(s => s.Type == General.PersonType.Employee) :
            unitOfWork.Query<PER_Type>().First(s => s.Type == General.PersonType.Client);

          person.LastEditedDT = DateTime.Now;

          #region Add cell phone number if does not exist...
          if (!string.IsNullOrWhiteSpace(cellPhoneNumber))
          {
            if (!person.GetContacts.Any(s =>
              s.Contact?.ContactType.ContactTypeId == (int)General.ContactType.CellNo && s.Contact?.Value == cellPhoneNumber))
            {
              var contact = new Contact(unitOfWork)
              {
                ContactType = unitOfWork.Query<ContactType>().First(s => s.ContactTypeId == (int)General.ContactType.CellNo),
                CreatedDT = DateTime.Now,
                Value = cellPhoneNumber,
                IsActive = true
              };

              new PER_Contacts(unitOfWork)
              {
                Person = person,
                Contact = contact
              };
              //person.Contacts.Add(contact);
            }
          }
          #endregion

          #region Save e-mail address
          if (!string.IsNullOrWhiteSpace(personDetails.EMailAddress))
          {
            var emailAddress = personDetails.EMailAddress.TrimEnd();

            if (!person.GetContacts.Any(s =>
                s.Contact?.ContactType.ContactTypeId == (int)General.ContactType.Email && s.Contact?.Value == emailAddress))
            {
              var newEmail = new Contact(unitOfWork)
              {
                ContactType = unitOfWork.Query<ContactType>().First(s => s.ContactTypeId == (int)General.ContactType.Email),
                CreatedDT = DateTime.Now,
                Value = emailAddress,
                IsActive = true
              };

              new PER_Contacts(unitOfWork)
              {
                Person = person,
                Contact = newEmail
              };
              //person.Contacts.Add(newEmail);
            }

            person.Email = emailAddress;
          }
          #endregion

          if (!string.IsNullOrWhiteSpace(personDetails.LegacyOperatorId))
          {
            var personSecurity = unitOfWork.Query<PER_Security>().FirstOrDefault(s => s.Person == person);
            if (personSecurity == null)
            {
              personSecurity = new PER_Security(unitOfWork);
              personSecurity.Person = person;
              personSecurity.CreatedDT = DateTime.Now;
            }

            personSecurity.LegacyOperatorId = personDetails.LegacyOperatorId;
            personSecurity.IsActive = true;
            personSecurity.LastEditedDT = DateTime.Now;
            person.Security = personSecurity;
          }

          unitOfWork.CommitChanges();
        }
        #endregion

        personId = person.PersonId;
        log.Information("{MethodName} completed successfully with PersonId: {PersonId}", methodName, personId);
        return (int)General.WCFCallResult.OK;
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        errorMessage = FPActivation.SERVER_ERR_UNEXPECTED;
        return (int)General.WCFCallResult.ServerError;
      }

    }
  }
}
