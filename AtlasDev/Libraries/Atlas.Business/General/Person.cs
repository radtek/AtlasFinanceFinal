using System;
using System.Linq;
using Atlas.Common.ExceptionBase;
using Atlas.Domain.Model;
using DevExpress.Xpo;

namespace Atlas.Business.General
{
  public class Person
  {
    /// <summary>
    /// Generates client code for person
    /// </summary>
    /// <param name="personId">Person on which to generate a client code.</param>
    /// <returns></returns>
    public string GetClientCode(long personId)
    {
      using (var uow = new UnitOfWork())
      {
        var person = new XPQuery<PER_Person>(uow).FirstOrDefault(_ => _.PersonId == personId);

        if (person == null)
          throw new RecordNotFoundException("Person does not exist in the database");

				if (string.IsNullOrEmpty(person.ClientCode))
				{
          var code = string.Empty;

					code = person.Lastname.Substring(0, Math.Min(person.Lastname.Length, 3));
					person.ClientCode = string.Format("{0}{1}{2}", code.ToUpper(), person.Host.HostId.ToString("D2"), person.PersonId.ToString("D10"));
					uow.CommitChanges();
				}

        return person.ClientCode;
      }
    }
  }
}