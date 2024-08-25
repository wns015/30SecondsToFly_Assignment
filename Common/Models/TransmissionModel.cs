using Newtonsoft.Json;

public class TransmissionModel
{
    [JsonProperty("encryptedString")]
    public string EncryptedString { get; set; }
}