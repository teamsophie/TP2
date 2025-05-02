using Microsoft.AspNetCore.Mvc;
using Stripe;
using PaiementService.Models;
using PaiementService.Data;

namespace PaiementService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaiementController : ControllerBase
    {
        private readonly PaiementDbContext _context;

        public PaiementController(PaiementDbContext context)
        {
            _context = context;
        }

        [HttpPost("charge")]
        public async Task<IActionResult> Charge([FromBody] PaiementRequest request)
        {
            var options = new ChargeCreateOptions
            {
                Amount = request.Amount,
                Currency = request.Currency,
                Description = request.Description,
                Source = request.Source, // Stripe test token, e.g., "tok_visa"
            };

            var service = new ChargeService();
            Charge charge;

            try
            {
                charge = await service.CreateAsync(options);
            }
            catch (StripeException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }

            var payment = new Paiement
            {
                StripeChargeId = charge.Id,
                Amount = request.Amount,
                Currency = request.Currency,
                Description = request.Description,
                Status = charge.Status,
                CreatedAt = DateTime.UtcNow
            };

            _context.Paiements.Add(payment);
            await _context.SaveChangesAsync();

            if (charge.Status == "succeeded")
            {
                return Ok(new { success = true, charge.Id });
            }
            else
            {
                return BadRequest(new { success = false, charge.FailureMessage });
            }
        }
    }
}
