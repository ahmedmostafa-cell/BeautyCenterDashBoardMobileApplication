using Newtonsoft.Json;

namespace JamalKhanah.Core.DTO.NotificationModel;

public class NotificationModel
{
    [JsonProperty("deviceId")]
    public string DeviceId { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("body")]
    public string Body { get; set; }
}