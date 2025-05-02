using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PaymentService.Models;
using Stripe;
using Stripe.Checkout;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly PaymentDbContext _context;

        public WebhookController(IConfiguration config, PaymentDbContext context)
        {
            _configuration = config;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Handle()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeSecret = _configuration["Stripe:WebhookSecret"];
                var stripeSignature = Request.Headers["Stripe-Signature"];
                var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, stripeSecret);

                // 🔐 Vérification de l’événement (manuellement, car Events.CheckoutSessionCompleted est non dispo)
                if (stripeEvent.Type == "checkout.session.completed")
                {
                    if (stripeEvent.Data.Object is Session session)
                    {
                        var paiement = new PaiementModel
                        {
                            CommandeId = int.TryParse(session.Metadata["commandeId"], out var cid) ? cid : 0,
                            Montant = (session.AmountTotal ?? 0) / 100.0,
                            DatePaiement = DateTime.UtcNow
                        };

                        _context.Paiements.Add(paiement);
                        await _context.SaveChangesAsync();
                    }
                }

                return Ok();
            }
            catch (StripeException e)
            {
                Console.WriteLine($"Stripe Webhook Error: {e.Message}");
                return BadRequest();
            }
        }
    }
}
