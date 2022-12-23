using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Hikvision.Api.ResponseModels;

[XmlRoot("DeviceInfo")]
public class DeviceInfoResponse
{
    [XmlElement("deviceName")]
    public string DeviceName { get; set; }
 
    [XmlElement("deviceID")]
    public string DeviceID { get; set; }
    
    [XmlElement("model")]
    public string Model { get; set; }
    
    [XmlElement("firmwareVersion")]
    public string FirmwareVersion { get; set; }
   
    [XmlElement("firmwareReleasedDate")]
    public string FirmwareReleasedDate { get; set; }
    
    [XmlElement("macAddress")]
    public string MacAddress { get; set; }
    
    [XmlElement("serialNumber")]
    public string SerialNumber { get; set; }

    public static DeviceInfoResponse FromXml(string xml)
    {
        if (string.IsNullOrEmpty(xml))
            return new DeviceInfoResponse();
        
        var xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        if (xmlDocument.FirstChild != null)
            xmlDocument.RemoveChild(xmlDocument.FirstChild); 

        var serializedXmlNode = JsonConvert.SerializeXmlNode(
            xmlDocument, 
            Newtonsoft.Json.Formatting.Indented, 
            true
        );
        var messageData = JsonConvert.DeserializeObject<DeviceInfoResponse>(serializedXmlNode);
        return messageData ?? new DeviceInfoResponse();
    }
}