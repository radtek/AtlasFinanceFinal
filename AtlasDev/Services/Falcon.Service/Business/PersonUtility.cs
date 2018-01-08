using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Common.Extensions;
using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using Atlas.Enumerators;
using AutoMapper;
using DevExpress.Xpo;
using Falcon.Common.Structures;
using Falcon.Common.Structures.Branch;
using Contact = Atlas.Domain.Model.Contact;
using ContactType = Atlas.Domain.Model.ContactType;
using Province = Atlas.Domain.Model.Province;
using Region = Falcon.Common.Structures.Region;

namespace Falcon.Service.Business
{
  public static class PersonUtility
  {
    public static AccountContact AddContact(long personId, General.ContactType contactType, string value)
    {
      using (var uow = new UnitOfWork())
      {
        var person = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == personId);

        if (person == null)
          throw new Exception("Person does not exist in DB");

        var newContact = new Contact(uow)
        {
          ContactType = new XPQuery<ContactType>(uow).First(c => c.ContactTypeId == contactType.ToInt()),
          Value = value,
          IsActive = true,
          CreatedDT = DateTime.Now
        };

        var perContact = new PER_Contacts(uow)
        {
          Person = person,
          Contact = newContact
        };

        uow.CommitChanges();

        var accountContact = new AccountContact()
        {
          ContactId = newContact.ContactId,
          ContactType = newContact.ContactType.Description,
          ContactTypeId = newContact.ContactType.ContactTypeId,
          CreateDate = newContact.CreatedDT,
          IsActive = newContact.IsActive,
          Value = newContact.Value
        };

        return accountContact;
      }
    }

