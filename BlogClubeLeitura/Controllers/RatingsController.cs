using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogClubeLeitura.Data;
using BlogClubeLeitura.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlogClubeLeitura.Controllers
{
    public class RatingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RatingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Ratings
        public async Task<IActionResult> Index(string searchQuery, int pageNumber = 1, int pageSize = 9)
        {
            var ratingsQuery = _context.Ratings
                .Include(r => r.Book)
                .GroupBy(r => r.Book)
                .Select(g => new BookRatingsViewModel
                {
                    BookTitle = g.Key.Title,
                    BookId = g.Key.Id,
                    FiveStarsCount = g.Count(r => r.Stars == 5),
                    FourStarsCount = g.Count(r => r.Stars == 4),
                    ThreeStarsCount = g.Count(r => r.Stars == 3),
                    TwoStarsCount = g.Count(r => r.Stars == 2),
                    OneStarCount = g.Count(r => r.Stars == 1),
                    CoverImagePath = g.Key.CoverImagePath
                }).AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                ratingsQuery = ratingsQuery.Where(b => b.BookTitle.ToLower().Contains(searchQuery.ToLower()));
            }

            var pagedRatings = await PaginatedList<BookRatingsViewModel>.CreateAsync(ratingsQuery, pageNumber, pageSize);
            return View(pagedRatings);
        }
        // GET: Ratings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rating = await _context.Ratings
                .Include(r => r.Book)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rating == null)
            {
                return NotFound();
            }

            return View(rating);
        }

        // GET: Ratings/Create
        public IActionResult Create()
        {
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName");
            return View();
        }

        // POST: Ratings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Stars,RatingDate,BookId,UserId")] Rating rating)
        {
            if (ModelState.IsValid)
            {
                _context.Add(rating);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", rating.BookId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", rating.UserId);
            return View(rating);
        }
        public int GetModeRating(int bookId)
        {
            var ratings = _context.Ratings.Where(r => r.BookId == bookId).ToList();
            if (ratings == null || !ratings.Any())
                return 0;

            var mode = ratings.GroupBy(r => r.Stars)
                              .OrderByDescending(g => g.Count())
                              .First().Key;

            return mode ?? 0;
        }

        // GET: Ratings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rating = await _context.Ratings.FindAsync(id);
            if (rating == null)
            {
                return NotFound();
            }
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", rating.BookId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", rating.UserId);
            return View(rating);
        }


        // POST: Ratings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Stars,RatingDate,BookId,UserId")] Rating rating)
        {
            if (id != rating.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rating);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RatingExists(rating.Id))
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
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", rating.BookId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", rating.UserId);
            return View(rating);
        }

        // GET: Ratings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rating = await _context.Ratings
                .Include(r => r.Book)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rating == null)
            {
                return NotFound();
            }

            return View(rating);
        }

        // POST: Ratings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rating = await _context.Ratings.FindAsync(id);
            if (rating != null)
            {
                _context.Ratings.Remove(rating);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool RatingExists(int id)
        {
            return _context.Ratings.Any(e => e.Id == id);
        }
    }

    
    
}