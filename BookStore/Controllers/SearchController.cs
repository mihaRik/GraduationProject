using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Data;
using BookStore.Models;
using Google.Apis.Books.v1;
using Google.Apis.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BookStore.Controllers
{
    public class SearchController : Controller
    {
        private readonly BookContext _db;
        private readonly IConfiguration _config;

        public SearchController(BookContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public async Task<IActionResult> Books(string q)
        {
            await CheckDbBook(q);

            var books = _db.Books.Where(b => b.Title.Contains(q) ||
                                             b.Subtitle.Contains(q) ||
                                             b.Description.Contains(q) ||
                                             b.Authors.Any(a => a.Author.Fullname.Contains(q)) ||
                                             b.Categories.Any(c => c.Category.Name.Contains(q)))
                                 .OrderByDescending(b => b.CreatedAt)
                                 .Take(5)
                                 .Select(b => new
                                 {
                                     b.Title,
                                     Description = b.Description.Length > 50 ?
                                                   b.Description.Substring(0, 50) + "..." : b.Description,
                                     Category = b.Categories.FirstOrDefault().Category.Name,
                                     Image = b.Images.FirstOrDefault().ImageUrl,
                                     Url = $"/book/details/{b.Id}"
                                 });
            var obj = new
            {
                items = books
            };

            return Json(obj);
        }

        public async Task CheckDbBook(string q)
        {
            var service = new BooksService(new BaseClientService.Initializer
            {
                ApiKey = _config["Data:APIKeys:GoogleBookAPI"]
            });

            var volumes = await service.Volumes.List(q).ExecuteAsync();

            var items = volumes?.Items;

            if (items != null)
            {
                foreach (var item in items)
                {
                    if (_db.Books.Any(b => b.GID == item.Id)) return;

                    var categories = item?.VolumeInfo?.Categories;
                    var authors = item?.VolumeInfo?.Authors;
                    var volume = item?.VolumeInfo;

                    if (categories != null && authors != null && volume != null)
                    {
                        foreach (var author1 in volume.Authors)
                        {
                            if (!_db.Authors.Any(c => c.Fullname == author1))
                            {
                                await _db.Authors.AddAsync(new Author
                                {
                                    Fullname = author1,
                                    CreatedAt = DateTime.Now,
                                    ModifiedAt = DateTime.Now
                                });
                                await _db.SaveChangesAsync();
                            }
                        }

                        foreach (var category1 in volume.Categories)
                        {
                            if (!_db.Categories.Any(c => c.Name == category1))
                            {
                                await _db.Categories.AddAsync(new Category
                                {
                                    Name = category1,
                                    CreatedAt = DateTime.Now,
                                    ModifiedAt = DateTime.Now
                                });
                                await _db.SaveChangesAsync();
                            }
                        }

                        var publishdate = DateTime.Now;

                        try
                        {
                            publishdate = Convert.ToDateTime(volume.PublishedDate);
                        }
                        catch (Exception)
                        {
                            publishdate = DateTime.Now;
                        }

                        var book = new Book
                        {
                            Title = volume.Title,
                            Subtitle = volume.Subtitle,
                            CreatedAt = DateTime.Now,
                            ModifiedAt = DateTime.Now,
                            Description = volume.Description,
                            GID = item.Id,
                            Language = volume.Language,
                            PageCount = volume.PageCount ?? 100,
                            PublishedDate = publishdate,
                            Publisher = volume.Publisher,
                        };
                        await _db.Books.AddAsync(book);
                        await _db.SaveChangesAsync();

                        #region Images

                        var image = new Image
                        {
                            ImageUrl = volume.ImageLinks.ExtraLarge,
                            BookId = book.Id
                        };
                        if (!string.IsNullOrEmpty(image.ImageUrl))
                        {
                            await _db.Images.AddAsync(image);
                            await _db.SaveChangesAsync();
                        }

                        var image1 = new Image
                        {
                            ImageUrl = volume.ImageLinks.Large,
                            BookId = book.Id
                        };
                        if (!string.IsNullOrEmpty(image.ImageUrl))
                        {
                            await _db.Images.AddAsync(image1);
                            await _db.SaveChangesAsync();
                        }

                        var image2 = new Image
                        {
                            ImageUrl = volume.ImageLinks.Medium,
                            BookId = book.Id
                        };
                        if (!string.IsNullOrEmpty(image.ImageUrl))
                        {
                            await _db.Images.AddAsync(image2);
                            await _db.SaveChangesAsync();
                        }

                        var image3 = new Image
                        {
                            ImageUrl = volume.ImageLinks.SmallThumbnail,
                            BookId = book.Id
                        };
                        if (!string.IsNullOrEmpty(image.ImageUrl))
                        {
                            await _db.Images.AddAsync(image3);
                            await _db.SaveChangesAsync();
                        }

                        var image4 = new Image
                        {
                            ImageUrl = volume.ImageLinks.Thumbnail,
                            BookId = book.Id
                        };
                        if (!string.IsNullOrEmpty(image4.ImageUrl))
                        {
                            await _db.Images.AddAsync(image4);
                            await _db.SaveChangesAsync();
                        }

                        #endregion

                        foreach (var category2 in volume.Categories)
                        {
                            var pivot = new BookCategoryPivot
                            {
                                BookId = book.Id,
                                CategoryId = _db.Categories.FirstOrDefault(c => c.Name == category2).Id
                            };
                            await _db.BookCategories.AddAsync(pivot);
                            await _db.SaveChangesAsync();
                        }

                        foreach (var author2 in volume.Authors)
                        {
                            var pivot = new BookAuthorPivot
                            {
                                BookId = book.Id,
                                AuthorId = _db.Authors.FirstOrDefault(a => a.Fullname == author2).Id
                            };
                            await _db.BookAuthors.AddAsync(pivot);
                            await _db.SaveChangesAsync();
                        }
                    }
                }
            }
        }
    }
}