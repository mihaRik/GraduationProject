using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Data;
using BookStore.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    public class BookController : Controller
    {
        private readonly BookContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookController(BookContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var book = await _db.Books.FindAsync(id);

            if (book == null) return NotFound();

            var recommendedBook = book.Categories.ToList()[0]
                                       .Category.Books
                                       .Select(b => b.Book)
                                       .Where(b => b.Id != book.Id)
                                       .Take(12);

            var canAddToFavorites = false;
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                canAddToFavorites = !user.Favorites.Any(f => f.BookId == id);
            }

            book.ViewCount++;
            _db.Entry(book).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            var model = new BookDetailsViewModel
            {
                Book = book,
                RecommendedBooks = recommendedBook,
                CanAddToFavorites = canAddToFavorites
            };

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Rating(int? rating, int? bookId)
        {
            var res = new
            {
                code = 404,
                data = "Something goes wrong.",
                icon = "error"
            };

            if (rating == null || bookId == null) return Json(res);

            var rat = new Rating
            {
                BookId = (int)bookId,
                RatingValue = (int)rating,
                UserId = _userManager.GetUserId(User)
            };

            await _db.Rating.AddAsync(rat);
            await _db.SaveChangesAsync();

            res = new
            {
                code = 200,
                data = "Rating added.",
                icon = "success"
            };

            return Json(res);
        }

        [Authorize]
        public async Task<IActionResult> CheckCanReview(int? bookId)
        {
            var result = false;

            if (bookId == null) return Json(result);

            var book = await _db.Books.FindAsync(bookId);

            if (book == null) return Json(result);

            var userId = _userManager.GetUserId(User);

            result = book.Rating.Any(r => r.UserId == userId);

            return Json(result);
        }

        [Authorize]
        public async Task<IActionResult> RefreshReviewPartial(int bookId)
        {
            var book = await _db.Books.FindAsync(bookId);

            return PartialView("_ReviewsPartial", book);
        }

        public async Task<IActionResult> GetBooks()
        {
            return View();
        }

        public async Task<IActionResult> GetBooksAPI(int start, int length)
        {
            var model = new
            {
                total = _db.Books.Count(),
                items = _db.Books.Skip(start).Take(length),
            };

            return Json(model);
        }
    }
}