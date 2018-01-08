/*
****************************************************************************************************
* IBScanMatcherApi
*
* DESCRIPTION:
*     API functions for IBScanMatcher.
*     http://www.integratedbiometrics.com
*
* NOTES:
*     Copyright (c) Integrated Biometrics, 2013-2014
*
* HISTORY:
*     2014/08/19  0.10.1b  Reformatted.
*                          Added IBSU_FreeMemory to release the allocated memory block
*                          on the internal heap of library.
*                          This is obtained by IBSM_OpenFIR(), IBSM_OpenFMR() and other API functions.
****************************************************************************************************
*/

using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;
using IBscanUltimate;

namespace IBscanMatcher
{
    public partial class MDLL
    {
        /*
        ****************************************************************************************************
        * GLOBAL FUNCTIONS
        ****************************************************************************************************
        */

        /*
        ****************************************************************************************************
        * IBSM_GetSDKVersion()
        * 
        * DESCRIPTION:
        *     Obtain product and software version information.
        *
        * ARGUMENTS:
        *     pVerinfo  Pointer to structure that will receive SDK version information.
        *
        * RETURNS:
        *     IBSM_STATUS_OK, if successful.
        *     Error code < 0, otherwise.  See error codes in 'IBScanMatcherApi_err'.
        ****************************************************************************************************
        */
        [DllImport("IBScanMatcher.DLL")]
        private static extern int IBSM_GetSDKVersion(
            ref IBSM_SDKVersion pVerinfo                        
                                                                
            );
        public static int _IBSM_GetSDKVersion(
            ref IBSM_SDKVersion pVerinfo
            )
        {
            int nRc = IBSM_STATUS_OK;
            try
            {
                nRc = IBSM_GetSDKVersion(ref pVerinfo);
            }
            catch (Exception except)
            {
                Trace.WriteLine("IBSM_GetSDKVersion : " + except.Message);
            }

            return nRc;
        }

        /*
        ****************************************************************************************************
        * IBSM_GetMatcherCount()
        * 
        * DESCRIPTION:
        *     Get the number of open matchers.
        *
        * ARGUMENTS:
        *     pMatcherCount  (output) Pointer to integer that will receive number of open matchers.
        *
        * RETURNS:
        *     IBSM_STATUS_OK, if successful.
        *     Error code < 0, otherwise.  See error codes in 'IBScanMatcherApi_err'.
        ****************************************************************************************************
        */
        [DllImport("IBScanMatcher.DLL")]
        private static extern int IBSM_GetMatcherCount(
            ref int pMatcherCount                            
                                                               
            );
        public static int _IBSM_GetMatcherCount(
            ref int pMatcherCount
            )
        {
            int nRc = IBSM_STATUS_OK;
            try
            {
                nRc = IBSM_GetMatcherCount(ref pMatcherCount);
            }
            catch (Exception except)
            {
                Trace.WriteLine("IBSM_GetMatcherCount : " + except.Message);
            }

            return nRc;
        }

        /*
        ****************************************************************************************************
        * IBSM_OpenMatcher()
        * 
        * DESCRIPTION:
        *     Open a new matcher.
        *
        * ARGUMENTS:
        *     matcher_handle  (output) Pointer to integer that will receive matcher handle
        *
        * RETURNS:
        *     IBSM_STATUS_OK, if successful.
        *     Error code < 0, otherwise.  See error codes in 'IBScanMatcherApi_err'.
        ****************************************************************************************************
        */
        [DllImport("IBScanMatcher.DLL")]
        private static extern int IBSM_OpenMatcher(
            ref int     matcher_handle                                  
                                                                
            );
        public static int _IBSM_OpenMatcher(
            ref int     matcher_handle
            )
        {
            int nRc = IBSM_STATUS_OK;
            try
            {
                nRc = IBSM_OpenMatcher(ref matcher_handle);
            }
            catch (Exception except)
            {
                Trace.WriteLine("IBSM_OpenMatcher : " + except.Message);
            }

            return nRc;
        }

        /*
        ****************************************************************************************************
        * IBSM_CloseMatcher()
        * 
        * DESCRIPTION:
        *     Close a matcher.
        *
        * ARGUMENTS:
        *     matcher_handle  Matcher handle.
        *
        * RETURNS:
        *     IBSM_STATUS_OK, if successful.
        *     Error code < 0, otherwise.  See error codes in 'IBScanMatcherApi_err'.
        ****************************************************************************************************
        */
        [DllImport("IBScanMatcher.DLL")]
        private static extern int IBSM_CloseMatcher(
            int  handle                                 
            );
        public static int _IBSM_CloseMatcher(
            int handle
            )
        {
            int nRc = IBSM_STATUS_OK;
            try
            {
                nRc = IBSM_CloseMatcher(handle);
            }
            catch (Exception except)
            {
                Trace.WriteLine("IBSM_CloseMatcher : " + except.Message);
            }

            return nRc;
        }

