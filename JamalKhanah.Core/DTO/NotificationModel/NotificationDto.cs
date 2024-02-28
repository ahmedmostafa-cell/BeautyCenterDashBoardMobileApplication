using System.ComponentModel.DataAnnotations;

namespace JamalKhanah.Core.DTO.NotificationModel;

public class NotificationDto
{
    [Required]
    public string Token { get; set; }
}