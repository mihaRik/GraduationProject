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
using Microsoft.EntityFrameworkCore;
using static BookStore.Utility.DeleteFile;

namespace BookStore.Areas.Admin.Controllers
{
    [Area(Roles.Admin)]
    [Authorize(Roles = Roles.Admin)]
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
                    $"<img src='{b.Images.ToList()[0].ImageUrl ?? "/images/no-image.png"}' class='img-thumbnail' />",
                    $"<a href='/admin/book/edit/{b.Id}' class='btn btn-outline-info'><i class='far fa-edit'></i></a>" +
                    $"<a href='javascript:deleteBook({b.Id})' class='btn btn-outline-danger delete'><i class='far fa-trash-alt'></i></a>"
                })
            };

            return Json(model);
        }

        public IActionResult Create()
        {
            var model = new BookViewModel
            {
                Categories = _db.Categories,
                Authors = _db.Authors,
                CategoriesId = new int[0],
                AuthorsId = new int[0]
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookViewModel viewModel, int[] categories, int[] authors)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Categories = _db.Categories;
                viewModel.Authors = _db.Authors;
                viewModel.CategoriesId = categories;
                viewModel.AuthorsId = authors;
                return View(viewModel);
            }

            if (categories.Length == 0 || authors.Length == 0)
            {
                ModelState.AddModelError("", "Minimum one category and author are required.");
                viewModel.Categories = _db.Categories;
                viewModel.Authors = _db.Authors;
                viewModel.CategoriesId = categories;
                viewModel.AuthorsId = authors;
                return View(viewModel);
            }

            if (viewModel.Book.Photos.Count > 5)
            {
                ModelState.AddModelError("", "Maximum 5 images can be added.");
                viewModel.Categories = _db.Categories;
                viewModel.Authors = _db.Authors;
                viewModel.CategoriesId = categories;
                viewModel.AuthorsId = authors;
                return View(viewModel);
            }

            foreach (var file in viewModel.Book.Photos)
            {
                if (!file.IsPhoto())
                {
                    ModelState.AddModelError("", "Files must be images.");
                    viewModel.Categories = _db.Categories;
                    viewModel.Authors = _db.Authors;
                    viewModel.CategoriesId = categories;
                    viewModel.AuthorsId = authors;
                    return View(viewModel);
                }
            }

            viewModel.Book.CreatedAt = DateTime.Now;
            viewModel.Book.ModifiedAt = DateTime.Now;

            await _db.Books.AddAsync(viewModel.Book);
            await _db.SaveChangesAsync();

            foreach (var img in viewModel.Book.Photos)
            {
                var image = new Image
                {
                    BookId = viewModel.Book.Id,
                    ImageUrl = await img.SavePhotoAsync(_env.WebRootPath, "books")
                };

                await _db.Images.AddAsync(image);
                await _db.SaveChangesAsync();
            }

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

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var book = await _db.Books.FindAsync(id);

            if (book == null) return NotFound();

            var model = new BookViewModel
            {
                Book = book,
                CategoriesId = book.Categories.Select(b => b.CategoryId),
                AuthorsId = book.Authors.Select(a => a.AuthorId),
                Categories = _db.Categories,
                Authors = _db.Authors
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BookViewModel viewModel, int[] categories, int[] authors)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Categories = _db.Categories;
                viewModel.Authors = _db.Authors;
                viewModel.CategoriesId = categories;
                viewModel.AuthorsId = authors;
                return View(viewModel);
            }

            if (categories.Length == 0 || authors.Length == 0)
            {
                ModelState.AddModelError("", "Minimum one category and author are required.");
                viewModel.Categories = _db.Categories;
                viewModel.Authors = _db.Authors;
                viewModel.CategoriesId = categories;
                viewModel.AuthorsId = authors;
                return View(viewModel);
            }

            var bookFromDb = await _db.Books.FindAsync(viewModel.Book.Id);

            if (viewModel.Book.Photos != null)
            {
                if (viewModel.Book.Photos.Count + bookFromDb.Images.Count > 5)
                {
                    ModelState.AddModelError("", "Maximum 5 images can be in one book.");
                    viewModel.Categories = _db.Categories;
                    viewModel.Authors = _db.Authors;
                    viewModel.CategoriesId = categories;
                    viewModel.AuthorsId = authors;
                    viewModel.Book.Images = bookFromDb.Images;
                    return View(viewModel);
                }

                foreach (var file in viewModel.Book.Photos)
                {
                    if (!file.IsPhoto())
                    {
                        ModelState.AddModelError("", "Files must be images.");
                        viewModel.Categories = _db.Categories;
                        viewModel.Authors = _db.Authors;
                        viewModel.CategoriesId = categories;
                        viewModel.AuthorsId = authors;
                        return View(viewModel);
                    }
                }

                foreach (var img in viewModel.Book.Photos)
                {
                    var image = new Image
                    {
                        BookId = viewModel.Book.Id,
                        ImageUrl = await img.SavePhotoAsync(_env.WebRootPath, "books")
                    };

                    await _db.Images.AddAsync(image);
                    await _db.SaveChangesAsync();
                }
            }


            foreach (var cat in bookFromDb.Categories.ToList().Where(c => !categories.Any(ct => ct == c.CategoryId)))
            {
                var pivot = await _db.BookCategories.FindAsync(cat.Id);
                _db.Entry(pivot).State = EntityState.Deleted;
                await _db.SaveChangesAsync();
            }

            foreach (var author in bookFromDb.Authors.ToList().Where(a => !authors.Any(au => au == a.AuthorId)))
            {
                var pivot = await _db.BookAuthors.FindAsync(author.Id);
                _db.Entry(pivot).State = EntityState.Deleted;
                await _db.SaveChangesAsync();
            }

            foreach (var catId in categories.Where(c => !bookFromDb.Categories.Any(bc => bc.CategoryId == c)))
            {
                var bookCategory = new BookCategoryPivot
                {
                    CategoryId = catId,
                    BookId = viewModel.Book.Id
                };
                await _db.BookCategories.AddAsync(bookCategory);
                await _db.SaveChangesAsync();
            }

            foreach (var authorId in authors.Where(a => !bookFromDb.Authors.Any(ab => ab.AuthorId == a)))
            {
                var bookAuhtor = new BookAuthorPivot
                {
                    AuthorId = authorId,
                    BookId = viewModel.Book.Id
                };
                await _db.BookAuthors.AddAsync(bookAuhtor);
                await _db.SaveChangesAsync();
            }

            viewModel.Book.CreatedAt = bookFromDb.CreatedAt;
            viewModel.Book.ModifiedAt = DateTime.Now;
            _db.Entry(bookFromDb).State = EntityState.Detached;
            _db.Entry(viewModel.Book).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteImage(int? imageId)
        {
            var result = new
            {
                data = "Something goes wrong.",
                icon = "error"
            };

            if (imageId == null) return Json(result);

            var image = await _db.Images.FindAsync(imageId);

            if (image == null) return Json(result);

            Delete(_env.WebRootPath + @"\" + image.ImageUrl);

            _db.Entry(image).State = EntityState.Deleted;
            await _db.SaveChangesAsync();

            result = new
            {
                data = "Image was successfully deleted.",
                icon = "success"
            };

            return Json(result);
        }

        [ActionName("Delete")]
        public async Task<IActionResult> DeleteBook(int bookId)
        {
            var res = new
            {
                code = 404,
                title = "Something goes wrong.",
                data = "Please try again later.",
                icon = "error"
            };

            var book = await _db.Books.FindAsync(bookId);

            if (book == null) return Json(res);

            _db.Entry(book).State = EntityState.Deleted;
            await _db.SaveChangesAsync();

            res = new
            {
                code = 200,
                title = $"{book.Title}",
                data = "Book was deleted successfully",
                icon = "success"
            };

            return Json(res);
        }
    }
}