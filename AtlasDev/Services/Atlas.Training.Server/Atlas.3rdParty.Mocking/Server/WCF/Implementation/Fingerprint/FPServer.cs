using System;
using System.Collections.Generic;

using Atlas.WCF.FPServer.Interface;
using Atlas.WCF.FPServer.Security.Interface;


namespace Atlas.Server.Training
{
  internal class FPServer : IFPServer
  {
    public int CancelEnrollPerson(SourceRequest sourceRequest, long startEnrollRef, out string errorMessage)
    {
      throw new NotImplementedException();
    }


    public int EndEnrollPerson(SourceRequest sourceRequest, long startEnrollRef, out string errorMessage)
    {
      throw new NotImplementedException();
    }


    public int EnrollFingerprint(SourceRequest sourceRequest, long startEnrollRef, List<FPRawBufferDTO> fpBitmaps, out string errorMessage)
    {
      throw new NotImplementedException();
    }


    public int EnrollPerson(SourceRequest sourceRequest, FPScannerInfoDTO scanner, FPScannerOptionDTO scannerOptions, FPRawBufferDTO[] fpBitmaps, bool isStaff, out string errorMessage)
    {
      throw new NotImplementedException();
    }


    public int GetPersonScanOptions(SourceRequest sourceRequest, FPScannerInfoDTO scanner, long personId, out FPScannerOptionDTO scanOptions, out string errorMessage)
    {
      throw new NotImplementedException();
    }


    public int GetPersonVerifyOptions(SourceRequest sourceRequest, FPScannerInfoDTO scanner, long personId, out FPVerifyOptionsDTO verifyOptions, out string errorMessage)
    {
      throw new NotImplementedException();
    }


    public int GetTemplatesForPerson(SourceRequest sourceRequest, long personId, out List<FPTemplateDTO> templates, out string errorMessage)
    {
      throw new NotImplementedException();
    }


    public int IdentifyPerson(SourceRequest sourceRequest, FPScannerInfoDTO scanner, FPScannerOptionDTO scannerOptions, FPRawBufferDTO[] compressedImages, out BasicPersonDetailsDTO person, out string errorMessage)
    {
      throw new NotImplementedException();
    }


    public int StartEnrollPerson(SourceRequest sourceRequest, FPScannerInfoDTO scanner, FPScannerOptionDTO scannerOptions, long personId, out long startEnrollRef, out string errorMessage)
    {
      throw new NotImplementedException();
    }


    public int UnEnrollPerson(SourceRequest sourceRequest, long personId, out string errorMessage)
    {
      throw new NotImplementedException();
    }

  }
}