    public static AccountContact DisableContact(long personId, long contactId)
    {
      using (var uow = new UnitOfWork())
      {
        var person = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == personId);
        if (person == null)
          throw new Exception("Person not found in DB");

        var contact = person.GetContacts.FirstOrDefault(c => c.Contact.ContactId == contactId);
        if (contact != null)
        {
          contact.Contact.IsActive = false;

          uow.CommitChanges();

          return new AccountContact()
          {
            ContactId = contact.Contact.ContactId,
            ContactType = contact.Contact.ContactType.Description,
            ContactTypeId = contact.Contact.ContactType.ContactTypeId,
            CreateDate = contact.Contact.CreatedDT,
            IsActive = contact.Contact.IsActive,
            Value = contact.Contact.Value
          };
        }
        return null;
      }
    }

    public static AccountAddress AddAddress(long personId, long userPersonId, General.AddressType addressType,
      General.Province province, string line1, string line2, string line3, string line4, string postalCode)
    {
      using (var uow = new UnitOfWork())
      {
        var person = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == personId);

        if (person == null)
          throw new Exception("Person does not exist in DB");

        var newAddress = new ADR_Address(uow)
        {
          AddressType = new XPQuery<ADR_Type>(uow).First(a => a.AddressTypeId== addressType.ToInt()),
          CreatedBy = new XPQuery<PER_Person>(uow).First(p => p.PersonId == userPersonId),
          CreatedDT = DateTime.Now,
          IsActive = true,
          Line1 = line1,
          Line2 = line2,
          Line3 = line3,
          Line4 = line4,
          PostalCode = postalCode,
          Province = new XPQuery<Province>(uow).First(p => p.ProvinceId == province.ToInt())
        };

        var perAddress = new PER_AddressDetails(uow)
        {
          Person = person,
          Address =  newAddress
        };

        uow.CommitChanges();

        var accountAddress = new AccountAddress()
        {
          AddressId = newAddress.AddressId,
          AddressType = newAddress.AddressType.Description,
          AddressTypeId = newAddress.AddressType.AddressTypeId,
          AddressLine1 = newAddress.Line1,
          AddressLine2 = newAddress.Line2,
          AddressLine3 = newAddress.Line3,
          AddressLine4 = newAddress.Line4,
          PostalCode = newAddress.PostalCode,
          IsActive = newAddress.IsActive,
          CreateDate = newAddress.CreatedDT
        };
        if (newAddress.Province != null)
        {
          accountAddress.Province = newAddress.Province.Description;
          accountAddress.ProvinceId = newAddress.Province.ProvinceId;
        }

        return accountAddress;
      }
    }

    public static AccountAddress DisableAddress(long personId, long addressId)
    {
      using (var uow = new UnitOfWork())
      {
        var person = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == personId);
        if (person == null)
          throw new Exception("Person not found in DB");

        var address = person.GetAddressDetails.Select(a=>a.Address).FirstOrDefault(a => a.AddressId == addressId);
        if (address != null)
        {
          address.IsActive = false;

          uow.CommitChanges();

          var accountAddress = new AccountAddress()
          {
            AddressId = address.AddressId,
            AddressType = address.AddressType.Description,
            AddressTypeId = address.AddressType.AddressTypeId,
            AddressLine1 = address.Line1,
            AddressLine2 = address.Line2,
            AddressLine3 = address.Line3,
            AddressLine4 = address.Line4,
            PostalCode = address.PostalCode,
            IsActive = address.IsActive,
            CreateDate = address.CreatedDT
          };
          if (address.Province != null)
          {
            accountAddress.Province = address.Province.Description;
            accountAddress.ProvinceId = address.Province.ProvinceId;
          }
          return accountAddress;
        }
        return null;
      }
    }

    public static Relation CreateNewRelation(long personId, long userPersonId, string firstname, string lastname,
      string cellNo, General.RelationType relationType)
    {
      using (var uow = new UnitOfWork())
      {
        var person = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == personId);
        if (person == null)
          throw new Exception("Person does not exist in DB");

        var createUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == userPersonId);
        if (createUser == null)
          throw new Exception("User does not exist in DB");

        var relative = new PER_Person(uow)
        {
          Firstname = firstname,
          Lastname = lastname,
          CreatedDT = DateTime.Now,
          CreatedBy = createUser
        };

        var contact = new Contact(uow)
        {
          ContactType = new XPQuery<ContactType>(uow).First(c => c.ContactTypeId == General.ContactType.CellNo.ToInt()),
          Value = cellNo,
          IsActive = true,
          CreatedDT = DateTime.Now
        };

        var perContact = new PER_Contacts(uow)
        {
          Person =  relative,
          Contact = contact
        };

        person.GetRelations.Add(new PER_Relation(uow)
        {
          Person = person,
          RelationPerson = relative,
          Relation = new XPQuery<PER_RelationType>(uow).FirstOrDefault(r => r.Type == relationType)
        });

        uow.CommitChanges();

        return new Relation()
        {
          CellNo = cellNo,
          FirstName = relative.Firstname,
          LastName = relative.Lastname,
          PersonId = relative.PersonId,
          RelationTypeId = (int) relationType,
          RelationType = relationType.ToStringEnum()
        };
      }
    }

    public static Relation UpdateRelation(long personId, long relationPersonId, string firstname, string lastname,
      string cellNo, General.RelationType relationType)
    {
      using (var uow = new UnitOfWork())
      {
        var person = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == personId);
        if (person == null)
          throw new Exception("Person does not exist in DB");

        var relative = person.GetRelations.FirstOrDefault(p => p.RelationPerson.PersonId == relationPersonId);
        if (relative == null)
          throw new Exception("Relationship does not exist in DB");

        var perContact =
          relative.RelationPerson.GetContacts.FirstOrDefault(
            c => c.Contact.ContactType.ContactTypeId== General.ContactType.CellNo.ToInt() && c.Contact.IsActive);
        if (perContact == null)
        {
          var newContact  =new Contact(uow)
          {
            ContactType = new XPQuery<ContactType>(uow).First(c => c.ContactTypeId== General.ContactType.CellNo.ToInt()),
            CreatedDT = DateTime.Now,
            IsActive = true,
            Value = cellNo
          };

          perContact = new PER_Contacts(uow)
          {
            Person = relative.RelationPerson,
            Contact = newContact
          };
        }
        else
        {
          perContact.Contact.Value = cellNo;
        }

        relative.RelationPerson.Firstname = firstname;
        relative.RelationPerson.Lastname = lastname;
        relative.Relation = new XPQuery<PER_RelationType>(uow).FirstOrDefault(r => r.Type == relationType);

        uow.CommitChanges();

        return new Relation()
        {
          CellNo = cellNo,
          FirstName = relative.RelationPerson.Firstname,
          LastName = relative.RelationPerson.Lastname,
          PersonId = relative.RelationPerson.PersonId,
          RelationTypeId = (int) relationType,
          RelationType = relationType.ToStringEnum()
        };
      }
    }

    public static PER_PersonDTO LocateByIdentityNo(string idNo)
    {
      using (var uow = new UnitOfWork())
      {
        var person = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.IdNum.Trim() == idNo.Trim());
        if (person == null)
          return null;
        return Mapper.Map<PER_Person, PER_PersonDTO>(person);
      }
    }

    public static long? VerifyIsValid(string idNo, string cellNo)
    {
      using (var uow = new UnitOfWork())
      {
        var person = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.IdNum.Trim() == idNo.Trim());

        if (person == null)
          return null;

        if (person.GetContacts.All(p => p.Contact.Value.Trim() != cellNo.Trim()))
          return null;

        return person.PersonId;
      }
    }

    public static Dictionary<Region, List<Branch>> GetRegionBranches(long personId)
    {
      var regionBranches = new Dictionary<Region, List<Branch>>();
      using (var uow = new UnitOfWork())
      {
        var branchRegions =
          new XPQuery<PER_Person>(uow).Where(p => p.PersonId == personId)
            .Select(
              p => new {p.Branch.BranchId, p.Branch.Company.Name, p.Branch.Region.RegionId, p.Branch.Region.Description})
            .ToList();
        var regions = branchRegions.Select(r => new {r.RegionId, r.Description}).Distinct().ToList();
        foreach (var region in regions)
        {
          var branches =
            branchRegions.Where(rb => rb.RegionId == region.RegionId)
              .Select(b => new {b.BranchId, b.Name})
              .ToList()
              .Select(
                branchRegion =>
                  new Branch
                  {
                    BranchId = branchRegion.BranchId,
                    Name = branchRegion.Name,
                    Region = region.Description,
                    RegionId = region.RegionId
                  })
              .ToList();
          regionBranches.Add(new Region {RegionId = region.RegionId, Description = region.Description}, branches);
        }
      }

      return regionBranches;
    }
  }
}