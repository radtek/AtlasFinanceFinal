using System;
using System.ComponentModel;


namespace Atlas.Enumerators
{
  public class Document
  {
    /// <summary>
    /// File format type, i.e. JPEG, PNG, etc.
    /// </summary>
    public enum FileFormat
    {
      [Description("Undefined")]
      NotSet = 0,
      
      [Description("ZIP")]
      ZIP = 1,
      
      [Description("Text")]
      TXT = 2,
      
      [Description("Comma-separated values")]
      CSV = 3,
      
      [Description("Adobe Acrobat")]
      PDF = 4,
      
      [Description("HTML")]
      HTML = 5,
      
      [Description("MHT")]
      MHT = 6,
      
      [Description("Rich Text Format")]
      RTF = 11,
      
      [Description("TIFF")]
      TIFF = 12,
      
      [Description("Windows Bitmap")]
      BMP = 13,
      
      [Description("Portable Network Graphic")]
      PNG = 14,
      
      [Description("JPEG")]
      JPEG = 15,
      
      [Description("JPEG 2000")]
      JPEG2K = 16,
      
      [Description("Enhanced metafile")]
      EMF = 17,

      [Description("Microsoft Excel (binary)")]
      XLS = 20,
      
      [Description("Microsoft Excel (XML)")]
      XLSX = 21,
      
      [Description("Microsoft Word (binary)")]
      DOC = 22,
      
      [Description("Microsoft Word (XML)")]
      DOCX = 23,

      [Description("OpenOffice Spreadsheet")]
      ODS = 30,
      
      [Description("OpenOffice Word processing")]
      ODT = 31,

      [Description("DevExpress REPX")]
      REPX = 40,
      
      [Description("DevExpress PRNX- Report Document")]
      PRNX = 41,
      
      [Description("DevExpres SNX- Snap Document")]
      SNX = 42
    };


    /// <summary>
    /// The source generator of the document
    /// </summary>
    public enum Generator
    {
      [Description("Undefined")]
      NotSet = 0,
      [Description("Scanner")]
      Scanner = 1,
      [Description("Third party")]
      ThirdParty = 2,
      [Description("Atlas Document Generator")]
      AtlasDocGen = 3,
      [Description("email")]
      email = 4,
      [Description("Fax")]
      Fax = 5
    };


    /// <summary>
    /// The 'general' document category- i.e. quote, application form, etc.
    /// </summary>
    public enum Category
    {
      [Description("Undefined")]
      NotSet = 0,

      [Description("Quote")]
      Quote = 1,
           
      [Description("Application form")]
      Application = 2,

      [Description("Contract")]
      Contract = 3,

      [Description("Receipt/invoice")]
      Receipt = 4,

      [Description("Insurance")]
      Insurance = 5,

      [Description("Affordability")]
      Afford = 6,   
  
      [Description("NuCard")]
      NuCard = 7,

      [Description("Employment (payslip, contract, etc.)")]
      Employment = 8,

      [Description("Bank (statement, proof of account hoder, etc.)")]
      Bank = 9,

      [Description("Identification (SA ID book, passport, driver's license, temporary id)")]
      Identification = 10,
      
      [Description("Statement")]
      Statement = 11,

      [Description("Scorecard")]
      Scorecard = 12,

      [Description("AVS")]
      AVS = 13,

      [Description("Identity verification/validation")]
      IDVerification = 14,

      [Description("Voucher")]
      Voucher = 15
    };


    /// <summary>
    /// A 'specific' document type- i.e. 'Ass quote', 'Ass contract'.
    /// For ease of use/lookups, the first part of the enum name must be made up of 'Category'
    /// </summary>
    public enum DocumentTemplate
    {
      [Description("Undefined")]
      NotSet = 0,

      [Description("Quote- ASS Quick")]
      Quote_AssQuick = 1,

      [Description("Quote- ASS")]
      Quote_ASS = 2,

      [Description("Application- ASS Credit")]
      Application_ASSCredit = 3,
      
      [Description("Contract- ASS")]
      Contract_ASS = 4,

      [Description("Insurance- ASS Policy")]
      Insurance_ASSPolicy = 5,

      [Description("Insurance- ASS Wording")]
      Insurance_ASSWording = 6,

      [Description("Affordability- ASS")]
      Afford_ASS = 7,

      [Description("Receipt- NuCard- ASS")]
      Receipt_ASSNuCard = 8,
      
      [Description("Quote- VAP Beneficiary")]
      Insurance_VAPBeneficiary = 9,

      [Description("Contract- VAP")]
      Contract_VAP = 10,

      [Description("Quote- VAP")]
      Quote_VAP = 11,

      [Description("Insurance- VAP Wording")]
      Insurance_VAPWording = 12
    }
  }
}
