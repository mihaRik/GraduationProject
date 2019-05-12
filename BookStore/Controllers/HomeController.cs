using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BookStore.Models;
using Google.Apis.Books.v1;
using Google.Apis.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using BookStore.Data;
using BookStore.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace BookStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;
        private readonly BookContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(IConfiguration config, BookContext db, UserManager<ApplicationUser> userManager)
        {
            _config = config;
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeIndexViewModel
            {
                Slider = _db.Books.OrderByDescending(b => b.CreatedAt).Take(10),
                MostCommentedBooks = _db.Books.OrderByDescending(b => b.Comments.Count).Take(4),
                MostFavoriteBooks = _db.Books.OrderByDescending(b => b.Favorites.Count).Take(4),
                MostViewedBooks = _db.Books.OrderByDescending(b => b.ViewCount).Take(4),
                MostRatedBooks = _db.Books.OrderByDescending(b => b.Rating.Count).Take(4),
                Banner = await _db.Banners.FirstOrDefaultAsync()
            };

            return View(model);
        }

        public async Task<IActionResult> About()
        {
            return View(await _db.Banners.FirstOrDefaultAsync());
        }

        public async Task<IActionResult> Contact()
        {
            var model = new ContactViewModel
            {
                Banner = await _db.Banners.FirstOrDefaultAsync(),
                User = User.Identity.IsAuthenticated ? await _userManager.GetUserAsync(User) : null,
                IsSent = false
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(Contact contact)
        {
            var model = new ContactViewModel
            {
                Banner = await _db.Banners.FirstOrDefaultAsync(),
                User = User.Identity.IsAuthenticated ? await _userManager.GetUserAsync(User) : null,
                IsSent = false
            };

            if (!ModelState.IsValid)
            {
                model.Contact = contact;
                return View(model);
            }

            await _db.Contacts.AddAsync(contact);
            await _db.SaveChangesAsync();

            model.IsSent = true;

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