        /*
        ****************************************************************************************************
        * IBSM_CloseAllMatchers()
        * 
        * DESCRIPTION:
        *     Close all matchers.
        *
        * RETURNS:
        *     IBSM_STATUS_OK, if successful.
        *     Error code < 0, otherwise.  See error codes in 'IBScanMatcherApi_err'.
        ****************************************************************************************************
        */
        [DllImport("IBScanMatcher.DLL")]
        private static extern int IBSM_CloseAllMatchers(
            );
        public static int _IBSM_CloseAllMatchers(
            )
        {
            int nRc = IBSM_STATUS_OK;
            try
            {
                nRc = IBSM_CloseAllMatchers();
            }
            catch (Exception except)
            {
                Trace.WriteLine("IBSM_CloseAllMatchers : " + except.Message);
            }

            return nRc;
        }

        /*
        ****************************************************************************************************
        * IBSM_IsMatcherOpened()
        * 
        * DESCRIPTION:
        *     Determine whether the matcher handle belongs to an open matcher.
        *
        * ARGUMENTS:
        *     matcher_handle  Matcher handle.
        *
        * RETURNS:
        *     IBSM_STATUS_OK, if device is open/initialized.
        *     Error code < 0, otherwise.  See error codes in 'IBScanMatcherApi_err'.
        ****************************************************************************************************
        */
        [DllImport("IBScanMatcher.DLL")]
        private static extern int IBSM_IsMatcherOpened(
            int  matcher_handle                                 
            );
        public static int _IBSM_IsMatcherOpened(
            int matcher_handle
            )
        {
            int nRc = IBSM_STATUS_OK;
            try
            {
                nRc = IBSM_IsMatcherOpened(matcher_handle);
            }
            catch (Exception except)
            {
                Trace.WriteLine("IBSM_IsMatcherOpened : " + except.Message);
            }

            return nRc;
        }

        /*
        ****************************************************************************************************
        * IBSM_ExtractTemplate()
        * 
        * DESCRIPTION:
        *     Extract template from image data.
        *
        * ARGUMENTS:
        *     matcher_handle  Matcher handle.
        *     in_image        Image data from which template will be extracted.
        *     out_template    (output) Pointer to structure into which template will be stored.
        *
        * RETURNS:
        *     IBSM_STATUS_OK, if successful.
        *     Error code < 0, otherwise.  See error codes in 'IBScanMatcherApi_err'.
        ****************************************************************************************************
        */
        [DllImport("IBScanMatcher.DLL")]
        private static extern int IBSM_ExtractTemplate(
            int               matcher_handle,                     
            DLL.IBSM_ImageData    in_image,                 
            ref IBSM_Template out_template               
            );
        public static int _IBSM_ExtractTemplate(
            int               matcher_handle,
            DLL.IBSM_ImageData in_image,                 
            ref IBSM_Template out_template               
            )
        {
            int nRc = IBSM_STATUS_OK;
            try
            {
                nRc = IBSM_ExtractTemplate(matcher_handle, in_image, ref out_template);
            }
            catch (Exception except)
            {
                Trace.WriteLine("IBSM_ExtractTemplate : " + except.Message);
            }

            return nRc;
        }

        /*
        ****************************************************************************************************
        * IBSM_GetProcessedImage()
        * 
        * DESCRIPTION:
        *     Get processed image data from previous extract template operation.
        *
        * ARGUMENTS:
        *     matcher_handle       Matcher handle.
        *     processed_imagetype  Type of processed image.
        *     pImageData           (output) Pointer to structure into which image data will be stored.
        *
        * RETURNS:
        *     IBSM_STATUS_OK, if successful.
        *     Error code < 0, otherwise.  See error codes in 'IBScanMatcherApi_err'.
        ****************************************************************************************************
        */
        [DllImport("IBScanMatcher.DLL")]
        private static extern int IBSM_GetProcessedImage(
            int                     matcher_handle,                     
            IBSM_ProcessedImageType processed_imagetype,
            ref DLL.IBSM_ImageData pImageData               
            );
        public static int _IBSM_GetProcessedImage(
            int                     matcher_handle,                     
            IBSM_ProcessedImageType processed_imagetype,
            ref DLL.IBSM_ImageData pImageData               
            )
        {
            int nRc = IBSM_STATUS_OK;
            try
            {
                nRc = IBSM_GetProcessedImage(matcher_handle, processed_imagetype, ref pImageData);
            }
            catch (Exception except)
            {
                Trace.WriteLine("IBSM_GetProcessedImage : " + except.Message);
            }

            return nRc;
        }

        /*
        ****************************************************************************************************
        * IBSM_MatchingTemplate()
        * 
        * DESCRIPTION:
        *     Determine whether two templates match.  The templates must match to the level set with
        *     IBSU_SetMatchingLevel().
        *
        * ARGUMENTS:
        *     matcher_handle  Matcher handle.
        *     in_template1    First template to compare.
        *     in_template2    Second template to compare.
        *     matching_score  (output) Pointer to variable that will receive matching score.  If match is
        *                     better than the configured matching level, a non-zero matching score will be
        *                     returned.  Otherwise, a positive matching score will be returned.
        *
        * RETURNS:
        *     IBSM_STATUS_OK, if successful.
        *     Error code < 0, otherwise.  See error codes in 'IBScanMatcherApi_err'.
        ****************************************************************************************************
        */
        [DllImport("IBScanMatcher.DLL")]
        private static extern int IBSM_MatchingTemplate(
            int           matcher_handle,
            IBSM_Template in_template1,
            IBSM_Template in_template2,
            ref int       matching_score
        );
        public static int _IBSM_MatchingTemplate(
            int           matcher_handle,
            IBSM_Template in_template1,
            IBSM_Template in_template2,
            ref int       matching_score
            )
        {
            int nRc = IBSM_STATUS_OK;
            
            try
            {
                nRc = IBSM_MatchingTemplate(matcher_handle, in_template1, in_template2, ref matching_score);
            }
            catch (Exception except)
            {
                Trace.WriteLine("IBSM_MatchingTemplate : " + except.Message);            
            }
            
            return nRc;
        }

