namespace JamalKhanah.Core.DTO.EntityDto
{
    public class SavePaymentDto
    {
        /// <summary>
        /// Payment’s unique ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Payment status. (default: initiated)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// A positive integer in the smallest currency unit (e.g 100 cents to charge an amount of $1.00, 100 halalah to charge an amount of 1 SAR, or 1 to charge an amount of ¥1, a 0-decimal? currency) representing how much to charge the card. The minimum amount is $0.50 (or equivalent in charge currency).
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// Transaction fee in halals.
        /// </summary>
        public decimal? Fee { get; set; }

        /// <summary>
        /// 3-letter ISO code for currency. E.g., SAR, CAD, USD. (default: SAR)
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Refunded amount in halals. (default: 0)
        /// </summary>
        public decimal? Refunded { get; set; }

        /// <summary>
        /// Datetime of refunded. (default: null)
        /// </summary>
        public DateTime? Refunded_at { get; set; }

        /// <summary>
        /// Captured amount in halals. (default: 0)
        /// </summary>
        public decimal? Captured { get; set; }

        /// <summary>
        /// Datetime of authroized payment captured. (default: null)
        /// </summary>
        public DateTime? Captured_at { get; set; }

        /// <summary>
        /// Datetime of voided. (default: null)
        /// </summary>
        public DateTime? Voided_at { get; set; }

        /// <summary>
        /// Payment description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// ID of the invoice this payment is for if one exists.(default: null)
        /// </summary>
        public string Invoice_id { get; set; }

        /// <summary>
        /// User IP
        /// </summary>
        public string Ip { get; set; }
        public string Amount_format { get; set; }
        public string Fee_format { get; set; }
        public string Refunded_format { get; set; }
        public string Captured_format { get; set; }

        /// <summary>
        /// Page url in customer’s site for final redirection. (used for creditcard 3-D secure and form payment)
        /// </summary>
        public string Callback_url { get; set; }

        /// <summary>
        /// Creation timestamp in ISO 8601 format.
        /// </summary>
        public DateTime? Created_at { get; set; }

        /// <summary>
        /// Modification timestamp in ISO 8601 format.
        /// </summary>
        public DateTime? Updated_at { get; set; }
        public Source Source { get; set; }
    }

    public class Source
    {
        /// <summary>
        /// Type of payment, creditcard.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Credit card’s company mada or visa or master or amex
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// Credit card’s holder name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Credit card’s masked number
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Payment’s internal gateway identifier
        /// </summary>
        public string Gateway_id { get; set; }

        /// <summary>
        /// Payment’s bank reference number
        /// </summary>
        public string Reference_number { get; set; }

        /// <summary>
        /// Card’s token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Payment gateway message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// URL to complete 3-D secure transaction authorization at bank gateway
        /// </summary>
        public string Transaction_url { get; set; }
    }

}
