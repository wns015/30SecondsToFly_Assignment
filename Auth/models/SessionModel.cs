using Newtonsoft.Json;

public class SessionModel {
    [JsonProperty("accessToken")]
    public string AccessToken { get; set; }
    [JsonProperty("username")]
    public string Username { get; set; }
}