        /*
        ****************************************************************************************************
        * IBSM_SetMatchingLevel()
        * 
        * DESCRIPTION:
        *     Set matching level.  This level is used when matching templates (with IBSM_MatchingTemplate())
        *     or generating enrollment templates (with IBSM_SingleEnrollment() or IBSM_MultiEnrollment()).
        *
        * ARGUMENTS:
        *     matcher_handle  Matcher handle.
        *     matching_level  Matching level required, between 1 (loosest) and 7 (strictest), inclusive.
        *
        * RETURNS:
        *     IBSM_STATUS_OK, if successful.
        *     Error code < 0, otherwise.  See error codes in 'IBScanMatcherApi_err'.
        ****************************************************************************************************
        */
        [DllImport("IBScanMatcher.DLL")]
        private static extern int IBSM_SetMatchingLevel(
            int     matcher_handle,
            int     matching_level
            );
        public static int _IBSM_SetMatchingLevel(
            int     matcher_handle,
            int     matching_level
            )
        {
            int nRc = IBSM_STATUS_OK;
            try
            {
                nRc = IBSM_SetMatchingLevel(matcher_handle, matching_level);
            }
            catch (Exception except)
            {
                Trace.WriteLine("IBSM_SetMatchingLevel : " + except.Message);
            }

            return nRc;
        }

        /*
        ****************************************************************************************************
        * IBSM_GetMatchingLevel()
        * 
        * DESCRIPTION:
        *     Get matching level.  This level is used when matching templates (with IBSM_MatchingTemplate())
        *     or generating enrollment templates (with IBSM_SingleEnrollment() or IBSM_MultiEnrollment()).
        *
        * ARGUMENTS:
        *     matcher_handle  Matcher handle.
        *     matching_level  (output) Pointer to integer that will receive matching level required,
        *                     between 1 (loosest) and 7 (strictest), inclusive.
        *
        * RETURNS:
        *     IBSM_STATUS_OK, if successful.
        *     Error code < 0, otherwise.  See error codes in 'IBScanMatcherApi_err'.
        ****************************************************************************************************
        */
        [DllImport("IBScanMatcher.DLL")]
        private static extern int IBSM_GetMatchingLevel(
            int     matcher_handle,
            ref int matching_level
            );
        public static int _IBSM_GetMatchingLevel(
            int     matcher_handle,
            ref int matching_level
            )
        {
            int nRc = IBSM_STATUS_OK;
            try
            {
                nRc = IBSM_GetMatchingLevel(matcher_handle, ref matching_level);
            }
            catch (Exception except)
            {
                Trace.WriteLine("IBSM_GetMatchingLevel : " + except.Message);
            }

            return nRc;
        }

        /*
        ****************************************************************************************************
        * IBSM_SingleEnrollment()
        * 
        * DESCRIPTION:
        *     Generate one enrollment template from three image data.  Templates extracted from each pair of
        *     images must match to the level set with IBSM_SetMatchingLevel().  The template extracted from the
        *     most representative image will be returned.
        *
        * ARGUMENTS:
        *     matcher_handle  Matcher handle.
        *     in_image1       First image data.
        *     in_image2       Second image data.
        *     in_image3       Third image data.
        *     out_template    (output) Pointer to structure that will receive enrollment template.
        *
        * RETURNS:
        *     IBSM_STATUS_OK, if successful.
        *     Error code < 0, otherwise.  See error codes in 'IBScanMatcherApi_err'.
        ****************************************************************************************************
        */
        [DllImport("IBScanMatcher.DLL")]
        private static extern int IBSM_SingleEnrollment(
            int               matcher_handle,
            DLL.IBSM_ImageData in_image1,
            DLL.IBSM_ImageData in_image2,
            DLL.IBSM_ImageData in_image3,
            ref IBSM_Template out_template
            );
        public static int _IBSM_SingleEnrollment(
            int               matcher_handle,
            DLL.IBSM_ImageData in_image1,
            DLL.IBSM_ImageData in_image2,
            DLL.IBSM_ImageData in_image3,
            ref IBSM_Template out_template
            )
        {
            int nRc = IBSM_STATUS_OK;
            try
            {
                nRc = IBSM_SingleEnrollment(matcher_handle, in_image1, in_image2, in_image3, ref out_template);
            }
            catch (Exception except)
            {
                Trace.WriteLine("IBSM_SingleEnrollment : " + except.Message);
            }

            return nRc;
        }

