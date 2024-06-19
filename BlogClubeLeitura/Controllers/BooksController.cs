using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogClubeLeitura.Data;
using BlogClubeLeitura.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlogClubeLeitura.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public BooksController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Books
        public async Task<IActionResult> Index(string searchQuery, int pageNumber = 1, int pageSize = 9)
        {
            var books = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.ToLower();
                books = books.Where(b => b.Title.ToLower().Contains(searchQuery));
            }

            var pagedBooks = await PaginatedList<Book>.CreateAsync(books, pageNumber, pageSize);

            foreach (var book in pagedBooks)
            {
                book.ModeRating = await CalculateModeRatingAsync(book.Id);
            }

            ViewData["searchQuery"] = searchQuery;
            return View(pagedBooks);
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Posts)
                    .ThenInclude(p => p.User)
                .Include(b => b.Ratings)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            book.ModeRating = await CalculateModeRatingAsync(book.Id);

            return View(book);
        }

        // Método para calcular a moda das estrelas
        private async Task<int> CalculateModeRatingAsync(int bookId)
        {
            var ratings = await _context.Ratings
                .Where(r => r.BookId == bookId)
                .ToListAsync();

            if (ratings.Count == 0)
                return 0;

            return ratings
                .GroupBy(r => r.Stars)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key ?? 0)
                .FirstOrDefault();
        }

        // POST: Books/DeletePost
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id = post?.BookId });
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Author,Description,PublishedDate")] Book book, IFormFile CoverImage)
        {
            if (ModelState.IsValid)
            {
                if (CoverImage != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(CoverImage.FileName);
                    string path = Path.Combine(wwwRootPath + "/images/books/", fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await CoverImage.CopyToAsync(fileStream);
                    }

                    book.CoverImagePath = "/images/books/" + fileName;
                }

                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Books/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,Description,CoverImagePath,PublishedDate")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}