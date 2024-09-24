using Library_management_system.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library_management_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BorrowController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Book Search
        [HttpGet("searches")]
        public async Task<ActionResult<IEnumerable<Book>>> SearchBooks(string bookname)
        {
            var books = await _context.Books
                .Where(b => b.Title.Contains(bookname) || b.Author.Contains(bookname) || b.Language.Contains(bookname))
                .ToListAsync();

            return Ok(books);
        }

        [HttpPost("borrow/{bookId}/{userId}")]
        public async Task<IActionResult> BorrowBook(int bookId, int userId)
        {
            // Check if book exists
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                return NotFound(new { message = "Book not found." });
            }

            // Check if book is available
            if (book.Quantity <= 0)
            {
                return BadRequest(new { message = "Book is not available." });
            }

            // Check if user exists
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Update book quantity
            book.Quantity -= 1;
            _context.Books.Update(book);

            // Log the borrow action
            var borrowEntry = new Borrow
            {
                BookId = bookId,
                UserId = userId,
                BorrowDate = DateTime.Now,
                ReturnDate = null
            };

            _context.Borrows.Add(borrowEntry);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Book borrowed successfully.", book });
        }

        // Update Borrow Log (Return Book)
        [HttpPut("return")]
        public async Task<IActionResult> ReturnBook([FromBody] ReturnBookModel model)
        {
            var borrowEntry = await _context.Borrows
                .FirstOrDefaultAsync(b => b.BookId == model.BookId && b.UserId == model.UserId && b.ReturnDate == null);

            if (borrowEntry == null)
            {
                return NotFound(new { message = "No active borrow found for this book and user." });
            }

            // Set the return date
            borrowEntry.ReturnDate = DateTime.Now;

            // Update the book quantity
            var book = await _context.Books.FindAsync(model.BookId);
            if (book != null)
            {
                book.Quantity += 1;
            }

            _context.Borrows.Update(borrowEntry);
            _context.Books.Update(book);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Book returned successfully." });
        }


    }

}
