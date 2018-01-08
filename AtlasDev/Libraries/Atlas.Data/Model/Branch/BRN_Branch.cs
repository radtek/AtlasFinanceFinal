
/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for Branch
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
  public class BRN_Branch : XPCustomObject
  {
    private Int64 _BranchId;
    [Key(AutoGenerate = true)]
    public Int64 BranchId
    {
      get { return _BranchId; }
      set { SetPropertyValue("BranchId", ref _BranchId, value); }
    }

    private CPY_Company _Company;
    [Persistent, Indexed]
    public CPY_Company Company
    {
      get { return _Company; }
      set { SetPropertyValue("Company", ref _Company, value); }
    }

    private NUC_NuCardProfile _DefaultNuCardProfile;
    [Persistent]
    public NUC_NuCardProfile DefaultNuCardProfile
    {
      get { return _DefaultNuCardProfile; }
      set { SetPropertyValue("DefaultNuCardProfile", ref _DefaultNuCardProfile, value); }
    }

    private Region _Region;
    [Persistent, Indexed]
    public Region Region
    {
      get { return _Region; }
      set { SetPropertyValue("Region", ref _Region, value); }
    }

    private string _LegacyBranchNum;
    [Persistent, Indexed]
    public string LegacyBranchNum
    {
      get { return _LegacyBranchNum; }
      set { SetPropertyValue("LegacyBranchNum", ref _LegacyBranchNum, value); }
    }

    private DateTime? _OpenDT;
    [Persistent]
    public DateTime? OpenDT
    {
      get { return _OpenDT; }
      set { SetPropertyValue("OpenDT", ref _OpenDT, value); }
    }

    private DateTime? _CloseDT;
    [Persistent]
    public DateTime? CloseDT
    {
      get { return _CloseDT; }
      set { SetPropertyValue("CloseDT", ref _CloseDT, value); }
    }


    private string _Comment;
    [Persistent]
    public string Comment
    {
      get { return _Comment; }
      set { SetPropertyValue("Comment", ref _Comment, value); }
    }

    private bool _isClosed;
    [Persistent]
    public bool IsClosed
    {
      get { return _isClosed; }
      set { SetPropertyValue("IsClosed", ref _isClosed, value); }
    }

    private PER_Person _CreatedBy;
    [Persistent]
    public PER_Person CreatedBy
    {
      get { return _CreatedBy; }
      set { SetPropertyValue("CreatedBy", ref _CreatedBy, value); }
    }

    private PER_Person _DeletedBy;
    [Persistent]
    public PER_Person DeletedBy
    {
      get { return _DeletedBy; }
      set { SetPropertyValue("DeletedBy", ref _DeletedBy, value); }
    }

    private DateTime? _CreatedDT;
    [Persistent]
    public DateTime? CreatedDT
    {
      get { return _CreatedDT; }
      set { SetPropertyValue("CreatedDT", ref _CreatedDT, value); }
    }

    private List<CPY_Branches> _companies;
    [NonPersistent]
    public List<CPY_Branches> GetCompanies
    {
      get
      {
        if (_companies == null)
        {
          if (Session != null && Session.IsConnected)
          {
            _companies = Session.Query<CPY_Branches>().Where(b => b.Branch.BranchId == BranchId).ToList();
          }
        }
        return _companies;
      }
    }

    private List<BRN_Contacts> _contacts;
    [NonPersistent]
    public List<BRN_Contacts> GetContacts
    {
      get
      {
        if (_contacts == null)
        {
          if (Session != null && Session.IsConnected)
          {
            _contacts = Session.Query<BRN_Contacts>().Where(b => b.Branch.BranchId == BranchId).ToList();
          }
        }
        return _contacts;
      }
    }

    private List<BRN_Config> _Configs;
    [NonPersistent]
    public List<BRN_Config> GetConfigs
    {
      get
      {
        if (_Configs == null)
        {
          if (Session != null && Session.IsConnected)
          {
            _Configs = Session.Query<BRN_Config>().Where(b => b.Branch.BranchId == BranchId).ToList();
          }
        }
        return _Configs;
      }
    }

    private List<PER_Person> _persons;
    [NonPersistent]
    public List<PER_Person> GetPersons
    {
      get
      {
        if (_persons == null)
        {
          if (Session != null && Session.IsConnected)
          {
            _persons = Session.Query<PER_Person>().Where(b => b.Branch.BranchId == BranchId).ToList();
          }
        }
        return _persons;
      }
    }

    #region Constructors

    public BRN_Branch() : base() { }
    public BRN_Branch(Session session) : base(session) { }

    #endregion

    #region Public Methods

    //public List<BranchCompany> GetBranches()
    //{
    //  List<BranchCompany> branchCompany = null;
    //  using (var UoW = new UnitOfWork())
    //  {
    //    branchCompany = new List<BranchCompany>();

    //    foreach (var branch in new XPQuery<BRN_Branch>(UoW).ToList())
    //    {
    //      branchCompany.Add(new BranchCompany() { BranchId = branch.BranchId, BranchName = string.Format("{0}", branch.Company.Name), BranchNo = ASSBranchCodeToGL(branch.LegacyBranchNum) });
    //    }
    //  }
    //  return branchCompany;
    //}

    public static string ASSBranchCodeToGL(string assBranchCode, int padto = 2)
    {
      string result = assBranchCode;
      result = result.Replace("A", "10");
      result = result.Replace("B", "11");
      result = result.Replace("C", "12");
      result = result.Replace("D", "13");
      result = result.Replace("E", "14");
      result = result.Replace("F", "15");
      result = result.Replace("G", "16");
      result = result.Replace("H", "17");
      result = result.Replace("I", "18");
      result = result.Replace("J", "19");
      result = result.Replace("K", "20");
      result = result.Replace("L", "21");
      result = result.Replace("M", "22");
      result = result.Replace("N", "23");
      result = result.Replace("O", "24");
      result = result.Replace("P", "25");

      if (result.Length < padto)
      {
        result = result.PadLeft(padto, '0');
      }
      return result;
    }


    //public static string ASSBranchCodeFromGL(string legacyBranchCode, int padto = 2)
    //{
    //  string result = legacyBranchCode;
    //  if (legacyBranchCode.TrimStart('0').Length > 2)
    //  {
    //    if (result.Substring(0, 2) == "10")
    //    {
    //      result = string.Format("A{0}", result.Substring(2));
    //    }

    //    if (result.Substring(0, 2) == "11")
    //    {
    //      result = string.Format("B{0}", result.Substring(2));
    //    }

    //    if (result.Substring(0, 2) == "12")
    //    {
    //      result = string.Format("C{0}", result.Substring(2));
    //    }

    //    if (result.Substring(0, 2) == "13")
    //    {
    //      result = string.Format("D{0}", result.Substring(2));
    //    }

    //    if (result.Substring(0, 2) == "14")
    //    {
    //      result = string.Format("E{0}", result.Substring(2));
    //    }

    //    if (result.Substring(0, 2) == "15")
    //    {
    //      result = string.Format("F{0}", result.Substring(2));
    //    }

    //    if (result.Substring(0, 2) == "16")
    //    {
    //      result = string.Format("G{0}", result.Substring(2));
    //    }

    //    if (result.Substring(0, 2) == "17")
    //    {
    //      result = string.Format("H{0}", result.Substring(2));
    //    }

    //    if (result.Substring(0, 2) == "18")
    //    {
    //      result = string.Format("I{0}", result.Substring(2));
    //    }
    //    if (result.Substring(0, 2) == "19")
    //    {
    //      result = string.Format("J{0}", result.Substring(2));
    //    }
    //    if (result.Substring(0, 2) == "20")
    //    {
    //      result = string.Format("K{0}", result.Substring(2));
    //    }
    //    if (result.Substring(0, 2) == "21")
    //    {
    //      result = string.Format("L{0}", result.Substring(2));
    //    }
    //    if (result.Substring(0, 2) == "22")
    //    {
    //      result = string.Format("M{0}", result.Substring(2));
    //    }
    //    if (result.Substring(0, 2) == "23")
    //    {
    //      result = string.Format("N{0}", result.Substring(2));
    //    }
    //  }
    //  
    //  if (result.Length < padto)
    //  {
    //    result = result.PadLeft(padto, '0');
    //  }

    //  return result;
    //}

    #endregion

  }
}
