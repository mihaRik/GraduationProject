using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BookStore.Areas.Admin.Controllers
{
    [Area(Roles.Admin)]
    [Authorize(Roles = Roles.Admin)]
    public class CategoryController : Controller
    {
        private readonly BookContext _db;

        public CategoryController(BookContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View(_db.Categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid) return View(category);

            category.CreatedAt = DateTime.Now;
            category.ModifiedAt = DateTime.Now;

            await _db.Categories.AddAsync(category);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var cat = await _db.Categories.FindAsync(id);

            if (cat == null) return NotFound();

            return View(cat);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (!ModelState.IsValid) return View(category);

            var categoryFromDb = await _db.Categories.FindAsync(category.Id);

            categoryFromDb.Name = category.Name;
            categoryFromDb.ModifiedAt = DateTime.Now;
            _db.Entry(categoryFromDb).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var cat = await _db.Categories.FindAsync(id);

            if (cat == null) return NotFound();

            return View(cat);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var cat = await _db.Categories.FindAsync(id);

            if (cat == null) return NotFound();

            _db.Entry(cat).State = EntityState.Deleted;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}