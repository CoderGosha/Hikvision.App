using System.Net;
using System.Xml;
using System.Xml.Serialization;
using ExtendedXmlSerializer;
using ExtendedXmlSerializer.Configuration;
using Hikvision.Api.ResponseModels;

namespace Hikvision.Api;

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
    private readonly CredentialCache CredCache;

    public HikvisionClient(HikvisionConfig config)
    {
        Url = config.Url;
        User = config.User;
        Password = config.Password;
        // https://stackoverflow.com/questions/59939357/net-core-httpclient-digest-authentication
        CredCache = new CredentialCache {{new Uri(Url), "Digest", new NetworkCredential(User, Password)}};
    }

    public async Task<BaseResponse<DeviceInfoResponse>> DeviceInfoAsync()
    {
        using var httpClient = new HttpClient( new HttpClientHandler { Credentials = CredCache});

        var path = new Uri(new Uri(Url), UrlGetDeviceInfo);
        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, path);
        
        using HttpResponseMessage response = await httpClient.SendAsync(request);
        
        if (response.StatusCode != HttpStatusCode.OK)
            return new BaseResponse<DeviceInfoResponse>()
            {
                Code = response.StatusCode,
            };
        
        var contents = await response.Content.ReadAsStringAsync();
        
        // TODO кривая расшифровка посылок
        var serializer = new ConfigurationContainer()
            .UseOptimizedNamespaces()
            .UseAutoFormatting()
            .EnableImplicitTyping(typeof(DeviceInfoResponse)).Create();

        var messageData = serializer.Deserialize<DeviceInfoResponse>(contents);

        return new BaseResponse<DeviceInfoResponse>()
        {
            Code = response.StatusCode,
            Data = messageData
        };
    }
}