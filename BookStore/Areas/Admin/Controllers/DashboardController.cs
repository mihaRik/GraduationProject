using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Data;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Areas.Admin.Controllers
{
    [Area(Roles.Admin)]
    [Authorize(Roles = Roles.Admin)]
    public class DashboardController : Controller
    {
        private readonly BookContext _db;

        public DashboardController(BookContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var model = new AdminDashboardViewModel
            {
                Users = _db.Users.OrderByDescending(u => u.Id).Take(5),
                LastFiveAddedBooks = _db.Books.OrderByDescending(b => b.Id).Take(5),
                LastFiveComments = _db.Comments.OrderByDescending(c => c.Id).Take(5),
                LastFiveRatedBooks = _db.Rating.OrderByDescending(r => r.Id).Select(r => r.Book).Take(5)
            };

            return View(model);
        }
    }
}