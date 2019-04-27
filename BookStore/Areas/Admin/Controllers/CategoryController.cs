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
            return View(_db.Categories.OrderByDescending(c => c.CreatedAt));
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

        public async Task<IActionResult> CreateAsync(string category)
        {
            var result = new
            {
                data = "This category already exists.",
                icon = "error"
            };

            if (!await _db.Categories.AnyAsync(c => c.Name == category))
            {
                await _db.Categories.AddAsync(new Category
                {
                    Name = category,
                    CreatedAt = DateTime.Now,
                    ModifiedAt = DateTime.Now
                });
                await _db.SaveChangesAsync();

                result = new
                {
                    data = "Category successfully added!",
                    icon = "success"
                };
            }

            return Json(result);
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

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var cat = await _db.Categories.FindAsync(id);

            if (cat == null) return NotFound();

            _db.Entry(cat).State = EntityState.Deleted;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> GetCategoriesAsync(int? bookId)
        {
            if (bookId == null)
            {
                return PartialView("_CategoriesCreatePartial", _db.Categories);
            }
            else
            {
                var book = await _db.Books.FindAsync(bookId);
                var model = new BookEditViewModel
                {
                    Categories = _db.Categories,
                    CategoriesId = book.Categories.Select(c => c.CategoryId)
                };

                return PartialView("_CategoriesEditPartial", model);
            }
        }
    }
}