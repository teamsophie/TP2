namespace PaiementService.Models
{
    public class PaiementRequest
    {
        public long Amount { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public string Source { get; set; } 
    }
}
