using Microsoft.AspNetCore.Mvc;
using Library_management_system.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Library_management_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserLoginController : ControllerBase
    {
        private readonly ApplicationDbContext _context; // Database context for accessing data

        public UserLoginController(ApplicationDbContext Context)
        {
            _context = Context; // Dependency injection of the context
        }

        //// Model for capturing login request data
        //public class LoginRequest
        //{
        //    [Required] // Ensures UserName is provided
        //    public string UserName { get; set; }

        //    [Required] // Ensures Password is provided
        //    public string Password { get; set; }
        //}

        //// Model for structuring login response
        //public class LoginResponse
        //{
        //    public string Message { get; set; } // Message indicating login success or failure
        //    public string User { get; set; }    // The username of the logged-in user
        //}

        //[HttpPost]
        //[Route("Login")] // Endpoint for user login
        //public async Task<IActionResult> LogIn([FromBody] LoginRequest login)
        //{
        //    // Validate the incoming request model
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState); // Return a 400 Bad Request if invalid
        //    }

        //    // Retrieve the user from the database using the provided username
        //    var user = await dbContext.Users
        //        .FirstOrDefaultAsync(u => u.Name == login.UserName);

        //    // If no user is found, return Unauthorized
        //    if (user == null)
        //    {
        //        return Unauthorized("Invalid username or password");
        //    }

        //    // Compare the provided password with the stored password
        //    if (login.Password==user.Password) // Replace with hashed password comparison in production
        //    {
        //        // Prepare a successful login response
        //        var response = new LoginResponse
        //        {
        //            Message = "Login successful",
        //            User = user.Name // Return the username of the logged-in user
        //        };
        //        return Ok(response); // Return a 200 OK response with the login info
        //    }

        //    // If the password does not match, return Unauthorized
        //    return Unauthorized("Invalid username or password");
        //}

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login loginModel)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Name == loginModel.UserName && u.Password == loginModel.Password);

            if (user == null)
            {
                return BadRequest(new { message = "Invalid username or password" });
            }

            // Return success message and userId
            return Ok(new
            {
                message = "Login successful",
                userId = user.UId  // Assuming the user's ID is UId
            });
        }

    }
}
