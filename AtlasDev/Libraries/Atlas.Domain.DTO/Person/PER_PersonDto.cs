using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Atlas.Domain.DTO.Nucard;


namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class PER_PersonDTO
  {
    [DataMember]
    public Int64 PersonId { get;set;}
    [DataMember]
    public PER_SecurityDTO Security { get;set;}
    [DataMember]
    public PER_TypeDTO PersonType  { get;set;}
    [DataMember]
    public BRN_BranchDTO Branch { get;set;}
    [DataMember]
    public CPY_CompanyDTO Employer { get; set; }
    [DataMember]
    public string LegacyClientCode  { get;set;}
    [DataMember]
    public string ClientCode  { get;set;}
    [DataMember]
    public string Designation  { get;set;}
    [DataMember]
    public string Firstname  { get;set;}
    [DataMember]
    public string Middlename  { get;set;}
    [DataMember]
    public string Lastname  { get;set;}
    [DataMember]
    public string Othername  { get;set;}
    [DataMember]
    public string Email  { get;set;}
    [DataMember]
    public string IdNum  { get;set;}
    [DataMember]
    public string SalaryFrequency  { get;set;}
    [DataMember]
    public string Gender  { get;set;}
    [DataMember]
    public string Race  { get;set;}
    [DataMember]
    public HostDTO Host  { get;set;}
    [DataMember]
    public DateTime DateOfBirth  { get;set;}
    [DataMember]
    public PER_PersonDTO CreatedBy  { get;set;}
    [DataMember]
    public DateTime? CreatedDT { get;set;}
    [DataMember]
    public DateTime? LastEditedDT { get; set; }

    #region Get below as needed after mapping

    [DataMember]
    public List<BankDetailDTO> BankDetails { get; set; }
    [DataMember]
    public List<AddressDTO> AddressDetails { get; set; }
    [DataMember]
    public List<ContactDTO> Contacts { get; set; }
    [DataMember]
    public List<NUC_NuCardDTO> Cards { get; set; }
    [DataMember]
    public List<PER_RelationDTO> Relations { get; set; }
    [DataMember]
    public List<CPY_CompanyDTO> EmploymentHistory { get; set; }

#endregion
  }
}