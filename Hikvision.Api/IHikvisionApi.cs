using Hikvision.Api.ResponseModels;

namespace Hikvision.Api;

public interface IHikvisionApi
{
    /// <summary>
    /// Проверяет валидность логина и пароля
    /// </summary>
    /// <returns></returns>
    public Task<BaseResponse> IsAuthenticatedAsync();
    
    /// <summary>
    /// Получает данные о камере
    /// </summary>
    /// <returns></returns>
    public Task<BaseResponse<DeviceInfoResponse>> DeviceInfoAsync();

    /// <summary>
    /// Получает фотографию с распознаванием 
    /// </summary>
    /// <returns></returns>
    public Task<BaseResponse<ManualCupResponse>> ManualCupAsync();
}