using System;
using System.Collections.Generic;
using System.Xml.Serialization;


namespace Atlas.ThirdParty.CompuScan.Batch.XML
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public partial class trans_data
    {
      #region Registrations

      [XmlElement("CSREG_CLIENT")]
      public List<CSREG_CLIENT> CSREG_CLIENT { get; set; }

      [XmlElement("CSREG_LOAN")]
      public List<CSREG_LOAN> CSREG_LOAN { get; set; }

      [XmlElement("CSREG_PAYMENT")]
      public List<CSREG_PAYMENT> CSREG_PAYMENT { get; set; }

      [XmlElement("CSREG_ADDRESS")]
      public List<CSREG_ADDRESS> CSREG_ADDRESS { get; set; }

      [XmlElement("CSREG_TELEPHONE")]
      public List<CSREG_TELEPHONE> CSREG_TELEPHONE { get; set; }

      [XmlElement("CSREG_EMPLOYER")]
      public List<CSREG_EMPLOYER> CSREG_EMPLOYER { get; set; }

      #endregion   

      [XmlElement("CSUPD_CLIENT")]
      public List<CSUPD_CLIENT> CSUPD_CLIENT { get; set; }


      #region Enquiry

      [XmlElement("CSENQ_GLOBAL2")]
      public List<CSENQ_GLOBAL2> CSENQ_GLOBAL2 { get; set; }

      [XmlElement("CSENQ_CONFLICT")]
      public List<CSENQ_CONFLICT> CSENQ_CONFLICT { get; set; }

      #endregion


      #region NLR

      [XmlElement("NLR_LOANREG")]
      public List<NLR_LOANREG> NLR_LOANREG { get; set; }

      [XmlElement("NLR_LOANREG2")]
      public List<NLR_LOANREG2> NLR_LOANREG2 { get; set; }

      [XmlElement("NLR_LOANCLOSE")]
      public List<NLR_LOANCLOSE> NLR_LOANCLOSE { get; set; }

      [XmlElement("NLR_BATB2")]
      public List<NLR_BATB2> NLR_BATB2 { get; set; }

      #endregion


      #region Updates
      
      [XmlElement("CSUPD_LOAN")]
      public List<CSUPD_LOAN> CSUPD_LOAN { get; set; }

      #endregion


      #region Should They Serialize

      public bool ShouldSerializeCSREG_CLIENT()
      {
        return CSREG_CLIENT != null;
      }


      public bool ShouldSerializeCSREG_LOAN()
      {
        return CSREG_LOAN != null;
      }


      public bool ShouldSerializeCSREG_PAYMENT()
      {
        return CSREG_PAYMENT != null;
      }


      public bool ShouldSerializeCSREG_ADDRESS()
      {
        return CSREG_ADDRESS != null;
      }


      public bool ShouldSerializeCSREG_TELEPHONE()
      {
        return CSREG_TELEPHONE != null;
      }


      public bool ShouldSerializeCSREG_EMPLOYER()
      {
        return CSREG_EMPLOYER != null;
      }


      public bool ShouldSerializeCSUPD_CLIENT()
      {
        return CSUPD_CLIENT != null;
      }


      public bool ShouldSerializeCSUPD_LOAN()
      {
        return CSUPD_LOAN != null;
      }


      public bool ShouldSerializeCSENQ_GLOBAL2()
      {
        return CSENQ_GLOBAL2 != null;
      }


      public bool ShouldSerializeCSENQ_CONFLICT()
      {
        return CSENQ_CONFLICT != null;
      }


      public bool ShouldSerializeNLR_LOANREG()
      {
        return NLR_LOANREG != null;
      }


      public bool ShouldSerializeNLR_LOANCLOSE()
      {
        return NLR_LOANCLOSE != null;
      }


      public bool ShouldSerializeNLR_BATB2()
      {
        return NLR_BATB2 != null;
      }

      #endregion

    }
  }
