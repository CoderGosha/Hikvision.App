using Hikvision.Api.ResponseModels;

namespace Hikvision.Api;

public class HikvisionSimpleApi : IHikvisionApi
{
    private readonly HikvisionClient client;

    public HikvisionSimpleApi(HikvisionConfig config)
    {
        client = new HikvisionClient(config);
    }

    public async Task<BaseResponse> IsAuthenticatedAsync()
    {
        return await client.DeviceInfoAsync();
    }

    public async Task<BaseResponse<DeviceInfoResponse>> DeviceInfoAsync()
    {
        return await client.DeviceInfoAsync();
    }

    public async Task<BaseResponse<ManualCupResponse>> ManualCupAsync()
    {
        return await client.ManualCupAsync();
    }

    public async Task<BaseResponse<PictureResponse>> PictureAsync()
    {
        return await client.PictureAsync();
    }
}