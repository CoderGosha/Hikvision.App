using System;
using System.Threading.Tasks;
using Hikvision.Api;

namespace HikvisionApp
{
    class Program
    {
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
        }
    }
}