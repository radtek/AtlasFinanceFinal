/*
****************************************************************************************************
* IBScanMatcherApi_err
*
* DESCRIPTION:
*     Error codes for IBScanMatcher.
*     http://www.integratedbiometrics.com
*
* NOTES:
*     Copyright (c) Integrated Biometrics, 2013
*
* HISTORY:
*     2013/03/28  0.10.0b  Removed NFIQ error definition.
*                          Renumbered error definitions to eliminate overlap in values.
*     2014/08/19  0.10.1b  Reformatted.
*     2015/08/25  0.10.2b  Added error codes to support NBIS
****************************************************************************************************
*/

using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace IBscanMatcher
{
    public partial class MDLL
    {
        /*
        ****************************************************************************************************
        * GENERIC ERROR CODES
        ****************************************************************************************************
        */
        public const int IBSM_STATUS_OK                             = 0;  /* Function completed successfully. */
        public const int IBSM_ERR_INVALID_PARAM_VALUE               = -1;  /* Invalid parameter value. */
        public const int IBSM_ERR_MEM_ALLOC                         = -2;  /* Insufficient memory. */
        public const int IBSM_ERR_NOT_SUPPORTED                     = -3;  /* Requested functionality isn't supported. */
        public const int IBSM_ERR_FILE_OPEN                         = -4;  /* File (USB handle, pipe, or image file) open failed. */
        public const int IBSM_ERR_FILE_READ                         = -5;  /* File (USB handle, pipe, or image file) read failed. */
        public const int IBSM_ERR_RESOURCE_LOCKED                   = -6;  /* Failure due to a locked resource. */
        public const int IBSM_ERR_MISSING_RESOURCE                  = -7;  /* Failure due to a missing resource (e.g. DLL file). */
        public const int IBSM_ERR_INVALID_ACCESS_POINTER            = -8;  /* Invalid access pointer address. */
        public const int IBSM_ERR_THREAD_CREATE                     = -9;  /* Thread creation failed. */
        public const int IBSM_ERR_COMMAND_FAILED                    = -10;  /* Generic command execution failed. */
        public const int IBSM_ERR_FILE_SAVE                         = -11;  /* File save failed (e.g. usb handle, pipe, image file ,,,). */

        /*
        ****************************************************************************************************
        * NBIS-RELATED ERROR CODES
        ****************************************************************************************************
        */
        public const int IBSM_ERR_NBIS_NFIQ_FAILED                  = -500;  /* Getting NFIQ score failed. */
        public const int IBSM_ERR_NBIS_WSQ_ENCODE_FAILED            = -501;  /* WSQ encode failed. */
        public const int IBSM_ERR_NBIS_WSQ_DECODE_FAILED            = -502;  /* WSQ decode failed. */
        public const int IBSM_ERR_NBIS_PNG_ENCODE_FAILED            = -503;  /* PNG encode failed. */
        public const int IBSM_ERR_NBIS_PNG_DECODE_FAILED            = -504;  /* PNG decode failed. */
        public const int IBSM_ERR_NBIS_JP2_ENCODE_FAILED            = -505;  /* JP2 encode failed. */
        public const int IBSM_ERR_NBIS_JP2_DECODE_FAILED            = -506;  /* JP2 decode failed. */
        public const int IBSM_ERR_NBIS_BMP_ENCODE_FAILED            = -507;  /* BMP encode failed. */
        public const int IBSM_ERR_NBIS_BMP_DECODE_FAILED            = -508;  /* BMP decode failed. */

        /*
        ****************************************************************************************************
        * LOW-LEVEL I/O ERROR CODES
        ****************************************************************************************************
        */
        public const int IBSM_ERR_OPEN_MATCHER_FAILED               = -600;
        public const int IBSM_ERR_CLOSE_MATCHER_FAILED              = -601;
        public const int IBSM_ERR_NO_MATCHER_INSTANCE               = -602;
        public const int IBSM_ERR_INVALID_HANDLE                    = -603;

        public const int IBSM_ERR_EXTRACTION_FAILED                 = -604;
        public const int IBSM_ERR_ENROLLMENT_FAILED                 = -605;
        public const int IBSM_ERR_MATCHING_FAILED                   = -606;
        public const int IBSM_ERR_COMPRESSION_FAILED                = -607;
        public const int IBSM_ERR_DECOMPRESSION_FAILED              = -608;
        public const int IBSM_ERR_CONVERT_FAILED                    = -609;
        public const int IBSM_ERR_THERE_IS_NO_DATA                  = -610;
        public const int IBSM_ERR_NOT_SUPPORTED_FUNCTION            = -611;
        public const int IBSM_ERR_NOT_SUPPORTED_IMAGE_FORMAT        = -612;
        public const int IBSM_ERR_NOT_SUPPORTED_DEVICE_TYPE         = -613;

        public const int IBSM_ERR_INCORRECT_ISO_FILE                = -700;
    }
}
