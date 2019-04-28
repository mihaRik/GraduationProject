using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Data;
using BookStore.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Areas.Admin.Controllers
{
    [Area(Roles.Admin)]
    [Authorize(Roles = Roles.Admin)]
    public class AuthorController : Controller
    {
        private readonly BookContext _db;

        public AuthorController(BookContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View(_db.Authors.OrderByDescending(a => a.CreatedAt));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Author author)
        {
            if (!ModelState.IsValid) return View(author);

            author.CreatedAt = DateTime.Now;
            author.ModifiedAt = DateTime.Now;

            await _db.Authors.AddAsync(author);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> CreateAsync(string author)
        {
            var result = new
            {
                data = "This author already exists in system.",
                icon = "error"
            };

            if (!await _db.Authors.AnyAsync(a => a.Fullname == author))
            {
                await _db.Authors.AddAsync(new Author
                {
                    Fullname = author,
                    CreatedAt = DateTime.Now,
                    ModifiedAt = DateTime.Now
                });
                await _db.SaveChangesAsync();

                result = new
                {
                    data = "Author successefully added.",
                    icon = "success"
                };
            }

            return Json(result);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var author = await _db.Authors.FindAsync(id);

            if (author == null) return NotFound();

            return View(author);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Author author)
        {
            if (!ModelState.IsValid) return View(author);

            var authorFromDb = await _db.Authors.FindAsync(author.Id);

            authorFromDb.Fullname = author.Fullname;
            authorFromDb.ModifiedAt = DateTime.Now;

            _db.Entry(authorFromDb).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var author = await _db.Authors.FindAsync(id);

            if (author == null) return NotFound();

            return View(author);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var author = await _db.Authors.FindAsync(id);

            if (author == null) return NotFound();

            _db.Entry(author).State = EntityState.Deleted;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> GetAuthorsAsync(int? bookId)
        {
            var model = new BookViewModel
            {
                Authors = _db.Authors,
                AuthorsId = new int[0]
            };

            if (bookId != null)
            {
                var book = await _db.Books.FindAsync(bookId);
                model.AuthorsId = book.Authors.Select(a => a.AuthorId);
            }

            return PartialView("_AuthorsPartial", model);
        }
    }
}