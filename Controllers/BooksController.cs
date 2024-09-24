using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Library_management_system.Models;

namespace Library_management_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            return await _context.Books.ToListAsync();
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBook", new { id = book.Id }, book);
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpGet("search")]
        public IActionResult SearchBooks(string? author = null, string? language = null, string? title = null)
        {
            var books = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(author))
            {
                books = books.Where(b => b.Author.ToLower().Contains(author.ToLower()));
            }

            if (!string.IsNullOrEmpty(language))
            {
                books = books.Where(b => b.Language.ToLower().Contains(language.ToLower()));
            }

            if (!string.IsNullOrEmpty(title))
            {
                books = books.Where(b => b.Title.ToLower().Contains(title.ToLower()));
            }

            var filteredBooks = books.ToList();

            if (!filteredBooks.Any())
            {
                return NotFound("No books found matching the search criteria.");
            }

            return Ok(filteredBooks);
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

        // Borrow a Book
        [HttpPost("borrow/{bookId}/{userId}")]
        public async Task<IActionResult> BorrowBook(int bookId, int userId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                return NotFound();
            }

            if (book.Quantity <= 0)
            {
                return BadRequest("Book is not available.");
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
        // Return a Book
        [HttpPost("return/{bookId}/{userId}")]
        public async Task<IActionResult> ReturnBook(int bookId, int userId)
        {
            // Find the book by its ID
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                return NotFound(new { message = "Book not found." });
            }

            // Increase the quantity of the book
            book.Quantity += 1;
            _context.Books.Update(book);

            // Find the borrow entry for this book and user
            var borrowEntry = await _context.Borrows
                .FirstOrDefaultAsync(b => b.BookId == bookId && b.UserId == userId && b.ReturnDate == null);

            if (borrowEntry == null)
            {
                return NotFound(new { message = "Borrow record not found." });
            }

            // Update the return date in the borrow record
            borrowEntry.ReturnDate = DateTime.Now;
            _context.Borrows.Update(borrowEntry);

            // Save the changes to the database
            await _context.SaveChangesAsync();

            return Ok(new { message = "Book returned successfully." });
        }




        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
