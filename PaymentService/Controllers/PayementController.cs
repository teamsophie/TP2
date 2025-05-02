using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Models;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        // Fake DB pour test
        private static List<PaiementModel> paiements = new List<PaiementModel>();
        private static int nextId = 1;

        // 🔁 GET all
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(paiements);
        }

        // 🔍 GET by ID
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var paiement = paiements.FirstOrDefault(p => p.Id == id);
            if (paiement == null) return NotFound();
            return Ok(paiement);
        }

        // ➕ POST
        [HttpPost]
        public IActionResult Create(PaiementModel model)
        {
            model.Id = nextId++;
            model.DatePaiement = DateTime.Now;
            paiements.Add(model);
            return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
        }

        // 🔁 PUT
        [HttpPut("{id}")]
        public IActionResult Update(int id, PaiementModel model)
        {
            var paiement = paiements.FirstOrDefault(p => p.Id == id);
            if (paiement == null) return NotFound();

            paiement.CommandeId = model.CommandeId;
            paiement.Montant = model.Montant;
            return Ok(paiement);
        }

        // ❌ DELETE
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var paiement = paiements.FirstOrDefault(p => p.Id == id);
            if (paiement == null) return NotFound();

            paiements.Remove(paiement);
            return NoContent();
        }
    }
}
