using Atlas.Domain.Model;
using Atlas.Enumerators;
using Atlas.Orchestration.Server.Structures;
using DevExpress.Xpo;
using log4net;
using System;
using System.Linq;
using Atlas.Common.Extensions;

namespace Atlas.Orchestration.Server.Helper
{
  public sealed class Save
  {
    private static readonly ILog Log = LogManager.GetLogger(typeof(Save));

    public long? BankDetailId { get; set; }
    public long? PersonId { get; set; }

    public void Process(Person client)
    {
      PER_Person person;
      CPY_Company clientEmployer;
      using (var uow = new UnitOfWork())
      {
        person = new XPQuery<PER_Person>(uow).FirstOrDefault(_ => _.IdNum == client.IdNo);
        clientEmployer = new XPQuery<CPY_Company>(uow).FirstOrDefault(_ => _.Name == client.Employer.Name);
      }

      var employer = SaveEmployer(client, clientEmployer == null ? (long?)null : clientEmployer.CompanyId);
      SaveClient(client, employer.CompanyId, person == null ? (long?)null : person.PersonId);
    }

    private void SaveAddress(Address addressAO, long companyId)
    {
      using (var uow = new UnitOfWork())
      {
        var company = new XPQuery<CPY_Company>(uow).FirstOrDefault(c => c.CompanyId == companyId);
        if (company != null)
        {
          var address =
            company.GetAddresses.Select(a => a.Address).FirstOrDefault(o => o.AddressType.AddressTypeId == addressAO.AddressType.ToInt()
                                                                            && o.Line1 == addressAO.Line1
                                                                            && o.Line2 == addressAO.Line2
                                                                            && o.Line3 == addressAO.Line3
                                                                            && o.Line4 == addressAO.Line4
                                                                            && o.PostalCode == addressAO.Code
                                                                            && o.Province.ProvinceId == addressAO.Province.ToInt());

          if (address == null)
          {
            address = new ADR_Address(uow);
            address.AddressType = new XPQuery<ADR_Type>(uow).First(_ => _.AddressTypeId == General.AddressType.Employer.ToInt());
            address.Line1 = addressAO.Line1;
            address.Line2 = addressAO.Line2;
            address.Line3 = addressAO.Line3;
            address.Line4 = addressAO.Line4;
            address.PostalCode = addressAO.Code;
            address.Province = new XPQuery<Province>(uow).First(_ => _.ProvinceId == addressAO.Province.ToInt());
            address.IsActive = true;
            address.CreatedDT = DateTime.Now;

            var cpyAddress = new CPY_Addresses(uow)
            {
              Address = address,
              Company = company
            };

            uow.CommitChanges();
          }
        }
      }
    }

    private void SaveContact(Structures.Contact contactAo, long companyId)
    {
      using (var uow = new UnitOfWork())
      {
        var company = new XPQuery<CPY_Company>(uow).FirstOrDefault(c => c.CompanyId == companyId);
        if (company != null)
        {
          var contact =
              company.GetContacts.Select(c => c.Contact)
                  .FirstOrDefault(p => p.ContactType.ContactTypeId == contactAo.ContactType.ToInt()
                                       && p.Value == contactAo.No);

          if (contact == null)
          {
            contact = new Domain.Model.Contact(uow)
            {
              ContactType =
                    new XPQuery<ContactType>(uow).FirstOrDefault(
                        _ => _.ContactTypeId == contactAo.ContactType.ToInt()),
              IsActive = true,
              CreatedDT = DateTime.Now,
              Value = contactAo.No
            };

            var cpyContact = new CPY_Contacts(uow)
            {
              Company = company,
              Contact = contact
            };
            uow.CommitChanges();
          }
        }

      }
    }

    private CPY_Company SaveEmployer(Person client, long? clientEmployerCompanyId)
    {
      Log.Info("SaveEmployer - Started");
      CPY_Company clientEmployer;

      using (var uow = new UnitOfWork())
      {
        if (clientEmployerCompanyId == null)
        {
          clientEmployer = new CPY_Company(uow) { Name = client.Employer.Name };

          uow.CommitChanges();
        }
        else
        {
          clientEmployer = new XPQuery<CPY_Company>(uow).FirstOrDefault(c => c.CompanyId == clientEmployerCompanyId);
        }
      }
      if (clientEmployer != null)
      {
        foreach (var cpyAddress in client.Employer.Addresses)
        {
          SaveAddress(cpyAddress, clientEmployer.CompanyId);
        }

        foreach (var cpyContact in client.Employer.Contacts)
        {
          SaveContact(cpyContact, clientEmployer.CompanyId);
        }
      }

      Log.Info("SaveEmployer - Ended");
      return clientEmployer;
    }

