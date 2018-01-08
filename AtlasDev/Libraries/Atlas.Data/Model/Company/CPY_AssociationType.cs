
/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for CompanyAssociationType
 * 
 * 
 *  Author:
 *  ------------------
 *     Fabian Franco-Roldan
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  public class CPY_AssociationType : XPLiteObject
  {
    private Int64 _CompanyAssociationTypeId;
    [Persistent]
    [Key(AutoGenerate = true)]
    public Int64 CompanyAssociationTypeId
    {
      get
      {
        return _CompanyAssociationTypeId;
      }
      set
      {
        SetPropertyValue("CompanyAssociationTypeId", ref _CompanyAssociationTypeId, value);
      }
    }

    private CPY_Type _CompanyType;
    [Persistent]
    public CPY_Type CompanyType
    {
      get
      {
        return _CompanyType;
      }
      set
      {
        SetPropertyValue("CompanyType", ref _CompanyType, value);
      }
    }

    private CPY_Company _Company;
    [Persistent]
    public CPY_Company Company
    {
      get
      {
        return _Company;
      }
      set
      {
        SetPropertyValue("Company", ref _Company, value);
      }
    }

    private DateTime? _CreatedDT;
    [Persistent]
    public DateTime? CreatedDT
    {
      get
      {
        return _CreatedDT;
      }
      set
      {
        SetPropertyValue("CreatedDT", ref _CreatedDT, value);
      }
    }

    #region Constructors

    public CPY_AssociationType() : base() { }
    public CPY_AssociationType(Session session) : base(session) { }

    #endregion

    #region Public Methods

    //[Obsolete("This should not be in the domain- this is a repository function")]
    //public List<CPY_CompanyDTO> GetCompaniesByCompanyType(Enumerators.General.CompanyType companyType)
    //{
    //  var companyCollection = new List<CPY_Company>();
    //  List<CPY_CompanyDTO> convertedCollection = null;
    //  using (var UoW = new UnitOfWork())
    //  {
    //    var association = new XPQuery<CPY_AssociationType>(UoW).Where(o => o.CompanyType.Type == companyType);
    //    foreach (var assoc in association)
    //    {
    //      companyCollection.Add(assoc.Company);
    //    }
    //    convertedCollection = AutoMapper.Mapper.Map<List<CPY_Company>, List<CPY_CompanyDTO>>(companyCollection);
    //  }
    //  return convertedCollection;
    //}

    #endregion

  }
}
