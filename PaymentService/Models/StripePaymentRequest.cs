namespace PaymentService.Models
{
    public class StripePaymentRequest
    {
        public string ProductName { get; set; }
        public double Amount { get; set; }
    }
}
