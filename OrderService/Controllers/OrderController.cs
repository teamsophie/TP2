using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Models;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderDbContext _context;

        public OrderController(OrderDbContext context)
        {
            _context = context;
        }
        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<Order>> GetAll()
        {
            return Ok(_context.Orders.ToList());
        }
        [Authorize]

        [HttpGet("{id}")]
        public ActionResult<Order> GetById(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }
        [Authorize]

        [HttpPost]
        public ActionResult<Order> Create(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }
        [Authorize]

        [HttpPut("{id}")]
        public IActionResult Update(int id, Order updatedOrder)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
                return NotFound();

            order.UserId = updatedOrder.UserId;
            order.ProductId = updatedOrder.ProductId;
            order.Quantity = updatedOrder.Quantity;

            _context.SaveChanges();
            return NoContent();
        }
        [Authorize]

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
                return NotFound();

            _context.Orders.Remove(order);
            _context.SaveChanges();
            return NoContent();
        }
    }
}