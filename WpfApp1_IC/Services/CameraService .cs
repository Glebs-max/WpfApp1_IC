using MvCodeReaderSDKNet;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media;
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

        public void InitCamera()
        {
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
        }

        public (DataMatrixResult dm, BitmapSource frame) TriggerAndRead()
        {
            if (_reader == null)
                throw new InvalidOperationException("Camera not initialized");

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

                //System.Diagnostics.Debug.WriteLine("PixelFormat: " + info.enPixelType);  //Вывод формата изображения далее надо будет удалить 

                BitmapSource frame = GetLastFrameBitmap(pData, info);

                // Если код не найден вообще
                if (!info.bIsGetCode || info.chResult == null)
                    return (null, frame);

                //Если найден но пустой (служебные байты)
                if (info.chResult.Length <= 8)
                {
                    Console.WriteLine("Код найден, но пустой или повреждён");
                    return (null, frame);
                }

                var dm = ExtractDM(info.chResult);

                // Если строка пустая — тоже считаем, что кода нет
                if (dm == null || string.IsNullOrWhiteSpace(dm.Normalized)) 
                    return (null, frame); 

                return (dm, frame);
            }
            finally
            {
                Marshal.FreeHGlobal(pInfo);
            }
        }

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

        // получение Raw кадра 
        public BitmapSource GetLastFrameBitmap(IntPtr pData, MvCodeReader.MV_CODEREADER_IMAGE_OUT_INFO info)
        {
            // JPEG поток
            int jpegSize = (int)info.nFrameLen;
            byte[] jpegData = new byte[jpegSize];

            // копируем JPEG из unmanaged памяти
            Marshal.Copy(pData, jpegData, 0, jpegSize);

            // декодируем JPEG
            using (var ms = new MemoryStream(jpegData))
            {
                var decoder = new JpegBitmapDecoder(
                    ms,
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.OnLoad);

                BitmapSource bmp = decoder.Frames[0];
                bmp.Freeze(); // обязательно
                return bmp;
            }
        }


        public void Dispose()
        {
            if (_reader != null)
            {
                _reader.MV_CODEREADER_StopGrabbing_NET();
                _reader.MV_CODEREADER_CloseDevice_NET();
                _reader.MV_CODEREADER_DestroyHandle_NET();
                _reader = null;
            }
        }
    }
}
