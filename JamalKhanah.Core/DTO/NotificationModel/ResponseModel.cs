using Newtonsoft.Json;

namespace JamalKhanah.Core.DTO.NotificationModel;

public class ResponseModel
{
    [JsonProperty("isSuccess")]
    public bool IsSuccess { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }
}