namespace JamalKhanah.Core.DTO.EntityDto
{
    public class InitalPaymentObjectDto
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string OrderDescription { get; set; }
        public string APIKey { get; set; }
        public string PaymentUrlIdentifier { get; set; }
    }
}
