using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProductService.Models;
using Microsoft.EntityFrameworkCore;

namespace ProductService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ProductDbContext _context;

        public ProductController(ProductDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Product>> Create(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Product updatedProduct)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            product.Title = updatedProduct.Title;
            product.Description = updatedProduct.Description;
            product.Price = updatedProduct.Price;
            product.Category = updatedProduct.Category;
            product.Image = updatedProduct.Image;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("import-fakestore")]
        public async Task<IActionResult> ImportFakeProducts()
        {
            using var httpClient = new HttpClient();
            try
            {
                var response = await httpClient.GetStringAsync("https://fakestoreapi.com/products");
                var products = JsonConvert.DeserializeObject<List<Product>>(response);

                if (products == null || !products.Any())
                    return BadRequest("No products fetched from FakeStore API");

                // Évite de réimporter si déjà présent
                if (_context.Products.Any())
                    return BadRequest("Products already imported");

                await _context.Products.AddRangeAsync(products);
                await _context.SaveChangesAsync();

                return Ok(new { message = $"{products.Count} products imported successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching products: {ex.Message}");
            }
        }
    }
}
