using System.Xml.Serialization;

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
}