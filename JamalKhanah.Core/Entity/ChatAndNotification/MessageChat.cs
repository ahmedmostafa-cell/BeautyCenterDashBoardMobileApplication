using JamalKhanah.Core.Entity.ApplicationData;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace JamalKhanah.Core.Entity.ChatAndNotification;

public class MessageChat
{
    public int Id { get; set; }
    public string Message { get; set; }

    public DateTime MessageDate { get; set; } = DateTime.Now;

    [ForeignKey("SendUser")]
    public string SendUserId { get; set; }

    public virtual ApplicationUser SendUser { get; set; }

    [ForeignKey("ReceiveUser")]
    public string ReceiveUserId { get; set; }

    public virtual ApplicationUser ReceiveUser { get; set; }

    [NotMapped]
    public IFormFile Img { get; set; }

    public string ImgUrl { get; set; }

    public bool IsImg { get; set; }

    public bool IsRead { get; set; }
}