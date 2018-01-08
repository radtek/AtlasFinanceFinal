using System;
using System.ServiceModel;


namespace Atlas.DocServer.WCF.Interface
{
  /// <summary>
  /// Document conversion- converts between document formats, using DevExpress printing system for text-based and GdPicture for image-based
  /// DX: i.e. DOCX/RTF/(M)HTML/Excel to PDF, DOCX to text, 
  /// GdPicture: BMP to PDF, PDF to TIFF, etc.  
  /// </summary>
  [ServiceContract(Namespace = "urn:Atlas/ASS/DocServer/Convert/2014/05")]
  public interface IDocumentConvertServer
  {
    /// <summary>
    /// Converts a document from one format to another (i.e. HTML/MHT/DOC/XLS/XLSX/DOCX/RTF to PDF/DOCX/RTF/HTML/TXT).
    /// Full graphic conversion support. Not 100% coverage between formats- only common conversion scenarios are 
    /// supported (i.e. TXT/CSV to anything else not supported).
    /// </summary>
    /// <param name="sourceFileFormat">The file format of the source document</param>
    /// <param name="source">The source data- if HTML/MHT, must be UTF8, if TXT- must be ASCII, else raw file bytes (i.e. PDF/PNG/DOCX, etc.)</param>
    /// <param name="destFileFormat">The required output file format</param>
    /// <param name="openSourcePassword">The password to open the source document (optional- PDF)</param>  
    /// <param name="destCompress">Compress the resulting byte array</param>
    /// <param name="docOptions">document formatting options (margins, etc.)</param>
    /// <param name="renderOptions">Rendering options</param>
    /// <returns>Byte array containing the destination file content, null if conversion failed- bad source file or conversion between file types not supported</returns>    
    [OperationContract]
    byte[] Convert(byte[] source, bool isCompressed, FileFormatEnums.FormatType sourceFileFormat,
      FileFormatEnums.FormatType destFileFormat,
      string openSourcePassword, DocOptions docOptions,
      RenderOptions renderOptions, bool destCompress);


    /// <summary>
    /// Adds PDF password/encryption to an existing PDF document
    /// </summary>
    /// <param name="source">Source PDF source file bytes</param>
    /// <param name="openPassword">PDF open password, if protected</param>        
    /// <param name="docOptions">Document formatting options</param>
    /// <param name="destCompress">Compres the result byte array</param>
    /// <param name="isCompressed"></param>    
    /// <returns>Byte array containing encrypted and password protected PDF, null if error</returns>    
    [OperationContract]
    byte[] PDFAddPassword(byte[] source, bool isCompressed, string openPassword, DocOptions docOptions, bool destCompress);


    /// <summary>
    /// Checks document can be read/opened
    /// </summary>
    /// <param name="sourceFileFormat">The file format of the source document</param>
    /// <param name="source">The source data- if HTML/MHT, must be UTF8, if TXT- must be ASCII, else raw file bytes (i.e. PDF/PNG/DOCX, etc.)</param>
    /// <param name="openPassword">The password to open the source document (optional)</param>
    /// <param name="isCompressed"></param>
    /// <returns>true if the source file byte data could be opened as specified by the document format</returns>
    [OperationContract]
    bool IsValidDocument(byte[] source, bool isCompressed, FileFormatEnums.FormatType sourceFileFormat, 
      
      
      string openPassword);

    /// <summary>
    /// Determines if the document contains any data. 
    /// If text-based (Excel, Word), will check for any text. If Word, will also check for images if not text found.
    /// If bitmap, will check for blank image. 
    /// If PDF, will check if file contains any images or any text.
    /// </summary>
    /// <param name="sourceFileFormat">The file format of the source document</param>
    /// <param name="source">The source file data</param>
    /// <param name="openPassword">The password to open the source document (optional for PDF)</param>
    /// <param name="isCompressed"true if the source file byte ></param>
    /// <returns>true if any content was found, false if no content</returns>
    [OperationContract]
    bool DocumentContainsData(byte[] source, bool isCompressed, FileFormatEnums.FormatType sourceFileFormat, 
      string openPassword);


    /// <summary>
    /// Tries to optimize a PDF by recompressing images and flattening/removing fields/bookmarks and digital 
    /// signatures and compressing the PDF.
    /// 
    /// If cannot optimize or an error while opening/processing, will return null
    /// </summary>
    /// <param name="source">The source PDF file as byte array</param>
    /// <param name="openPassword">The PDF open password</param>
    /// <param name="isCompressed">Is the source data compressed</param>
    /// <param name="flattenFields">Flatten fields (makes fields read-only)</param>
    /// <param name="removeAllAnnotations">Remove annotations</param>
    /// <param name="removeAllBookmarks">Remove bookmarks</param>
    /// <param name="removeAllLinks">Remove all links</param>
    /// <param name="removeAllSignatures">Remove all digital signatures</param>
    /// <param name="destCompress">Must the output data be compressed</param>
    /// <param name="options">Output parameters</param>
    /// <param name="jp2KQuality">JPEG2K image quality to use: 1-512, default is 16</param>
    /// <param name="setImagesBitDepth">Set all images bit depth (0- no set, 1- B&W, 8- Gray scale, 24- 24-bit)</param>
    /// <returns>byte array containing the optmized PDF, null if error, </returns>
    [OperationContract]
    byte[] PDFOptimize(byte[] source, bool isCompressed, string openPassword,
      bool flattenFields, bool removeAllBookmarks, bool removeAllSignatures, bool removeAllLinks, bool removeAllAnnotations,
      byte setImagesBitDepth, int jp2KQuality, DocOptions options, bool destCompress);


    /// <summary>
    /// Attempts to clean up a a scanned document using various smart GdPicture routines
    /// </summary>
    /// <param name="sourceFileFormat">The source file type</param>
    /// <param name="source">The source file</param>
    /// <param name="cleanUpOptions">Clean-up options to apply (required)</param>
    /// <param name="openPassword">The PDF open password</param>
    /// <param name="isCompressed">Is the source data compressed</param>
    /// <param name="destFileFormat">The destination file format</param>
    /// <param name="destCompress">true if the result byte array must be compressed</param>
    /// <returns>byte array containing cleaned-up document, null if error</returns>
    [OperationContract]
    byte[] CleanUpScan(byte[] source, bool isCompressed, FileFormatEnums.FormatType sourceFileFormat, string openPassword,
      CleanUpOptions cleanUpOptions, DocOptions docOptions, RenderOptions renderOptions,
      FileFormatEnums.FormatType destFileFormat, bool destCompress);


    /// <summary>
    /// Digitally sign a PDF (http://stackoverflow.com/questions/20445365/create-pkcs12-file-with-self-signed-certificate-via-openssl-in-windows-for-my-a)
    /// </summary>
    /// <param name="source">The source PDF file</param>
    /// <param name="openPassword">The PDF open password</param>
    /// <param name="isCompressed">Is source data compressed</param>    
    /// <param name="destCompress">Compress the result</param>
    /// <returns>byte array containing the signed PDF, null if error</returns>
    [OperationContract]
    byte[] PDFAddSignature(byte[] source,  bool isCompressed, string openPassword,DocOptions options, bool destCompress);
  }

}
