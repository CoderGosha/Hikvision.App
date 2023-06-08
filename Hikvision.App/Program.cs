using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hikvision.Api;
using Hikvision.Api.ResponseModels;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace HikvisionApp
{
    class Program
    {
        public static int TimeOut = 10;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!, Press to start");
            Console.ReadLine();
            try
            {
                var builder = new ConfigurationBuilder()
                    .AddJsonFile($"appsettings.json", true, true);
                var config = builder.Build();
                var configHikvision = new HikvisionConfig()
                {
                    Url = config["HikvisionConfig:Url"],
                    User = config["HikvisionConfig:User"],
                    Password = config["HikvisionConfig:Password"]
                };
                TimeOut = Int32.Parse(config["ManualCupWaitTimeOutSec"]);
                
                await CameraMain(configHikvision);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static async Task CameraMain(HikvisionConfig configHikvision)
        {
            
            Console.WriteLine($"Connecting camera with: {JsonConvert.SerializeObject(configHikvision)}");
            IHikvisionApi hikvison = new HikvisionSimpleApi(configHikvision);
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
                $"Device: {deviceInfo.Data.DeviceName}, ID: {deviceInfo.Data.DeviceID}, " +
                $"FWVersion: {deviceInfo.Data.FirmwareVersion}, FWData: {deviceInfo.Data.FirmwareReleasedDate}");

            
            // Тест получения кадра 
            var picture = await hikvison.PictureAsync();
            if (picture.IsSuccess)
                SavePhotoFileWithBinaryWriter(picture.Data.Image, "streaming_picture", "test");
            
            await ManualCap(hikvison);

            string exitKey = "x";
            string callSign;
            // 
            var messageRead = "Press key to manual cup, W to manual cup with timeout or X to exit:";
            Console.WriteLine(messageRead);
            while ((callSign = Console.ReadLine()?.ToLower()) != exitKey)
            {
                if (callSign == "w")
                    await ManualWaitCap(hikvison, TimeOut);
                else
                    await ManualCap(hikvison);

                Console.WriteLine("");
                Console.WriteLine(messageRead); //Prompt
            }
        }

        private static async Task ManualWaitCap(IHikvisionApi hikvison, int timeout)
        {
            Console.WriteLine($"{DateTime.Now} - Starting manual wait: {timeout}sec");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            BaseResponse<ManualCupResponse> manualCup = null;
            var timeStart = DateTime.Now + TimeSpan.FromSeconds(timeout);
            while ((timeStart > DateTime.Now) || (manualCup == null))
            {
                manualCup = await hikvison.ManualCupAsync();
                if (manualCup.Data.IsRecognize)
                {
                    var filePath =
                        SavePhotoFileWithBinaryWriter(manualCup.Data.Image, "manual_cup", manualCup.Data.Number);
                    sw.Stop();
                    Console.WriteLine(
                        $"{DateTime.Now} - Manual cup-{sw.ElapsedMilliseconds}ms: Recognize: {manualCup.Data?.IsRecognize} Number: {manualCup.Data?.Number} Image: {filePath}");
                }
            }

            Console.WriteLine($"{DateTime.Now} - Timeout manual wait");
            var filePathUnknown =
                SavePhotoFileWithBinaryWriter(manualCup.Data.Image, "manual_cup", manualCup.Data.Number);
            sw.Stop();
            Console.WriteLine(
                $"{DateTime.Now} - Manual cup-{sw.ElapsedMilliseconds}ms: Recognize: {manualCup.Data?.IsRecognize} Number: {manualCup.Data?.Number} Image: {filePathUnknown}");
        }
        
        public static string SavePhotoFileWithBinaryWriter(byte[] data, string prefixPath, string number)
        {
            var dir = Directory.GetCurrentDirectory();
            var basePath = Path.Join(dir, prefixPath);
            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            var fileName = $"{number}_" + DateTime.Now.ToString("yyyy_MM_dd_H-mm-ss") + ".jpg";
            var filePath = Path.Join(basePath, fileName);

            using var writer = new BinaryWriter(File.OpenWrite(filePath));
            writer.Write(data);
            return filePath;
        }

        public static async Task ManualCap(IHikvisionApi hikvison)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var manualCup = await hikvison.ManualCupAsync();
            if (!manualCup.IsSuccess)
            {
                Console.WriteLine($"Error manual cup: {manualCup.Code} message: {manualCup.Message}");
                return;
            }

            // Метод сохранения фото
            var filePath = SavePhotoFileWithBinaryWriter(manualCup.Data.Image, "manual_cup", manualCup.Data.Number);
            sw.Stop();
            Console.WriteLine(
                $"Manual cup-{sw.ElapsedMilliseconds}ms: Recognize: {manualCup.Data.IsRecognize} Number: {manualCup.Data.Number} Image: {filePath}");
        }
    }
}