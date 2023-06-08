using System.Net;
using System.Xml;
using Hikvision.Api.ResponseModels;
using Newtonsoft.Json;

namespace Hikvision.Api
{

    public class HikvisionClient
    {
        /// <summary>
        /// Базовый url камеры
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Логин (обычно admin НЕ работает)
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Пароль в открытом виде
        /// </summary>
        public string Password { get; set; }

        private readonly string UrlGetDeviceInfo = "/ISAPI/System/deviceInfo";
        private readonly string UrlManualCap = "/ISAPI/ITC/manualCap";
        private readonly string UrlPicture = "/ISAPI/Streaming/channels/1/picture";
        private readonly CredentialCache CredCache;

        public HikvisionClient(HikvisionConfig config)
        {
            Url = config.Url;
            User = config.User;
            Password = config.Password;

            if (string.IsNullOrWhiteSpace(User) || string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(Url))
                throw new ArgumentException(
                    $"One or more parameter is null. User: {User}, Password: {Password}, Url: {Url}");

            // https://stackoverflow.com/questions/59939357/net-core-httpclient-digest-authentication
            CredCache = new CredentialCache {{new Uri(Url), "Digest", new NetworkCredential(User, Password)}};
        }

        public async Task<BaseResponse<DeviceInfoResponse>> DeviceInfoAsync()
        {
            using var httpClient = new HttpClient(new HttpClientHandler {Credentials = CredCache});
            var path = new Uri(new Uri(Url), UrlGetDeviceInfo);
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, path);
            using HttpResponseMessage response = await httpClient.SendAsync(request);

            if (response.StatusCode != HttpStatusCode.OK)
                return new BaseResponse<DeviceInfoResponse>()
                {
                    Code = response.StatusCode,
                    Message = await response.Content.ReadAsStringAsync()
                };

            var contents = await response.Content.ReadAsStringAsync();
            var deviceInfo = DeviceInfoResponse.FromXml(contents);

            return new BaseResponse<DeviceInfoResponse>()
            {
                Code = response.StatusCode,
                Data = deviceInfo
            };
        }

        public async Task<BaseResponse<ManualCupResponse>> ManualCupAsync()
        {
            using var httpClient = new HttpClient(new HttpClientHandler {Credentials = CredCache});
            var path = new Uri(new Uri(Url), UrlManualCap);
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, path);
            using HttpResponseMessage response = await httpClient.SendAsync(request);

            if (response.StatusCode != HttpStatusCode.OK)
                return new BaseResponse<ManualCupResponse>()
                {
                    Code = response.StatusCode,
                    Message = await response.Content.ReadAsStringAsync()
                };

            var bytes = await response.Content.ReadAsByteArrayAsync();
            var manualCap = ManualCupResponse.FromBinary(bytes);

            return new BaseResponse<ManualCupResponse>()
            {
                Code = response.StatusCode,
                Data = manualCap
            };

        }

        public async Task<BaseResponse<PictureResponse>> PictureAsync()
        {
            using var httpClient = new HttpClient(new HttpClientHandler {Credentials = CredCache});
            var path = new Uri(new Uri(Url), UrlPicture);
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, path);
            using HttpResponseMessage response = await httpClient.SendAsync(request);

            if (response.StatusCode != HttpStatusCode.OK)
                return new BaseResponse<PictureResponse>()
                {
                    Code = response.StatusCode,
                    Message = await response.Content.ReadAsStringAsync()
                };

            var bytes = await response.Content.ReadAsByteArrayAsync();

            return new BaseResponse<PictureResponse>()
            {
                Code = response.StatusCode,
                Data = new PictureResponse()
                {
                    Image = bytes
                }
            };

        }
    }
}