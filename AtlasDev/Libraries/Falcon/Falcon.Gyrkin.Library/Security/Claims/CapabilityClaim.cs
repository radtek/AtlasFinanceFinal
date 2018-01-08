using System;
using System.Collections.Generic;

namespace Falcon.Gyrkin.Library.Security.Claims
{
  public class CapabilityClaim : BaseClaim
  {
    public const string Name = "CapabilityClaim";
    private readonly List<Department> departments;

    public CapabilityClaim(List<Department> departments)
    {
      this.departments = departments;
    }

    public List<Department> Departments
    {
      get { return departments; }
    }

    public override string ValueType()
    {
      return ClaimValueTypes.CapabilityClaim;
    }
  }

  public class Department
  {
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; }
  }
}