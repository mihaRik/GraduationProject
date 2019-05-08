using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    [Authorize]
    public class FavoriteController : Controller
    {
        private readonly BookContext _db;
        private UserManager<ApplicationUser> _userManager;

        public FavoriteController(BookContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> GetUserFavorites()
        {
            var user = await _userManager.GetUserAsync(User);

            var favorites = user.Favorites.OrderByDescending(f => f.AddedAt).Take(6);

            return PartialView("_FavoritesPartial", favorites);
        }

        public async Task<IActionResult> AddToFavorites(int? bookId)
        {
            var res = new
            {
                code = 404,
                data = "Something goes wrong.",
                icon = "error"
            };

            if (bookId == null) return Json(res);

            var book = await _db.Books.FindAsync(bookId);

            if (book == null) return Json(res);

            var user = await _userManager.GetUserAsync(User);

            var isFavoriteBookAlready = user.Favorites.Any(f => f.BookId == bookId);

            res = new
            {
                code = 204,
                data = "This book already in your favorites.",
                icon = "info"
            };

            if (isFavoriteBookAlready) return Json(res);

            var pivot = new BookFavoritePivot
            {
                BookId = (int)bookId,
                UserId = user.Id,
                AddedAt = DateTime.Now
            };

            await _db.BookFavorites.AddAsync(pivot);
            await _db.SaveChangesAsync();

            res = new
            {
                code = 200,
                data = "Book add to favorites",
                icon = "success"
            };

            return Json(res);
        }

        public async Task<IActionResult> RemoveFromFavorites(int? bookId)
        {
            var res = new
            {
                code = 404,
                data = "Something goes wrong.",
                icon = "error"
            };

            if (bookId == null) return Json(res);

            var book = await _db.Books.FindAsync(bookId);

            if (book == null) return Json(res);

            var user = await _userManager.GetUserAsync(User);

            if (!user.Favorites.Any(f => f.BookId == bookId)) return Json(res);

            var pivot = user.Favorites.FirstOrDefault(f => f.Book.Id == bookId);

            _db.Entry(pivot).State = EntityState.Deleted;
            await _db.SaveChangesAsync();

            res = new
            {
                code = 200,
                data = "This book removed from your favorites.",
                icon = "success"
            };

            return Json(res);
        }

        public async Task<IActionResult> FavCount()
        {
            var user = await _userManager.GetUserAsync(User);

            return Json(user.Favorites.Count);
        }

        public async Task<IActionResult> ShowMore()
        {
            var user = await _userManager.GetUserAsync(User);

            return View(user.Favorites.OrderByDescending(f => f.AddedAt));
        }
    }
}