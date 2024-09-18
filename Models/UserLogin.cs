using System.ComponentModel.DataAnnotations;

namespace Library_management_system.Models
{
    public class UserLogin
    {
        [Key]
        public int LogId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
