using System.ComponentModel.DataAnnotations;
namespace Library_management_system.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Language { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }
    }

}
