using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class CommentController : Controller
    {
        private readonly BookContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentController(BookContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Add(string comment, int bookId)
        {
            var result = new
            {
                code = 404,
                data = "Please login"
            };

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var cmnt = new Comment
                {
                    UserId = await _userManager.GetUserIdAsync(user),
                    BookId = bookId,
                    Content = comment,
                    PublishAt = DateTime.Now,
                    LikeCount = 0
                };

                await _db.Comments.AddAsync(cmnt);
                await _db.SaveChangesAsync();

                return PartialView("_CommentsPartial", _db.Comments.Where(c => c.BookId == bookId));
            }

            return Json(result);
        }
    }
}