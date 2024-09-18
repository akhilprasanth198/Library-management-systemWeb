using System.ComponentModel.DataAnnotations;

namespace Library_management_system.Models
{
    public class Borrow
    {
        [Key]
        public int BrId { get; set; }
        public int BookId { get; set; }
        public int UserId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