        /*
        ****************************************************************************************************
        * IBSM_MultiEnrollment()
        * 
        * DESCRIPTION:
        *     Generate two enrollment templates from six image data.  Templates extracted from sufficient pairs
        *     images will be expected to match to the level set with IBSM_SetMatchingLevel().  The templates
        *     extracted from the most representative image will be returned.
        *
        * ARGUMENTS:
        *     handle  Device handle.
        *
        * RETURNS:
        *     IBSM_STATUS_OK, if successful.
        *     Error code < 0, otherwise.  See error codes in 'IBScanMatcherApi_err'.
        ****************************************************************************************************
        */
        [DllImport("IBScanMatcher.DLL")]
        private static extern int IBSM_MultiEnrollment(
            int               matcher_handle,
            DLL.IBSM_ImageData in_image1,
            DLL.IBSM_ImageData in_image2,
            DLL.IBSM_ImageData in_image3,
            DLL.IBSM_ImageData in_image4,
            DLL.IBSM_ImageData in_image5,
            DLL.IBSM_ImageData in_image6,
            ref IBSM_Template out_template1,
            ref IBSM_Template out_template2
            );
        public static int _IBSM_MultiEnrollment(
            int               matcher_handle,
            DLL.IBSM_ImageData in_image1,
            DLL.IBSM_ImageData in_image2,
            DLL.IBSM_ImageData in_image3,
            DLL.IBSM_ImageData in_image4,
            DLL.IBSM_ImageData in_image5,
            DLL.IBSM_ImageData in_image6,
            ref IBSM_Template out_template1,
            ref IBSM_Template out_template2
            )
        {
            int nRc = IBSM_STATUS_OK;
            try
            {
                nRc = IBSM_MultiEnrollment(matcher_handle, in_image1, in_image2, in_image3,
                                           in_image4, in_image5, in_image6, ref out_template1, ref out_template2);
            }
            catch (Exception except)
            {
                Trace.WriteLine("IBSM_MultiEnrollment : " + except.Message);
            }

            return nRc;
        }

        /*
        ****************************************************************************************************
        * IBSM_CompressImage()
        * 
        * DESCRIPTION:
        *     Compress image data.
        *
        * ARGUMENTS:
        *     matcher_handle   Matcher handle.
        *     inimage          Image data to compress.
        *     outimage         (output) Pointer to structure to receive compressed image data.
        *     compress_format  Format of compressed image data.
        *
        * RETURNS:
        *     IBSM_STATUS_OK, if successful.
        *     Error code < 0, otherwise.  See error codes in 'IBScanMatcherApi_err'.
        ****************************************************************************************************
        */
        [DllImport("IBScanMatcher.DLL")]
        private static extern int IBSM_CompressImage(
            int                matcher_handle,
            DLL.IBSM_ImageData inimage,
            ref DLL.IBSM_ImageData outimage,
            DLL.IBSM_ImageFormat compress_format
            );
        public static int _IBSM_CompressImage(
            int                matcher_handle,
            DLL.IBSM_ImageData inimage,
            ref DLL.IBSM_ImageData outimage,
            DLL.IBSM_ImageFormat compress_format
            )
        {
            int nRc = IBSM_STATUS_OK;
            try
            {
                nRc = IBSM_CompressImage(matcher_handle, inimage, ref outimage, compress_format);
            }
            catch (Exception except)
            {
                Trace.WriteLine("IBSM_CompressImage : " + except.Message);
            }

            return nRc;
        }

        /*
        ****************************************************************************************************
        * IBSM_DecompressImage()
        * 
        * DESCRIPTION:
        *     Decompress image data.
        *
        * ARGUMENTS:
        *     matcher_handle   Matcher handle.
        *     inimage          Image data to decompress.
        *     outimage         (output) Pointer to structure to receive decompressed image data.
        *
        * RETURNS:
        *     IBSM_STATUS_OK, if successful.
        *     Error code < 0, otherwise.  See error codes in 'IBScanMatcherApi_err'.
        ****************************************************************************************************
        */
        [DllImport("IBScanMatcher.DLL")]
        private static extern int IBSM_DecompressImage(
            int                matcher_handle,
            DLL.IBSM_ImageData inimage,
            ref DLL.IBSM_ImageData outimage
            );
        public static int _IBSM_DecompressImage(
            int                matcher_handle,
            DLL.IBSM_ImageData inimage,
            ref DLL.IBSM_ImageData outimage
            )
        {
            int nRc = IBSM_STATUS_OK;
            try
            {
                nRc = IBSM_DecompressImage(matcher_handle, inimage, ref outimage);
            }
            catch (Exception except)
            {
                Trace.WriteLine("IBSM_DecompressImage : " + except.Message);
            }

            return nRc;
        }

