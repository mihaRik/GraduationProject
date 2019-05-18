using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Data;
using BookStore.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class CategoryController : Controller
    {
        private readonly BookContext _db;

        public CategoryController(BookContext db)
        {
            _db = db;
        }
        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var category = await _db.Categories.FindAsync(id);

            if (category == null) return NotFound();

            return View(category);
        }
    }
}