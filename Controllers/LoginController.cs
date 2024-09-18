using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Library_management_system.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
namespace Library_management_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public LoginController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [HttpPost]
        [Route("Login")]
        public IActionResult LogIn(Login login)
        {
            string adminName = "Admin";
            string adminPassword = "Adminpass";
            if(login.UserName==adminName && login.Password==adminPassword)
            {
                var response = new
                {
                    Message = "login Succesful",
                    User = "Admin"
                };
            return Ok(response);
            }
            return Unauthorized("Invalid password and username");
        }
    }
}
