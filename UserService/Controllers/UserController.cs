using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using UserService.Models;
using UserService.Models.UserService.Models;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserDbContext _context;
        private readonly JwtSettings _jwtSettings;

        public UserController(UserDbContext context, IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings.Value;
        }

        // 🔐 POST: api/User/register
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(User user)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser != null)
                return Conflict("Un utilisateur avec cet email existe déjà.");

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // 🔐 POST: api/User/login
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Email == model.Email && u.MotDePasse == model.MotDePasse);

            if (user == null)
                return Unauthorized("Email ou mot de passe incorrect.");

            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        // 🔑 Génération du token JWT
        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim("id", user.Id.ToString()),
                new Claim("role", user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // 🔒 GET: api/User
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // 🔒 GET: api/User/{id}
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return user;
        }

        // 🔒 PUT: api/User/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id) return BadRequest();

            _context.Entry(user).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Users.AnyAsync(e => e.Id == id)) return NotFound();
                else throw;
            }
            return NoContent();
        }

        // 🔒 DELETE: api/User/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // 🌍 GET: api/User/import-dummy-users
        [HttpGet("import-dummy-users")]
        public async Task<IActionResult> ImportDummyUsers()
        {
            using var httpClient = new HttpClient();
            try
            {
                var response = await httpClient.GetStringAsync("https://dummyjson.com/users");
                var dummyResponse = JsonDocument.Parse(response);

                var usersElement = dummyResponse.RootElement.GetProperty("users");
                var users = JsonSerializer.Deserialize<List<User>>(usersElement.ToString());

                if (users == null || !users.Any())
                    return BadRequest("Aucun utilisateur trouvé dans DummyJSON");

                if (_context.Users.Any())
                    return BadRequest("Des utilisateurs existent déjà en base");

                await _context.Users.AddRangeAsync(users);
                await _context.SaveChangesAsync();

                return Ok(new { message = $"{users.Count} utilisateurs importés avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de l'import : {ex.Message}");
            }
        }
    }
}