        /*
        ****************************************************************************************************
        * IBSM_ConvertTemplate_ISOtoIBSM()
        * 
        * DESCRIPTION:
        *     Convert template from ISO/IEC 19794-2 FMR (Fingerprint Minutiae Record) format to IB's template
        *     format.
        *
        * ARGUMENTS:
        *     matcher_handle      Matcher handle.
        *     in_template         Template to convert.
        *     out_template        (output) Pointer to memory that will receive a pointer to an array of
        *                         templates.
        *     out_template_count  (output) Pointer to integer that will receive the number of templates in
        *                         the array of templates.
        *
        * RETURNS:
        *     IBSM_STATUS_OK, if successful.
        *     Error code < 0, otherwise.  See error codes in 'IBScanMatcherApi_err'.
        ****************************************************************************************************
        const int matcher_handle,
        const ISO_FMR in_template,
        IBSM_Template **out_template,
        int *out_template_cnt
        */
        [DllImport("IBScanMatcher.DLL")]
        private static extern int IBSM_ConvertTemplate_ISOtoIBSM(
            int               matcher_handle,
            ISO_FMR           in_template,
            ref IntPtr        out_template,
            ref int           out_template_cnt
            );
        public static int _IBSM_ConvertTemplate_ISOtoIBSM(
            int               matcher_handle,
            ISO_FMR           in_template,
            ref IntPtr        out_template,
            ref int           out_template_cnt
            )
        {
            int nRc = IBSM_STATUS_OK;
            try
            {
                nRc = IBSM_ConvertTemplate_ISOtoIBSM(matcher_handle, in_template, ref out_template, ref out_template_cnt);
            }
            catch (Exception except)
            {
                Trace.WriteLine("IBSM_ConvertTemplate_ISOtoIBSM : " + except.Message);
            }

            return nRc;
        }

        /*
        ****************************************************************************************************
        * IBSM_ConvertImage_ISOtoIBSM()
        * 
        * DESCRIPTION:
        *     Convert image data from ISO/IEC 19794-4 FIR (Fingerprint Image Record) format to IB's template
        *     format.
        *
        * ARGUMENTS:
        *     matcher_handle  Matcher handle.
        *     in_image        Image to convert.
        *     out_image       (output) Pointer to memory that will receive a pointer to an array of
        *                     images.
        *     out_image_cnt   (output) Pointer to integer that will receive the number of images in
        *                     the array of images.
        *
        * RETURNS:
        *     IBSM_STATUS_OK, if successful.
        *     Error code < 0, otherwise.  See error codes in 'IBScanMatcherApi_err'.
        ****************************************************************************************************
        */
        [DllImport("IBScanMatcher.DLL")]
        private static extern int IBSM_ConvertImage_ISOtoIBSM(
            int                matcher_handle,
            ISO_FIR            in_image,
            ref IntPtr         out_image,
            ref int            out_image_cnt
            );
        public static int _IBSM_ConvertImage_ISOtoIBSM(
            int                matcher_handle,
            ISO_FIR            in_image,
            ref IntPtr         out_image,
            ref int            out_image_cnt
            )
        {
            int nRc = IBSM_STATUS_OK;
            try
            {
                nRc = IBSM_ConvertImage_ISOtoIBSM(matcher_handle, in_image, ref out_image, ref out_image_cnt);
            }
            catch (Exception except)
            {
                Trace.WriteLine("IBSM_ConvertImage_ISOtoIBSM : " + except.Message);
            }

            return nRc;
        }

        /*
        ****************************************************************************************************
        * IBSM_ConvertTemplate_IBSMtoISO()
        * 
        * DESCRIPTION:
        *     Convert template from IB's template format to ISO/IEC 19794-2 FMR (Fingerprint Minutiae Record)
        *     format.
        *
        * ARGUMENTS:
        *     matcher_handle  Matcher handle.
        *     in_template     Template to convert.
        *     out_template    (output) Pointer to memory that will receive template information.
        *
        * RETURNS:
        *     IBSM_STATUS_OK, if successful.
        *     Error code < 0, otherwise.  See error codes in 'IBScanMatcherApi_err'.
        ****************************************************************************************************
        */
        [DllImport("IBScanMatcher.DLL")]
        private static extern int IBSM_ConvertTemplate_IBSMtoISO(
            int           matcher_handle,
            IBSM_Template in_template,
            ref ISO_FMR   out_templatee
            );
        public static int _IBSM_ConvertTemplate_IBSMtoISO(
            int           matcher_handle,
            IBSM_Template in_template,
            ref ISO_FMR   out_templatee
            )
        {
            int nRc = IBSM_STATUS_OK;
            try
            {
                nRc = IBSM_ConvertTemplate_IBSMtoISO(matcher_handle, in_template, ref out_templatee);
            }
            catch (Exception except)
            {
                Trace.WriteLine("IBSM_ConvertTemplate_IBSMtoISO : " + except.Message);
            }

            return nRc;
        }

        /*
        ****************************************************************************************************
        * IBSM_ConvertImage_IBSMtoISO()
        * 
        * DESCRIPTION:
        *     Convert image data from IB's image format to ISO/IEC 19794-4 FIR (Fingerprint Image Record)
        *     format.
        *
        * ARGUMENTS:
        *     matcher_handle  Matcher handle.
        *     in_image        Image to convert.
        *     out_image       (output) Pointer to memory that will receive image information.
        *
        * RETURNS:
        *     IBSM_STATUS_OK, if successful.
        *     Error code < 0, otherwise.  See error codes in 'IBScanMatcherApi_err'.
        ****************************************************************************************************
        */
        [DllImport("IBScanMatcher.DLL")]
        private static extern int IBSM_ConvertImage_IBSMtoISO(
            int            matcher_handle,
            DLL.IBSM_ImageData in_image,
            ref ISO_FIR    out_image
         );
        public static int _IBSM_ConvertImage_IBSMtoISO(
            int            matcher_handle,
            DLL.IBSM_ImageData in_image,
            ref ISO_FIR    out_image
            )
        {
            int nRc = IBSM_STATUS_OK;
            try
            {
                nRc = IBSM_ConvertImage_IBSMtoISO(matcher_handle, in_image, ref out_image);
            }
            catch (Exception except)
            {
                Trace.WriteLine("IBSM_ConvertImage_IBSMtoISO : " + except.Message);
            }

            return nRc;
        }

