namespace PaiementService.Models
{
    public class Paiement
    {
        public int Id { get; set; }
        public string StripeChargeId { get; set; }
        public long Amount { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
