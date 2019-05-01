using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Data;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class AuthorController : Controller
    {
        private readonly BookContext _db;

        public AuthorController(BookContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var author = await _db.Authors.FindAsync(id);

            if (author == null) return NotFound();

            return View(author);
        }
    }
}