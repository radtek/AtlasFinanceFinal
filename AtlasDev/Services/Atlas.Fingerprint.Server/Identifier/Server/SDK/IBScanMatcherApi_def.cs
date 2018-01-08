/*
****************************************************************************************************
* IBScanMatcherApi_defs
*
* DESCRIPTION:
*     API structures and constants for IBScanMatcher.
*     http://www.integratedbiometrics.com
*
* NOTES:
*     Copyright (c) Integrated Biometrics, 2013
*
* HISTORY:
*     2014/08/19  0.10.1b  Reformatted.
****************************************************************************************************
*/


using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using IBscanUltimate;

namespace IBscanMatcher
{
    public partial class MDLL
    {
        /*
        ****************************************************************************************************
        * GLOBAL DEFINES
        ****************************************************************************************************
        */

        /* Required length of buffer for return string parameters. */
        public const int IBSM_MAX_STR_LEN = 128;

        /* Maximum size of minutiae. */
        public const int IBSM_MAX_MINUTIAE_SIZE = (255+2);

        /*
        ****************************************************************************************************
        * GLOBAL TYPES
        ****************************************************************************************************
        */

        /*
        ****************************************************************************************************
        * IBSM_SDKVersion
        *
        * DESCRIPTION:
        *     Container to hold SDK version.
        ****************************************************************************************************
        */
        public struct IBSM_SDKVersion
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = IBSM_MAX_STR_LEN)]
            public string Product;                    /* Product version string */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = IBSM_MAX_STR_LEN)]
            public string File;                       /* File version string */
        }

        /*
        ****************************************************************************************************
        * IBSM_TemplateVersion
        *
        * DESCRIPTION:
        *     Enumeration of template version.
        ****************************************************************************************************
        */
        public enum IBSM_TemplateVersion
        {
            /* TEMPLATE_OLD_VERSION. */
            IBSM_TEMPLATE_VERSION_IBISDK_0=0x00,

            /* TEMPLATE_MIX_VERSION. */
            IBSM_TEMPLATE_VERSION_IBISDK_1,

            /* TEMPLATE_FAST_VERSION. */
            IBSM_TEMPLATE_VERSION_IBISDK_2,

            /* Secuest 1nd Algorithm. */
            IBSM_TEMPLATE_VERSION_IBISDK_3,

            /* IBSM_NEW_TEMPLATE. */
            IBSM_TEMPLATE_VERSION_NEW_0=0x10
        }

        /*
        ****************************************************************************************************
        * IBSM_ScaleUnit
        *
        * DESCRIPTION:
        *     Enumeration of scale unit.
        ****************************************************************************************************
        */
        public enum IBSM_ScaleUnit
        {
            IBSM_SCALE_UNIT_INCH=0x01,

            IBSM_SCALE_UNIT_CENTIMETER=0x02
        }

        /*
        ****************************************************************************************************
        * IBSM_ProcessedImageType
        *
        * DESCRIPTION:
        *     Enumeration of processed image type.
        ****************************************************************************************************
        */
        public enum IBSM_ProcessedImageType
        {
            ENUM_IBSM_PROCESSED_IMAGE_TYPE_GABOR=0x00,

            ENUM_IBSM_PROCESSED_IMAGE_TYPE_BINARY,
            
            ENUM_IBSM_PROCESSED_IMAGE_TYPE_THIN
        }

        /*
        ****************************************************************************************************
        * IBSM_MemBlockType
        *
        * DESCRIPTION:
        *     Enumeration of type of allocated memory block.
        ****************************************************************************************************
        */
        public enum IBSM_MemBlockType
        {
            ENUM_IBSM_MEMBLOCK_TEMPLATE,

            ENUM_IBSM_MEMBLOCK_IMAGE,

            ENUM_IBSM_MEMBLOCK_ISO_TEMPLATE,

            ENUM_IBSM_MEMBLOCK_ISO_IMAGE
        }

        /*
        ****************************************************************************************************
        * IBSM_Template
        *
        * DESCRIPTION:
        *     Container to hold template information.
        ****************************************************************************************************
        */
        public unsafe struct IBSM_Template
        {
            public IBSM_TemplateVersion     version;
            public DLL.IBSM_FingerPosition  FingerPosition;
            public DLL.IBSM_ImpressionType ImpressionType;
            public DLL.IBSM_CaptureDeviceVendorID CaptureDeviceTechID;
            public ushort CaptureDeviceVendorID;
            public ushort CaptureDeviceTypeID;
            public ushort ImageSamplingX;
            public ushort ImageSamplingY;
            public ushort ImageSizeX;
            public ushort ImageSizeY;
            public fixed uint Minutiae[IBSM_MAX_MINUTIAE_SIZE];
            public uint Reserved;
        }

        /*
        ****************************************************************************************************
        * IBISDK_Template
        *
        * DESCRIPTION:
        *     Container to hold IBISDK template information.
        ****************************************************************************************************
        */
        public unsafe struct IBISDK_Template
        {
            public fixed uint Minutiae[101];
        }

        public struct ISO_FIR
        {
	        public IntPtr data;
	        public uint datasize;
        }

        public struct ISO_FMR
        {
	        public IntPtr data;
	        public uint datasize;
        }
    }
}
