namespace Atlas.Business.Security.Role
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using Atlas.Domain.Model;
  using Atlas.Domain.DTO;
  using DevExpress.Xpo;
  using Atlas.Domain.Security;

  /// <summary>
  /// Various role related functions
  /// </summary>
  public static class RoleContext
  {
    #region Internal Cached Roles

    internal static List<PER_RoleDTO> _cachedPersonRole = null;
    internal static PER_PersonDTO _person = null;

    #endregion

    #region Public Members

    public static PER_PersonDTO LoggedInUser = null;

    #endregion

    /// <summary>
    /// Builds a role context
    /// </summary>
    public static void InitializeRoleContext(long personId)
    {
      //// cache has already been built ignore
      if (_cachedPersonRole != null)
        return;

      using (var UoW = new UnitOfWork())
      {

        var person = new XPQuery<PER_Person>(UoW).FirstOrDefault(o => o.PersonId == personId);

        //person.Security.app
        //person.Security.MachineStore = new XPQuery<MachineStore>(UoW).FirstOrDefault( e => e.User == person.Security);

        var mappedPerson = AutoMapper.Mapper.Map<PER_Person, PER_PersonDTO>(person);
        _person = mappedPerson;
        LoggedInUser = mappedPerson;

        if (person != null && person.GetRoles.Count > 0)
        {
          var personRolesCollection = new XPQuery<PER_Role>(UoW).Where(o
                                                     => o.Person.PersonId == personId).ToList();

          if (_cachedPersonRole == null)
          {
            _cachedPersonRole = new List<PER_RoleDTO>();
          }
          _cachedPersonRole.AddRange(AutoMapper.Mapper.Map<List<PER_Role>, List<PER_RoleDTO>>(personRolesCollection));
        }
        else
        {
          _cachedPersonRole = new List<PER_RoleDTO>();
        }
      }
    }

    /// <summary>
    /// Returns a collection of person roles
    /// </summary>
    /// <param name="personId">Person of which you want to look up</param>
    /// <returns>Collection</returns>
    public static List<PER_RoleDTO> GetPersonRoles(long personId)
    {
      using (var UoW = new UnitOfWork())
      {
        var personRolesCollection = new XPQuery<PER_Role>(UoW).Where(
                                                       o => o.Person.PersonId == personId).ToList();

        if (personRolesCollection.Count > 0)
        {
          return AutoMapper.Mapper.Map<List<PER_Role>, List<PER_RoleDTO>>(personRolesCollection);
        }
        return null;
      }
    }

    /// <summary>
    /// Checks to see if a user belongs to a role
    /// Note: Causes a round trip to the database.
    /// </summary>
    /// <param name="personId">Person to check against</param>
    /// <param name="roleType">Role to check</param>
    public static bool HasRole(Enumerators.General.RoleType roleType)
    {
      using (var UoW = new UnitOfWork())
      {
        var result = new XPQuery<PER_Role>(UoW).FirstOrDefault(o
                               => o.RoleType.Type == roleType && o.Person.PersonId == _person.PersonId);
        if (result == null)
        {
          return false;
        }
        else
        {
          return true;
        }
      }
    }

    /// <summary>
    /// Gets role from cached role context
    /// </summary>
    /// <param name="personId">Person to check against</param>
    /// <param name="roleType">Role to check</param>
    public static bool HasRole(Enumerators.General.RoleType roleType, bool forceCacheRebuild = false)
    {
      if (_cachedPersonRole == null)
        throw new NullReferenceException("Internal role cache has not been built");

      List<Enumerators.General.RoleType> roleTypes = new List<Enumerators.General.RoleType>();
      roleTypes.Add(roleType);
      roleTypes.Add(Enumerators.General.RoleType.Administrator);

      var result = _cachedPersonRole.Where(o => roleTypes.Contains(o.RoleType.Type) && o.Person.PersonId == _person.PersonId);
      if (result.Count() == 0)
      {
        return false;
      }
      else
      {
        return true;
      }
    }

    /// <summary>
    /// Returns a collection of person's associated with a particular role type
    /// </summary>
    /// <param name="roleType">Role to search</param>
    public static List<PER_RoleDTO> GetPersonWithRole(Enumerators.General.RoleType roleType)
    {
      using (var UoW = new UnitOfWork())
      {
        var result = new XPQuery<PER_Role>(UoW).Where(o => o.RoleType.Type == roleType).ToList();
        
        return result.Count == 0 ? null : AutoMapper.Mapper.Map<List<PER_Role>, List<PER_RoleDTO>>(result);
      }
    }
  }
}
