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
        [HttpPut("return/{borrowId}")]
        public async Task<IActionResult> UpdateBorrowLog(int borrowId)
        {
            // Find the borrow record
            var borrow = await _context.Borrows.FindAsync(borrowId);
            if (borrow == null)
            {
                return NotFound("Borrow record not found.");
            }

            // Check if the book has already been returned
            if (borrow.ReturnDate != null)
            {
                return BadRequest("Book has already been returned.");
            }

            // Set the return date to now
            borrow.ReturnDate = DateTime.Now;
            _context.Borrows.Update(borrow);

            // Find the associated book
            var book = await _context.Books.FindAsync(borrow.BookId);
            if (book != null)
            {
                // Increase the quantity of the book when it's returned
                book.Quantity += 1;
                _context.Books.Update(book);
            }

            // Save changes
            await _context.SaveChangesAsync();

            return Ok(new { message = "Borrow log updated successfully.", borrow });
        }

    }

}
