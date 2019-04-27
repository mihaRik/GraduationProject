using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Data;
using BookStore.Extensions;
using BookStore.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace BookStore.Areas.Admin.Controllers
{
    [Area(Roles.Admin)]
    //[Authorize(Roles = Roles.Admin)]
    public class BookController : Controller
    {
        private readonly BookContext _db;
        private readonly IHostingEnvironment _env;

        public BookController(BookContext db, IHostingEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetBooks(int draw, int start, int length, [FromQuery(Name = "search[value]")] string search)
        {
            search = search ?? "";
            var model = new
            {
                draw,
                recordsTotal = _db.Books.Count(),
                recordsFiltered = _db.Books
                                     .Where(b => b.Title.Contains(search) || b.Authors.Any(a => a.Author.Fullname.Contains(search))
                                              || b.Categories.Any(c => c.Category.Name.Contains(search)) || b.Description.Contains(search))
                                     .Count(),
                data = _db.Books
                          .Where(b => b.Title.Contains(search) || b.Authors.Any(a => a.Author.Fullname.Contains(search))
                                    || b.Categories.Any(c => c.Category.Name.Contains(search)) || b.Description.Contains(search))
                          .OrderByDescending(b => b.CreatedAt)
                          .Skip(start)
                          .Take(length)
                          .Select(b => new List<object>
                {
                    b.Title.Length > 20 ? b.Title.Substring(0, 20) + "..." : b.Title,
                    b.Authors.Select(a => new List<object> { a.Author.Fullname }),
                    b.Categories.Select(c => new List<object> { c.Category.Name }),
                    b.Description.Length > 20 ? b.Description.Substring(0, 20) + "..." : b.Description,
                    b.Publisher,
                    b.Language,
                    b.PageCount,
                    $"<img src='{b.Images.Thumbnail ?? "/images/no-image.png"}' class='img-thumbnail' />",
                    $"<a href='/admin/book/edit/{b.Id}' class='btn btn-outline-info'><i class='far fa-edit'></i></a>" +
                    $"<a href='/admin/book/delete/{b.Id}' class='btn btn-outline-danger'><i class='far fa-trash-alt'></i></a>"
                })
            };

            return Json(model);
        }

        public IActionResult Create()
        {
            var model = new BookCreateViewModel
            {
                Categories = _db.Categories,
                Authors = _db.Authors
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookCreateViewModel viewModel, int[] categories, int[] authors)
        {
            if (!ModelState.IsValid) return View(viewModel);

            if (categories.Length == 0 || authors.Length == 0)
            {
                ModelState.AddModelError("", "Minimum one category and author are required.");
                viewModel.Categories = _db.Categories;
                viewModel.Authors = _db.Authors;
                return View(viewModel);
            }

            if (viewModel.Book.Photos.Count > 5)
            {
                ModelState.AddModelError("", "Maximum 5 images can be added.");
                viewModel.Categories = _db.Categories;
                viewModel.Authors = _db.Authors;
                return View(viewModel);
            }

            foreach (var file in viewModel.Book.Photos)
            {
                if (!file.IsPhoto())
                {
                    ModelState.AddModelError("", "Files must be images.");
                    viewModel.Categories = _db.Categories;
                    viewModel.Authors = _db.Authors;
                    return View(viewModel);
                }
            }

            viewModel.Book.CreatedAt = DateTime.Now;
            viewModel.Book.ModifiedAt = DateTime.Now;

            await _db.Books.AddAsync(viewModel.Book);
            await _db.SaveChangesAsync();

            foreach (var catId in categories)
            {
                var bookCategory = new BookCategoryPivot
                {
                    CategoryId = catId,
                    BookId = viewModel.Book.Id
                };
                await _db.BookCategories.AddAsync(bookCategory);
                await _db.SaveChangesAsync();
            }

            foreach (var authorId in authors)
            {
                var bookAuhtor = new BookAuthorPivot
                {
                    AuthorId = authorId,
                    BookId = viewModel.Book.Id
                };
                await _db.BookAuthors.AddAsync(bookAuhtor);
                await _db.SaveChangesAsync();
            }


            var images = new Images();

            try
            {
                images.BookId = viewModel.Book.Id;
                images.Thumbnail = await viewModel.Book.Photos[0]?.SavePhotoAsync(_env.WebRootPath, "books");
                images.SmallThumbnail = await viewModel.Book.Photos[1]?.SavePhotoAsync(_env.WebRootPath, "books");
                images.Meddium = await viewModel.Book.Photos[2]?.SavePhotoAsync(_env.WebRootPath, "books");
                images.Large = await viewModel.Book.Photos[3]?.SavePhotoAsync(_env.WebRootPath, "books");
                images.ExtraLarge = await viewModel.Book.Photos[4]?.SavePhotoAsync(_env.WebRootPath, "books");
            }
            catch (Exception ex)
            {
            }

            await _db.Images.AddAsync(images);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var book = await _db.Books.FindAsync(id);

            if (book == null) return NotFound();

            var model = new BookEditViewModel
            {
                Book = book,
                CategoriesId = book.Categories.Select(b => b.CategoryId),
                AuthorsId = book.Authors.Select(a => a.AuthorId),
                Categories = _db.Categories,
                Authors = _db.Authors
            };

            return View(model);
        }
    }
}