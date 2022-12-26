using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Hikvision.Api;
using Hikvision.Api.ResponseModels;

namespace HikvisionApp
{
    class Program
    {
        public static int TimeOut = 10;
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
                $"Manual cup-{sw.ElapsedMilliseconds}ms: Recognize: {manualCup.Data?.IsRecognize} Number: {manualCup.Data?.Number} Image: {filePath}");
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

            await ManualCap(hikvison);

            string exitKey = "x";
            string callSign;
            // 
            Console.WriteLine("Press key to manual cup, W to manual cup with timeout or X to exit: ");
            while ((callSign = Console.ReadLine().ToLower()) != exitKey)
            {
                if (callSign == "w")
                {
                    await ManualWaitCap(hikvison, TimeOut);
                }
                else
                    await ManualCap(hikvison);

                Console.WriteLine("");
                Console.WriteLine("Enter a call sign to find in the list. Press X to exit: "); //Prompt
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
    }
}