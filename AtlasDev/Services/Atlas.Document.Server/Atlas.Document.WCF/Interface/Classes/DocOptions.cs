using System;
using System.Runtime.Serialization;


namespace Atlas.DocServer.WCF.Interface
{
  [Serializable]
  [DataContract(Namespace = "urn:Atlas/ASS/DocServer/Convert/2014/05")]  
  /// <summary>
  /// Contains document options (mainly PDF, but 'Subject' field is used in other formats (i.e. sheet name in Excel)
  /// </summary>  
  public sealed class DocOptions
  {
    public DocOptions()
    {
      CanAddNotes = true;
      CanAssemble = true;
      CanCopy = true;
      CanCopyAccess = true;
      CanFillFields = true;
      CanModify = false;
      CanPrint = true;
      CanPrintFull = true;
    }


    [DataMember]
    public string UserPassword { get; set; }    // User password to open (PDF) 
    
    [DataMember]
    public string OwnerPassword { get; set; }   // Owner password- used for PDF ownwer password and Excel protection  

    [DataMember]
    public string Title { get; set; }

    [DataMember]
    public string Author { get; set; }

    [DataMember]
    public string Subject { get; set; }

    [DataMember]
    public string Creator { get; set; }

    [DataMember]
    public string Producer { get; set; }

    [DataMember]
    public string Generator { get; set; }

    [DataMember]
    public string Keywords { get; set; }

    [DataMember]
    public bool CanPrint { get; set; }

    [DataMember]
    public bool CanCopy { get; set; }

    [DataMember]
    public bool CanModify { get; set; }

    [DataMember]
    public bool CanAddNotes { get; set; }

    [DataMember]
    public bool CanFillFields { get; set; }

    [DataMember]
    public bool CanCopyAccess { get; set; }

    [DataMember]
    public bool CanAssemble { get; set; }

    [DataMember]
    public bool CanPrintFull { get; set; }    
  }
}