using System;
using System.IO;
using System.Threading.Tasks;
using Hikvision.Api;

namespace HikvisionApp
{
    class Program
    {
        public static void SavePhotoFileWithBinaryWriter(byte[] data, string prefixPath, string number)
        {
            var dir = Directory.GetCurrentDirectory();
            var basePath = Path.Join(dir, prefixPath);
            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            var fileName = $"{number}_" + DateTime.Now.ToString("yyyy_MM_dd_H-mm-ss") + ".jpg";
            var filePath = Path.Join(basePath, fileName);
            
            using var writer = new BinaryWriter(File.OpenWrite(filePath));
            writer.Write(data);
        }
        
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var config = new HikvisionConfig()
            {
                Url = "http://192.168.0.221",
                User = "user",
                Password = "1q2w3e4r"
            };
            IHikvisionApi hikvison = new HikvisionSimpleApi(config);
            var response = await hikvison.IsAuthenticatedAsync();
            if (!response.IsSuccess)
            {
                Console.WriteLine($"Auth is trouble: {response.Code} with {response.Message}");
                return;
            }

            var deviceInfo = await hikvison.DeviceInfoAsync();
            if (!deviceInfo.IsSuccess)
            {
                Console.WriteLine($"Get device is trouble: {deviceInfo.Code} with {deviceInfo.Message}");
                return;
            }

            Console.WriteLine(
                $"Device: {deviceInfo?.Data?.DeviceName}, ID: {deviceInfo?.Data?.DeviceID}, " +
                $"FWVersion: {deviceInfo?.Data?.FirmwareVersion}, FWData: {deviceInfo?.Data?.FirmwareReleasedDate}");

            var manualCup = await hikvison.ManualCupAsync();
            if (!manualCup.IsSuccess)
            {
                Console.WriteLine($"Error manual cup: {manualCup.Code} message: {manualCup.Message}");
                return;
            }
            
            Console.WriteLine($"Manual cup: Recognize: {manualCup.Data?.IsRecognize} Number: {manualCup.Data?.Number}");
            // Метод сохранения фото
            SavePhotoFileWithBinaryWriter(manualCup.Data.Image, "manual_cup", manualCup.Data.Number);
        }
    }
}