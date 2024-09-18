using Library_management_system.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library_management_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowController : ControllerBase
    {
        private static List<Borrow> borrows = new List<Borrow>();
        private static List<Book> books = new List<Book>();
        private static List<User> users = new List<User>();

        [HttpPost("borrow")]
        public IActionResult BorrowBook([FromBody] Borrow borrow)
        {
            // Check if the book is available
            var book = books.FirstOrDefault(b => b.Id == borrow.BookId);
            if (book == null)
            {
                return NotFound("Book not found");
            }

            // Check if the user exists
            var user = users.FirstOrDefault(u => u.UId == borrow.UserId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Check if the book is already borrowed
            var existingBorrow = borrows.FirstOrDefault(b => b.BookId == borrow.BookId && b.ReturnDate == null);
            if (existingBorrow != null)
            {
                return BadRequest("Book is already borrowed");
            }

            // Add the borrow record
            borrow.BorrowDate = DateTime.Now;
            borrows.Add(borrow);
            return Ok("Book borrowed successfully");
        }

        [HttpPost("return")]
        public IActionResult ReturnBook([FromBody] Borrow borrow)
        {
            var existingBorrow = borrows.FirstOrDefault(b => b.BookId == borrow.BookId && b.UserId == borrow.UserId && b.ReturnDate == null);
            if (existingBorrow == null)
            {
                return BadRequest("No record of this book being borrowed");
            }

            // Mark the book as returned
            existingBorrow.ReturnDate = DateTime.Now;
            return Ok("Book returned successfully");
        }

        [HttpGet("user-borrows/{userId}")]
        public IActionResult GetUserBorrows(int userId)
        {
            var userBorrows = borrows.Where(b => b.UserId == userId && b.ReturnDate == null).ToList();
            return Ok(userBorrows);
        }
    }

}
