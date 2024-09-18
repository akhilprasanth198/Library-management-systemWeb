using System.ComponentModel.DataAnnotations;

namespace Library_management_system.Models
{
    public class User
    {
        [Key]
        public int UId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } 
    }
}