        /*
        ****************************************************************************************************
        * IBSM_ConvertTemplate_IBISDKtoIBSM()
        * 
        * DESCRIPTION:
        *     Convert template from IBI SDK format to IB's template format (Curve only).
        *
        * ARGUMENTS:
        *     matcher_handle  Matcher handle.
        *     in_template     Template to convert.
        *     out_template    (output) Pointer to memory that will receive template information.
        *
        * RETURNS:
        *     IBSM_STATUS_OK, if successful.
        *     Error code < 0, otherwise.  See error codes in 'IBScanMatcherApi_err'.
        ****************************************************************************************************
        */
        [DllImport("IBScanMatcher.DLL")]
        private static extern int IBSM_ConvertTemplate_IBISDKtoIBSM(
            int               matcher_handle,
            IBISDK_Template   in_template,
            ref IBSM_Template out_template
            );
        public static int _IBSM_ConvertTemplate_IBISDKtoIBSM(
            int               matcher_handle,
            IBISDK_Template   in_template,
            ref IBSM_Template out_template
            )
        {
            int nRc = IBSM_STATUS_OK;
            try
            {
                nRc = IBSM_ConvertTemplate_IBISDKtoIBSM(matcher_handle, in_template, ref out_template);
            }
            catch (Exception except)
            {
                Trace.WriteLine("IBSM_ConvertTemplate_IBISDKtoIBSM : " + except.Message);
            }

            return nRc;
        }

        /*
        ****************************************************************************************************
        * IBSM_ConvertTemplate_IBSMtoIBISDK()
        * 
        * DESCRIPTION:
        *     Convert template from IB's template format to IBI SDK format (Curve only).
        *
        * ARGUMENTS:
        *     matcher_handle  Matcher handle.
        *     in_template     Template to convert.
        *     out_template    (output) Pointer to memory that will receive template information.
        *
        * RETURNS:
        *     IBSM_STATUS_OK, if successful.
        *     Error code < 0, otherwise.  See error codes in 'IBScanMatcherApi_err'.
        ****************************************************************************************************
        */
        [DllImport("IBScanMatcher.DLL")]
        private static extern int IBSM_ConvertTemplate_IBSMtoIBISDK(
            int                 matcher_handle,
            IBSM_Template       in_template,
            ref IBISDK_Template out_template
            );
        public static int _IBSM_ConvertTemplate_IBSMtoIBISDK(
            int                 matcher_handle,
            IBSM_Template       in_template,
            ref IBISDK_Template out_template
            )
        {
            int nRc = IBSM_STATUS_OK;
            try
            {
                nRc = IBSM_ConvertTemplate_IBSMtoIBISDK(matcher_handle, in_template, ref out_template);
            }
            catch (Exception except)
            {
                Trace.WriteLine("IBSM_ConvertTemplate_IBSMtoIBISDK : " + except.Message);
            }

            return nRc;
        }

        /*
        ****************************************************************************************************
        * IBSM_OpenImageData()
        * 
        * DESCRIPTION:
        *     Load image data from file in IB's image format.
        *
        * ARGUMENTS:
        *     matcher_handle  Matcher handle.
        *     filePath        Path of file from which to load image data.
        *     out_image       (output) Pointer to memory that will receive image data.
        *
        * RETURNS:
        *     IBSM_STATUS_OK, if successful.
        *     Error code < 0, otherwise.  See error codes in 'IBScanMatcherApi_err'.
        ****************************************************************************************************
        */
        [DllImport("IBScanMatcher.DLL")]
        private static extern int IBSM_OpenImageData(
            int                matcher_handle,
            string             filePath,
            ref DLL.IBSM_ImageData out_image
            );
        public static int _IBSM_OpenImageData(
            int                matcher_handle,
            string             filePath,
            ref DLL.IBSM_ImageData out_image
            )
        {
            int nRc = IBSM_STATUS_OK;
            try
            {
                nRc = IBSM_OpenImageData(matcher_handle, filePath, ref out_image);
            }
            catch (Exception except)
            {
                Trace.WriteLine("IBSM_OpenImageData : " + except.Message);
            }

            return nRc;
        }

        /*
        ****************************************************************************************************
        * IBSM_SaveImageData()
        * 
        * DESCRIPTION:
        *     Save image data to file in IB's image format.
        *
        * ARGUMENTS:
        *     matcher_handle  Matcher handle.
        *     filePath        Path of file to which to save image data.
        *     inImage         Image data to save.
        *
        * RETURNS:
        *     IBSM_STATUS_OK, if successful.
        *     Error code < 0, otherwise.  See error codes in 'IBScanMatcherApi_err'.
        ****************************************************************************************************
        */
        [DllImport("IBScanMatcher.DLL")]
        private static extern int IBSM_SaveImageData(
            int            matcher_handle,
            string         filePath,
            DLL.IBSM_ImageData inImage
            );
        public static int _IBSM_SaveImageData(
            int            matcher_handle,
            string         filePath,
            DLL.IBSM_ImageData inImage
            )
        {
            int nRc = IBSM_STATUS_OK;
            try
            {
                nRc = IBSM_SaveImageData(matcher_handle, filePath, inImage);
            }
            catch (Exception except)
            {
                Trace.WriteLine("IBSM_SaveImageData : " + except.Message);
            }

            return nRc;
        }

