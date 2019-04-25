using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BookStore.Areas.Admin.Controllers
{
    [Area(Roles.Admin)]
    //[Authorize(Roles = Roles.Admin)]
    public class BookController : Controller
    {
        private readonly BookContext _db;

        public BookController(BookContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetBooks(int draw, int start, int length)
        {
            var model = new
            {
                draw,
                recordsTotal = _db.Books.Where(b => b.Title.Contains("")).Count(),
                recordsFiltered = _db.Books.Where(b => b.Title.Contains("")).Count(),
                data = _db.Books
                          .Where(b => b.Title.Contains(""))
                          .Skip(start)
                          .Take(length)
                          .OrderBy(b => b.Title)
                          .Select(b => new List<object>
                {
                    b.Title.Length > 20 ? b.Title.Substring(0, 20) + "..." : b.Title,
                    b.Authors.Select(a => new List<object> { a.Author.Fullname }),
                    b.Categories.Select(c => new List<object> { c.Category.Name }),
                    b.Description.Length > 20 ? b.Description.Substring(0, 20) + "..." : b.Description,
                    b.Publisher,
                    b.Language,
                    b.PageCount,
                    b.PublishedDate,
                    $"<img src='{b.Images.Thumbnail}' class='img-thumbnail' />"
                })
            };

            return Json(model);
        }
    }
}