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

namespace BookStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;
        private readonly BookContext _db;

        public HomeController(IConfiguration config, BookContext db)
        {
            _config = config;
            _db = db;
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
            return View(await _db.Banners.FirstOrDefaultAsync());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
