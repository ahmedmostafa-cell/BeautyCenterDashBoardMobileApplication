namespace JamalKhanah.Core.Helpers;

public class PaymentSettings
{
    public bool IsLive { get; set; }
    public string TestSecretKey { get; set; }
    public string TestPublishableKey { get; set; }
    public string LiveSecretKey { get; set; }
    public string LivePublishableKey { get; set; }
    public string Currency { get; set; }
}