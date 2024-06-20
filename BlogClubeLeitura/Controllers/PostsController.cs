using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogClubeLeitura.Data;
using BlogClubeLeitura.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace BlogClubeLeitura.Controllers
{
    [Route("[controller]")]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        [Route("")]
        public async Task<IActionResult> Index(string month, string year)
        {
            IQueryable<Post> postsQuery = _context.Posts.Include(p => p.Book).Include(p => p.User).Include(p => p.Rating);

            if (!User.IsInRole("Admin"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                postsQuery = postsQuery.Where(p => p.UserId == userId);
            }

            if (!string.IsNullOrEmpty(month) && !string.IsNullOrEmpty(year))
            {
                int monthNumber = int.Parse(month);
                int yearNumber = int.Parse(year);
                postsQuery = postsQuery.Where(p => p.PostedDate.Month == monthNumber && p.PostedDate.Year == yearNumber);
            }
            else if (!string.IsNullOrEmpty(month))
            {
                int monthNumber = int.Parse(month);
                postsQuery = postsQuery.Where(p => p.PostedDate.Month == monthNumber);
            }
            else if (!string.IsNullOrEmpty(year))
            {
                int yearNumber = int.Parse(year);
                postsQuery = postsQuery.Where(p => p.PostedDate.Year == yearNumber);
            }

            var posts = await postsQuery.ToListAsync();

            ViewData["month"] = month;
            ViewData["year"] = year;

            return View(posts);
        }

        [HttpGet]
        [Route("Details/{id:int}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Book)
                .Include(p => p.User)
                .Include(p => p.Rating)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create(int bookId)
        {
            var post = new Post { BookId = bookId };
            return View(post);
        }

        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Comment,BookId")] Post post, int Stars)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                post.PostedDate = DateTime.UtcNow;
                post.UserId = userId;

                var rating = new Rating
                {
                    Stars = Stars,
                    RatingDate = DateTime.UtcNow,
                    UserId = userId,
                    BookId = post.BookId
                };

                post.Rating = rating;

                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Books", new { id = post.BookId });
            }
            return View(post);
        }

        [HttpGet]
        [Route("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Rating)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        [HttpPost]
        [Route("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Comment,BookId")] Post post, int Stars)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    post.PostedDate = DateTime.UtcNow;
                    post.UserId = userId;

                    var existingPost = await _context.Posts
                        .Include(p => p.Rating)
                        .FirstOrDefaultAsync(p => p.Id == post.Id);

                    if (existingPost != null)
                    {
                        existingPost.Comment = post.Comment;
                        existingPost.PostedDate = post.PostedDate;
                        existingPost.UserId = post.UserId;

                        if (existingPost.Rating != null)
                        {
                            existingPost.Rating.Stars = Stars;
                            existingPost.Rating.RatingDate = DateTime.UtcNow;
                        }
                        else
                        {
                            existingPost.Rating = new Rating
                            {
                                Stars = Stars,
                                RatingDate = DateTime.UtcNow,
                                UserId = userId,
                                BookId = post.BookId,
                                PostId = post.Id
                            };
                        }

                        _context.Update(existingPost);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Books", new { id = post.BookId });
            }
            return View(post);
        }

        [HttpGet]
        [Route("Delete/{id:int}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Book)
                .Include(p => p.User)
                .Include(p => p.Rating)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Post excluído com sucesso.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