    private void SaveClient(Person client, long clientEmployerId, long? clientPersonId)
    {
      PER_Person person;
      using (var uow = new UnitOfWork())
      {
        person = clientPersonId.HasValue ? new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == clientPersonId) : new PER_Person(uow);

        if (person != null)
        {
          person.Designation = client.Title;
          person.Firstname = client.FirstName;
          person.Lastname = client.LastName;
          person.IdNum = client.IdNo;
          person.DateOfBirth = client.DateOfBirth;
          person.Gender = client.Gender == General.Gender.Male ? "M" : "F";
          person.Email = client.Email;
                    //Edited By Prashant
                    //person.Host = new XPQuery<Host>(uow).First(_ => _.Type == client.Host);
                    person.Host = new XPQuery<Host>(uow).First(_ => _.HostId == (int)client.Host);
                    person.Employer = new XPQuery<CPY_Company>(uow).FirstOrDefault(c => c.CompanyId == clientEmployerId);
          // Save Contacts

          foreach (var clientContact in client.Contacts)
          {
            var perContact =
              new XPQuery<PER_Contacts>(uow).FirstOrDefault(
                c =>
                  c.Person.PersonId == person.PersonId &&
                  c.Contact.ContactType.ContactTypeId == clientContact.ContactType.ToInt() &&
                  c.Contact.Value == clientContact.No);

            if (perContact == null)
            {
              var contact = new Domain.Model.Contact(uow);
              contact.ContactType = new XPQuery<ContactType>(uow).FirstOrDefault(p => p.ContactTypeId == clientContact.ContactType.ToInt());
              contact.IsActive = true;
              contact.CreatedDT = DateTime.Now;
              contact.Value = clientContact.No;

              perContact = new PER_Contacts(uow)
              {
                Contact = contact,
                Person = person
              };
            }
          }

          // Save Address
          foreach (var clientAddress in client.Addresses)
          {
            var perAddress = new XPQuery<PER_AddressDetails>(uow).FirstOrDefault(p => p.Person.PersonId == person.PersonId
              && p.Address.AddressType.AddressTypeId == clientAddress.AddressType.ToInt()
                                     && p.Address.Line1 == clientAddress.Line1
                                     && p.Address.Line2 == clientAddress.Line2
                                     && p.Address.Line3 == clientAddress.Line3
                                     && p.Address.PostalCode == clientAddress.Code
                                     && p.Address.Province.ProvinceId == clientAddress.Province.ToInt());

            if (perAddress == null)
            {
              var address = new ADR_Address(uow);
              address.AddressType = new XPQuery<ADR_Type>(uow).First(_ => _.AddressTypeId == clientAddress.AddressType.ToInt());
              address.Line1 = clientAddress.Line1;
              address.Line2 = clientAddress.Line2;
              address.Line3 = clientAddress.Line3;
              address.Line4 = clientAddress.Line4;
              address.PostalCode = clientAddress.Code;
              address.Province = new XPQuery<Province>(uow).First(_ => _.ProvinceId == clientAddress.Province.ToInt());
              address.IsActive = true;
              address.CreatedDT = DateTime.Now;

              perAddress = new PER_AddressDetails(uow)
              {
                Address = address,
                Person = person
              };

            }
          }
          uow.CommitChanges();
        }
      }

      if (person != null)
      {
        new Business.General.Person().GetClientCode(person.PersonId);

        SaveRelation(client, person.PersonId);

        SaveBank(client, person.PersonId);

        PersonId = person.PersonId;
      }
    }

