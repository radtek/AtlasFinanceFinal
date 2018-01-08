using System;
using System.Runtime.InteropServices;

using IBscanMatcher;
using IBscanUltimate;


namespace Atlas.FP.Identifier.SDK.Utils
{
  /// <summary>
  /// Useful IB template utilities to compensate for IB's SDK shortcomings
  /// </summary>
  public static class IBTemplateUtils
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="bitmap"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="image"></param>
    /// <returns>IBSM_ImageData version of raw image. NOTE: Caller to free IBSM_ImageData.ImageData!!</returns>
    public static DLL.IBSM_ImageData RawImageToIBSMImage(byte[] bitmap, int width = IBConsts.FINGERPRINT_IMAGE_WIDTH, int height = IBConsts.FINGERPRINT_IMAGE_HEIGHT)
    {
      var len = width * height;
      var image = new DLL.IBSM_ImageData()
      {
        ImageData = Marshal.AllocHGlobal(len),
        BitDepth = 8,
        CaptureDeviceTechID = DLL.IBSM_CaptureDeviceTechID.IBSM_CAPTURE_DEVICE_ELECTRO_LUMINESCENT,
        CaptureDeviceVendorID = (ushort)DLL.IBSM_CaptureDeviceVendorID.IBSM_CAPTURE_DEVICE_VENDOR_INTEGRATED_BIOMETRICS,
        FingerPosition = DLL.IBSM_FingerPosition.IBSM_FINGER_POSITION_UNKNOWN,
        ImageDataLength = (uint)len,
        ImageFormat = DLL.IBSM_ImageFormat.IBSM_IMG_FORMAT_NO_BIT_PACKING,
        ImageSizeX = (ushort)width,
        ImageSizeY = (ushort)height,
        CaptureDeviceTypeID = (width == 400 && height == 500) ? (ushort)DLL.IBSM_CaptureDeviceTypeID.IBSM_CAPTURE_DEVICE_TYPE_ID_COLUMBO : (ushort)DLL.IBSM_CaptureDeviceTypeID.IBSM_CAPTURE_DEVICE_TYPE_ID_CURVE,
        ImageSamplingX = 500,
        ImageSamplingY = 500,
        ScaleUnit = 1,
        ScanSamplingX = 620,
        ScanSamplingY = 620
      }; 
      Marshal.Copy(bitmap, 0, image.ImageData, len);

      return image;
    }


    /// <summary>
    /// Serialize an IB template to a byte[]- 
    /// </summary>
    /// <param name="template"></param>
    /// <returns></returns>
    internal static unsafe byte[] Serialize(MDLL.IBSM_Template template)
    {
      var result = new byte[MDLL.IBSM_MAX_MINUTIAE_SIZE * 4];
      var p = (byte*)template.Minutiae;
      for (var i = 0; i < MDLL.IBSM_MAX_MINUTIAE_SIZE * 4; ++i)
      {
        result[i] = *p;
        p++;
      }

      return result;
    }


    /// <summary>
    /// Deserialize a byte[] to an IB template
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    internal static unsafe MDLL.IBSM_Template Deserialize(byte[] val, 
      int width = IBConsts.FINGERPRINT_IMAGE_WIDTH, int height = IBConsts.FINGERPRINT_IMAGE_HEIGHT)
    {
      var result = new MDLL.IBSM_Template()
        {
          CaptureDeviceTechID = DLL.IBSM_CaptureDeviceVendorID.IBSM_CAPTURE_DEVICE_VENDOR_INTEGRATED_BIOMETRICS,
          CaptureDeviceTypeID = (ushort)DLL.IBSM_CaptureDeviceTypeID.IBSM_CAPTURE_DEVICE_TYPE_ID_CURVE,
          CaptureDeviceVendorID = (ushort)DLL.IBSM_CaptureDeviceVendorID.IBSM_CAPTURE_DEVICE_VENDOR_INTEGRATED_BIOMETRICS,
          FingerPosition = DLL.IBSM_FingerPosition.IBSM_FINGER_POSITION_UNKNOWN,
          ImageSamplingX = 500,
          ImageSamplingY = 500,
          ImageSizeX = (ushort)width,
          ImageSizeY = (ushort)height,
          ImpressionType = DLL.IBSM_ImpressionType.IBSM_IMPRESSION_TYPE_UNKNOWN,
          version = MDLL.IBSM_TemplateVersion.IBSM_TEMPLATE_VERSION_NEW_0
        };

      var arrPos = 0;
      for (var i = 0; i < MDLL.IBSM_MAX_MINUTIAE_SIZE; i++)
      {
        result.Minutiae[i] = (uint)val[arrPos++] + ((uint)val[arrPos++] * 256U) +
          ((uint)val[arrPos++] * (uint)(256U * 256U)) + ((uint)val[arrPos++] * (256U * 256U * 256U)); // can't bit shift uint...
      }
      return result;
    }

  }
}
