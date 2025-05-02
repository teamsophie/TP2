using Microsoft.AspNetCore.Mvc;
using CartService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace CartService.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly CartDbContext _context;

        public CartController(CartDbContext context)
        {
            _context = context;
        }
        [Authorize]
        // 🔹 GET: api/cart/user/3
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetCartItemsByUser(int userId)
        {
            var items = await _context.CartItems.Where(c => c.UserId == userId).ToListAsync();
            return Ok(items);
        }
        [Authorize]
        // 🔹 POST: api/cart/add
        [HttpPost("add")]
        public async Task<ActionResult<CartItem>> AddToCart([FromBody] CartItem item)
        {
            _context.CartItems.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCartItemsByUser), new { userId = item.UserId }, item);
        }
        [Authorize]
        // 🔹 DELETE: api/cart/remove/5
        [HttpDelete("remove/{id}")]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var item = await _context.CartItems.FindAsync(id);
            if (item == null)
                return NotFound();

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [Authorize]
        // 🔁 Modifier la quantité d’un article
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateQuantity(int id, [FromBody] int newQuantity)
        {
            var item = await _context.CartItems.FindAsync(id);
            if (item == null)
                return NotFound();

            item.Quantity = newQuantity;
            await _context.SaveChangesAsync();
            return Ok(item);
        }
        [Authorize]

        // 🗑️ Vider le panier d’un utilisateur
        [HttpDelete("clear/{userId}")]
        public async Task<IActionResult> ClearCart(int userId)
        {
            var items = _context.CartItems.Where(c => c.UserId == userId);
            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
