using MvCodeReaderSDKNet;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media.Imaging;

namespace WpfApp1_IC.Services
{
    public class DataMatrixResult
    {
        public string Raw { get; set; }
        public string Normalized { get; set; }
    }

    public class CameraService : IDisposable
    {
        private MvCodeReader _reader;
        private bool _isOpened = false;

        // ================================
        // 1. ОТКРЫТИЕ КАМЕРЫ (вместо InitCamera)
        // ================================
        public void Open()
        {
            if (_isOpened)
                return;

            var list = new MvCodeReader.MV_CODEREADER_DEVICE_INFO_LIST
            {
                pDeviceInfo = new IntPtr[MvCodeReader.MV_CODEREADER_MAX_DEVICE_NUM]
            };

            int ret = MvCodeReader.MV_CODEREADER_EnumDevices_NET(
                ref list,
                MvCodeReader.MV_CODEREADER_GIGE_DEVICE);

            if (ret != MvCodeReader.MV_CODEREADER_OK || list.nDeviceNum == 0)
                throw new Exception("Camera not found");

            var devInfo = Marshal.PtrToStructure<MvCodeReader.MV_CODEREADER_DEVICE_INFO>(
                list.pDeviceInfo[0]);

            _reader = new MvCodeReader();
            _reader.MV_CODEREADER_CreateHandle_NET(ref devInfo);
            _reader.MV_CODEREADER_OpenDevice_NET();

            _reader.MV_CODEREADER_SetEnumValueByString_NET("TriggerMode", "On");
            _reader.MV_CODEREADER_SetEnumValueByString_NET("TriggerSource", "Software");
            _reader.MV_CODEREADER_SetEnumValueByString_NET("CodeType", "DataMatrix");

            _reader.MV_CODEREADER_StartGrabbing_NET();

            _isOpened = true;
        }

        // ================================
        // 2. ЗАКРЫТИЕ КАМЕРЫ
        // ================================
        public void Close()
        {
            if (!_isOpened)
                return;

            try
            {
                _reader.MV_CODEREADER_StopGrabbing_NET();
                _reader.MV_CODEREADER_CloseDevice_NET();
                _reader.MV_CODEREADER_DestroyHandle_NET();
            }
            catch { }

            _reader = null;
            _isOpened = false;
        }

        // ================================
        // 3. ТРИГГЕР И ЧТЕНИЕ КАДРА
        // ================================
        public (DataMatrixResult dm, BitmapSource frame) TriggerAndRead()
        {
            if (!_isOpened || _reader == null)
                throw new InvalidOperationException("Camera not opened");

            _reader.MV_CODEREADER_SetCommandValue_NET("TriggerSoftware");

            IntPtr pData = IntPtr.Zero;
            var info = new MvCodeReader.MV_CODEREADER_IMAGE_OUT_INFO();
            IntPtr pInfo = Marshal.AllocHGlobal(Marshal.SizeOf(info));

            try
            {
                int ret = _reader.MV_CODEREADER_GetOneFrameTimeout_NET(ref pData, pInfo, 1000);
                if (ret != MvCodeReader.MV_CODEREADER_OK)
                    return (null, null);

                info = Marshal.PtrToStructure<MvCodeReader.MV_CODEREADER_IMAGE_OUT_INFO>(pInfo);

                BitmapSource frame = GetLastFrameBitmap(pData, info);

                if (!info.bIsGetCode || info.chResult == null)
                    return (null, frame);

                if (info.chResult.Length <= 8)
                    return (null, frame);

                var dm = ExtractDM(info.chResult);

                if (dm == null || string.IsNullOrWhiteSpace(dm.Normalized))
                    return (null, frame);

                return (dm, frame);
            }
            finally
            {
                Marshal.FreeHGlobal(pInfo);
            }
        }

        // ================================
        // 4. ПАРСИНГ DM
        // ================================
        private DataMatrixResult ExtractDM(byte[] data)
        {
            if (data == null || data.Length < 9)
                return null;

            int start = 8;
            int zero = Array.IndexOf(data, (byte)0, start);
            if (zero < 0)
                return null;

            string raw = Encoding.UTF8.GetString(data, start, zero - start);
            string normalized = raw.Replace(((char)0x1D).ToString(), "");

            return new DataMatrixResult
            {
                Raw = raw,
                Normalized = normalized
            };
        }

        // ================================
        // 5. ПОЛУЧЕНИЕ КАДРА
        // ================================
        public BitmapSource GetLastFrameBitmap(IntPtr pData, MvCodeReader.MV_CODEREADER_IMAGE_OUT_INFO info)
        {
            int jpegSize = (int)info.nFrameLen;
            byte[] jpegData = new byte[jpegSize];

            Marshal.Copy(pData, jpegData, 0, jpegSize);

            using (var ms = new MemoryStream(jpegData))
            {
                var decoder = new JpegBitmapDecoder(
                    ms,
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.OnLoad);

                BitmapSource bmp = decoder.Frames[0];
                bmp.Freeze();
                return bmp;
            }
        }

        // ================================
        // 6. Dispose
        // ================================
        public void Dispose()
        {
            Close();
        }
    }
}