        /*
        ****************************************************************************************************
        * IBSM_OpenTemplate()
        * 
        * DESCRIPTION:
        *     Load template from file in IB's template format.
        *
        * ARGUMENTS:
        *     matcher_handle  Matcher handle.
        *     filePath        Path of file from which to load template.
        *     out_template    (output) Pointer to memory that will receive template information.
        *
        * RETURNS:
        *     IBSM_STATUS_OK, if successful.
        *     Error code < 0, otherwise.  See error codes in 'IBScanMatcherApi_err'.
        ****************************************************************************************************
        */
        [DllImport("IBScanMatcher.DLL")]
        private static extern int IBSM_OpenTemplate(
            int               matcher_handle,
            string            filePath,
            ref IBSM_Template out_template
            );
        public static int _IBSM_OpenTemplate(
            int               matcher_handle,
            string            filePath,
            ref IBSM_Template out_template
            )
        {
            int nRc = IBSM_STATUS_OK;
            try
            {
                nRc = IBSM_OpenTemplate(matcher_handle, filePath, ref out_template);
            }
            catch (Exception except)
            {
                Trace.WriteLine("IBSM_OpenTemplate : " + except.Message);
            }

            return nRc;
        }

        /*
        ****************************************************************************************************
        * IBSM_SaveTemplate()
        * 
        * DESCRIPTION:
        *     Save template to file in IB's template format.
        *
        * ARGUMENTS:
        *     matcher_handle  Matcher handle.
        *     filePath        Path of file to which to save image template.
        *     inTemplate      Image template to save.
        *
        * RETURNS:
        *     IBSM_STATUS_OK, if successful.
        *     Error code < 0, otherwise.  See error codes in 'IBScanMatcherApi_err'.
        ****************************************************************************************************
        */
        [DllImport("IBScanMatcher.DLL")]
        private static extern int IBSM_SaveTemplate(
            int           matcher_handle,
            string        filePath,
            IBSM_Template inTemplate
            );
        public static int _IBSM_SaveTemplate(
            int           matcher_handle,
            string        filePath,
            IBSM_Template inTemplate
            )
        {
            int nRc = IBSM_STATUS_OK;
            try
            {
                nRc = IBSM_SaveTemplate(matcher_handle, filePath, inTemplate);
            }
            catch (Exception except)
            {
                Trace.WriteLine("IBSM_SaveTemplate : " + except.Message);
            }

            return nRc;
        }

        /*
        ****************************************************************************************************
        * IBSM_OpenFIR()
        * 
        * DESCRIPTION:
        *     Load image data from file in ISO/IEC 19794-4 FIR (Fingerprint Image Record) format.
        *
        * ARGUMENTS:
        *     matcher_handle  Matcher handle.
        *     filePath        Path of file from which to load FIR.
        *     iso_fir         (output) Pointer to memory that will receive FIR data.
        *
        * RETURNS:
        *     IBSM_STATUS_OK, if successful.
        *     Error code < 0, otherwise.  See error codes in 'IBScanMatcherApi_err'.
        ****************************************************************************************************
        */
        [DllImport("IBScanMatcher.DLL")]
        private static extern int IBSM_OpenFIR(
            int         matcher_handle,
            string      filePath,
            ref ISO_FIR iso_fir
            );
        public static int _IBSM_OpenFIR(
            int         matcher_handle,
            string      filePath,
            ref ISO_FIR iso_fir
            )
        {
            int nRc = IBSM_STATUS_OK;
            try
            {
                nRc = IBSM_OpenFIR(matcher_handle, filePath, ref iso_fir);
            }
            catch (Exception except)
            {
                Trace.WriteLine("IBSM_OpenFIR : " + except.Message);
            }

            return nRc;
        }

        /*
        ****************************************************************************************************
        * IBSM_SaveFIR()
        * 
        * DESCRIPTION:
        *     Save image data to file in ISO/IEC 19794-4 FIR (Fingerprint Image Record) format.
        *
        * ARGUMENTS:
        *     matcher_handle  Matcher handle.
        *     filePath        Path of file to which to save FIR.
        *     iso_fir         FIR data to save.
        *
        * RETURNS:
        *     IBSM_STATUS_OK, if successful.
        *     Error code < 0, otherwise.  See error codes in 'IBScanMatcherApi_err'.
        ****************************************************************************************************
        */
        [DllImport("IBScanMatcher.DLL")]
        private static extern int IBSM_SaveFIR(
            int     matcher_handle,
            string  filePath,
            ISO_FIR iso_fir
            );
        public static int _IBSM_SaveFIR(
            int     matcher_handle,
            string  filePath,
            ISO_FIR iso_fir
            )
        {
            int nRc = IBSM_STATUS_OK;
            try
            {
                nRc = IBSM_SaveFIR(matcher_handle, filePath, iso_fir);
            }
            catch (Exception except)
            {
                Trace.WriteLine("IBSM_SaveFIR : " + except.Message);
            }

            return nRc;
        }

