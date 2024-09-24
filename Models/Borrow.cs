using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library_management_system.Models
{
    public class Borrow
    {
        [Key]
        public int BrId { get; set; }

        [ForeignKey("Book")]
        public int BookId { get; set; }
        public Book Book { get; set; }  // Navigation property

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }  // Navigation property

        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
