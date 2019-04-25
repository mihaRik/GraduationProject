using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Data;
using BookStore.Models;
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
            return View(_db.Authors);
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
    }
}