        /*
        ****************************************************************************************************
        * IBSM_OpenFMR()
        * 
        * DESCRIPTION:
        *     Load template from file in ISO/IEC 19794-2 FMR (Fingerprint Minutiae Record) format.
        *
        * ARGUMENTS:
        *     matcher_handle  Matcher handle.
        *     filePath        Path of file from which to load FMR.
        *     iso_fmr         (output) Pointer to memory that will receive FMR information.
        *
        * RETURNS:
        *     IBSM_STATUS_OK, if successful.
        *     Error code < 0, otherwise.  See error codes in 'IBScanMatcherApi_err'.
        ****************************************************************************************************
        */
        [DllImport("IBScanMatcher.DLL")]
        private static extern int IBSM_OpenFMR(
            int         matcher_handle,
            string      filePath,
            ref ISO_FMR iso_fmr
            );
        public static int _IBSM_OpenFMR(
            int         matcher_handle,
            string      filePath,
            ref ISO_FMR iso_fmr
            )
        {
            int nRc = IBSM_STATUS_OK;
            try
            {
                nRc = IBSM_OpenFMR(matcher_handle, filePath, ref iso_fmr);
            }
            catch (Exception except)
            {
                Trace.WriteLine("IBSM_OpenFMR : " + except.Message);
            }

            return nRc;
        }

        /*
        ****************************************************************************************************
        * IBSM_SaveFMR()
        * 
        * DESCRIPTION:
        *     Save template to file in ISO/IEC 19794-2 FMR (Fingerprint Minutiae Record) format.
        *
        * ARGUMENTS:
        *     matcher_handle  Matcher handle.
        *     filePath        Path of file to which to save FMR.
        *     iso_fmr         FMR data to save.
        *
        * RETURNS:
        *     IBSM_STATUS_OK, if successful.
        *     Error code < 0, otherwise.  See error codes in 'IBScanMatcherApi_err'.
        ****************************************************************************************************
        */
        [DllImport("IBScanMatcher.DLL")]
        private static extern int IBSM_SaveFMR(
            int     matcher_handle,
            string  filePath,
            ISO_FMR iso_fmr
            );
        public static int _IBSM_SaveFMR(
            int     matcher_handle,
            string  filePath,
            ISO_FMR iso_fmr
            )
        {
            int nRc = IBSM_STATUS_OK;
            try
            {
                nRc = IBSM_SaveFMR(matcher_handle, filePath, iso_fmr);
            }
            catch (Exception except)
            {
                Trace.WriteLine("IBSM_SaveFMR : " + except.Message);
            }

            return nRc;
        }

        /*
         * Release the allocated memory block on the internal heap of library.
         * This is obtained by IBSM_OpenFIR(), IBSM_OpenFMR() and other API functions.
         *
         * ARGUMENTS:
         *     matcher_handle  Matcher handle.
         *     memblock        Previously allocated memory block to be freed.
         *     memblock_type   Type of memory block.
         *
         * RETURNS:
         *     IBSM_STATUS_OK if successful.
         *     Otherwise, error code from IBScanMatcherApi_err.h.
         */
        [DllImport("IBScanMatcher.DLL")]
        private static extern int IBSM_FreeMemory(
            int               matcher_handle,
            IntPtr            memblock,
            IBSM_MemBlockType memblock_type
            );
        public static int _IBSM_FreeMemory(
            int               matcher_handle,
            IntPtr            memblock,
            IBSM_MemBlockType memblock_type
            )
        {
            int nRc = IBSM_STATUS_OK;
            try
            {
                nRc = IBSM_FreeMemory(matcher_handle, memblock, memblock_type);
            }
            catch (Exception except)
            {
                Trace.WriteLine("IBSM_FreeMemory : " + except.Message);
            }

            return nRc;
        }

        /*
         * Load raster image from file.
         *
         * ARGUMENTS:
         *     matcher_handle  Matcher handle.
         *     filePath        Path of file from which to load template.
         *     out_image       (output) Pointer to memory that will receive image data.
         *                     This pointer is to be deallocated by IBSM_FreeMemory() after using it.
         *
         * RETURNS:
         *     IBSM_STATUS_OK if successful.
         *     Otherwise, error code from IBScanMatcherApi_err.h.
         */
        [DllImport("IBScanMatcher.DLL")]
        private static extern int IBSM_OpenRasterImage(
            int matcher_handle,
            string filePath,
            ref DLL.IBSM_ImageData out_image
            );
        public static int _IBSM_OpenRasterImage(
            int matcher_handle,
            string filePath,
            ref DLL.IBSM_ImageData out_image
            )
        {
            int nRc = IBSM_STATUS_OK;
            try
            {
                nRc = IBSM_OpenRasterImage(matcher_handle, filePath, ref out_image);
            }
            catch (Exception except)
            {
                Trace.WriteLine("IBSM_OpenRasterImage : " + except.Message);
            }

            return nRc;
        }
    }
}
