using Library_management_system.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library_management_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRegister : ControllerBase
    {
        private readonly ApplicationDbContext dBContext;

        public UserRegister(ApplicationDbContext dBContext)
        {
            this.dBContext = dBContext;
        }
        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromBody] User userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if user already exists
            var existingUser = await dBContext.Users
                .FirstOrDefaultAsync(u => u.Email == userDto.Email);
            if (existingUser != null)
            {
                return Conflict("User with this email already exists.");
            }

            // Hash the password before saving
            userDto.Password = HashPassword(userDto.Password); // Ensure to hash the password

            // Save user to the database
            dBContext.Users.Add(userDto);
            await dBContext.SaveChangesAsync();

            return CreatedAtAction(nameof(RegisterUser), new { id = userDto.UId }, userDto);
        }

        // Method to hash the password (implement this according to your needs)
        private string HashPassword(string password)
        {
            // Use a proper hashing mechanism, e.g., BCrypt or similar
            return password; // Placeholder - replace with actual hashing logic
        }
    }
}