    private void SaveRelation(Person client, long personId)
    {
      using (var uow = new UnitOfWork())
      {
        var relation = client.Relation.FirstName.Split(null);

        var person = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == personId);
        if (person != null)
        {
          var narrowedRelation = person.GetRelations.FirstOrDefault(
              p =>
                p.RelationPerson.Firstname == client.Relation.FirstName &&
                p.RelationPerson.Lastname == client.Relation.LastName
                && p.Relation.Type == client.Relation.RelationType);

          if (narrowedRelation == null)
          {
            var relationPerson = new PER_Person(uow);

            if (relation.Length >= 2)
            {
              relationPerson.Firstname = relation[0];
              relationPerson.Lastname = relation[1];
            }
            else
            {
              relationPerson.Firstname = client.Relation.FirstName;
              relationPerson.Lastname = client.Relation.LastName;
            }
            relationPerson.CreatedDT = DateTime.Now;
                        //Edited By Prashant
                        //relationPerson.Host = new XPQuery<Host>(uow).First(p => p.Type == client.Host);
                        relationPerson.Host = new XPQuery<Host>(uow).First(p => p.HostId == (int)client.Host);

                        uow.CommitChanges();

            foreach (var relationContact in client.Relation.Contacts)
            {
              var contact = new Domain.Model.Contact(uow);
              contact.ContactType = new XPQuery<ContactType>(uow).First(p => p.ContactTypeId == relationContact.ContactType.ToInt());
              contact.Value = relationContact.No;

              var perContact = new PER_Contacts(uow)
              {
                Contact = contact,
                Person = person
              };
            }

            var personRelation = new PER_Relation(uow);
            personRelation.Person = person;
                        //Edited By Prashant
                        //personRelation.Relation = new XPQuery<PER_RelationType>(uow).First(p => p.Type == client.Relation.RelationType);
                        personRelation.Relation = new XPQuery<PER_RelationType>(uow).First(p => p.RelationTypeId == (int)client.Relation.RelationType);
                        personRelation.RelationPerson = relationPerson;

            person.GetRelations.Add(personRelation);

            uow.CommitChanges();
          }
        }
      }
    }

    private void SaveBank(Person client, long personId)
    {
      using (var uow = new UnitOfWork())
      {
        var person = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == personId);

        if (person != null)
        {
                    var bankDetail =
            person.GetBankDetails.Select(b => b.BankDetail)
              .FirstOrDefault(o => o.AccountNum == client.BankDetail.AccountNo
                                   && o.AccountName == client.BankDetail.AccountName
                                   //Edited By Prashant
                                   //&& o.AccountType.Type == client.BankDetail.AccountType
                                   //&& o.Bank.Type == client.BankDetail.Bank
                                   && o.AccountType.AccountTypeId == (int)client.BankDetail.AccountType
                                   && o.Bank.BankId == (int)client.BankDetail.Bank
                                   && o.IsActive);

          if (bankDetail == null)
          {
                        //Edited By Prashant
                        //var bank = new XPQuery<BNK_Bank>(uow).First(_ => _.Type == client.BankDetail.Bank);
                        var bank = new XPQuery<BNK_Bank>(uow).First(_ => _.BankId == (int)client.BankDetail.Bank);

                        bankDetail = new BNK_Detail(uow)
            {
              AccountNum = client.BankDetail.AccountNo,
              AccountName = client.BankDetail.AccountName,
              Code = bank.UniversalCode,
              Bank = bank,
              IsActive = false,
              CreatedDT = DateTime.Now
            };
                        //Edited By Prashant
                        //            bankDetail.AccountType =
                        //  new XPQuery<BNK_AccountType>(uow).First(_ => _.Type == client.BankDetail.AccountType);
                        //bankDetail.Period = new XPQuery<BNK_Period>(uow).First(p => p.Type == client.BankDetail.Period);
                        bankDetail.AccountType =
                          new XPQuery<BNK_AccountType>(uow).First(_ => _.AccountTypeId == (int)client.BankDetail.AccountType);
                        bankDetail.Period = new XPQuery<BNK_Period>(uow).First(p => p.PeriodId == (int)client.BankDetail.Period);

                        var perBankDetail = new PER_BankDetails(uow)
            {
              Person = person,
              BankDetail = bankDetail
            };

            uow.CommitChanges();
            BankDetailId = bankDetail.DetailId;
          }
          else
          {
            BankDetailId = bankDetail.DetailId;
          }
        }
      }
    }
  }
}