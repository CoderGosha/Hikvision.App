using System.Net;

namespace Hikvision.Api.ResponseModels;

public class BaseResponse
{
    /// <summary>
    /// Успешность операции
    /// </summary>
    public bool IsSuccess => Code == HttpStatusCode.OK;

    /// <summary>
    /// Код от Hikvision API
    /// </summary>
    public HttpStatusCode Code { get; set; }
    /// <summary>
    /// Ответ в случае ошибки
    /// </summary>
    public string Message { get; set; }
}

public class BaseResponse<T> : BaseResponse
{
    public T Data { get; set; } 
}