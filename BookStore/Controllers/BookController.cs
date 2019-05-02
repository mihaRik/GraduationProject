using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Data;
using BookStore.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class BookController : Controller
    {
        private readonly BookContext _db;

        public BookController(BookContext db)
        {
            _db = db;
        }
        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var book = await _db.Books.FindAsync(id);

            if (book == null) return NotFound();

            var model = new BookDetailsViewModel
            {
                Book = book,
                RecommendedBooks = book.Categories.ToList()[0]
                                       .Category.Books
                                       .Select(b => b.Book)
                                       .Where(b => b.Id != book.Id)
                                       .Take(12)
            };

            return View(model);
        }

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
                RatingValue = (int)rating
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
    }
}