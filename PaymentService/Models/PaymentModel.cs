namespace PaymentService.Models
{
    public class PaiementModel
    {
        public int Id { get; set; }
        public int CommandeId { get; set; }
        public double Montant { get; set; }
        public DateTime DatePaiement { get; set; } = DateTime.Now;
    }
}
