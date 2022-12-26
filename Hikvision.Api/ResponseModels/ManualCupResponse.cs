namespace Hikvision.Api.ResponseModels;

public class ManualCupResponse
{
    /// <summary>
    /// Бит успешности распозавания номера
    /// </summary>
    public bool IsRecognize { get; set; }
    
    /// <summary>
    /// Получили ли фото в посылке
    /// </summary>
    public bool IncludePhoto { get; set; }
    /// <summary>
    /// Распознаный номер
    /// </summary>
    public string Number { get; set; }
    /// <summary>
    /// Байтовый потом с изображением jpeg
    /// </summary>
    public byte[] Image { get; set; }
    /// <summary>
    /// Время прихода посылки, генерируется либой
    /// </summary>
    public DateTime DateTime { get; set; }
    public static ManualCupResponse FromBinary(byte[] bytes)
    {
        var response = new ManualCupResponse()
        {
            IncludePhoto = false,
            IsRecognize = false,
            DateTime = DateTime.Now
        };
        
        if (bytes.Length == 272)
        {
            // Приходит XML c ответом - OK - обычно означает что камера отдает ответ другому клиенту
            return response;
        }

        if (bytes.Length > 300)
        {
            // Копируем номер
            //var numberByte = new byte[12];
            // Array.Copy(bytes, 88, numberByte, 0, 12);
            var numberByte = bytes.Skip(88).Take(12).Where(x => x != 0).ToArray();
            string number = System.Text.Encoding.UTF8.GetString(numberByte);
            
            var image = bytes.Skip(764).ToArray();
            // # FF D8 FF - 764
            // # FF D9
            response.IsRecognize = number != "unknown";
            response.IncludePhoto = true;
            response.Image = image;
            response.Number = number;
            
            return response;
        }

        return response;
    }
}