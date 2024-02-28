namespace JamalKhanah.Core.Entity.ChatAndNotification;

public class UserConnection
{
    public int Id { get; set; }
    public string UserName { get; set; }

    public string Connection { get; set; }

    public DateTime ConnectionTime { get; set; } = DateTime.Now;
}