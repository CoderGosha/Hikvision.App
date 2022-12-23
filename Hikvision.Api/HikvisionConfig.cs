namespace Hikvision.Api;

public class HikvisionConfig
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
}