using MvCodeReaderSDKNet;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WpfApp1_IC.Services
{
    public static class CameraDiscovery
    {
        public static List<string> FindCameras()
        {
            var result = new List<string>();

            var list = new MvCodeReader.MV_CODEREADER_DEVICE_INFO_LIST
            {
                pDeviceInfo = new IntPtr[MvCodeReader.MV_CODEREADER_MAX_DEVICE_NUM]
            };

            int ret = MvCodeReader.MV_CODEREADER_EnumDevices_NET(
                ref list,
                MvCodeReader.MV_CODEREADER_GIGE_DEVICE);

            if (ret != MvCodeReader.MV_CODEREADER_OK || list.nDeviceNum == 0)
                return result;

            for (int i = 0; i < list.nDeviceNum; i++)
            {
                // Просто Camera #N
                result.Add($"Camera {i + 1}");
            }

            return result;
        }
    }
}
