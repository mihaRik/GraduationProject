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
    public class APIController : Controller
    {
        private readonly IConfiguration _config;
        private readonly BookContext _db;

        public APIController(IConfiguration config, BookContext db)
        {
            _config = config;
            _db = db;
        }

        public async Task<IActionResult> Categories()
        {
            var service = new BooksService(new BaseClientService.Initializer
            {
                ApiKey = _config["Data:APIKeys:GoogleBookAPI"]
            });
            var a = "qwertyuiopasdfghjklzxcvbnm";
            for (int i = 0; i < 100; i++)
            {
                var volumes = await service.Volumes.List(i.ToString()).ExecuteAsync();

                if (volumes.Items != null)
                {
                    foreach (var item in volumes.Items)
                    {
                        var cats = item?.VolumeInfo?.Categories;

                        if (cats != null)
                        {
                            foreach (var cat in cats)
                            {
                                if (!_db.Categories.Any(c => c.Name == cat))
                                {
                                    await _db.Categories.AddAsync(new Category
                                    {
                                        Name = cat,
                                        CreatedAt = DateTime.Now,
                                        ModifiedAt = DateTime.Now
                                    });
                                    await _db.SaveChangesAsync();
                                }
                            }
                        }
                    }
                }
            }

            return Content("ok");
        }

        public async Task<IActionResult> Authors()
        {
            var service = new BooksService(new BaseClientService.Initializer
            {
                ApiKey = _config["Data:APIKeys:GoogleBookAPI"]
            });

            var a = "qwertyuiopasdfghjklzxcvbnm";

            for (int i = 0; i < 100; i++)
            {
                var volumes = await service.Volumes.List(i.ToString()).ExecuteAsync();

                var items = volumes?.Items;

                if (items != null)
                {
                    foreach (var item in items)
                    {
                        var authors = item?.VolumeInfo?.Authors;

                        if (authors != null)
                        {
                            foreach (var author in authors)
                            {
                                if (!_db.Authors.Any(x => x.Fullname == author))
                                {
                                    await _db.Authors.AddAsync(new Author
                                    {
                                        Fullname = author,
                                        CreatedAt = DateTime.Now,
                                        ModifiedAt = DateTime.Now
                                    });
                                    await _db.SaveChangesAsync();
                                }
                            }
                        }
                    }
                }
            }

            return Content("ok");
        }

        public async Task<IActionResult> Books()
        {
            var service = new BooksService(new BaseClientService.Initializer
            {
                ApiKey = _config["Data:APIKeys:GoogleBookAPI"]
            });

            for (int i = 0; i < 100; i++)
            {
                var volumes = await service.Volumes.List(i.ToString()).ExecuteAsync();

                var items = volumes?.Items;

                if (items != null)
                {
                    foreach (var item in items)
                    {
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
                            catch (Exception ex)
                            {
                                publishdate = DateTime.Now;
                            }

                            var images = new Images
                            {
                                ExtraLarge = volume.ImageLinks.ExtraLarge,
                                Large = volume.ImageLinks.Large,
                                Meddium = volume.ImageLinks.Medium,
                                SmallThumbnail = volume.ImageLinks.SmallThumbnail,
                                Thumbnail = volume.ImageLinks.Thumbnail
                            };

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
                                Images = images
                            };
                            await _db.Books.AddAsync(book);
                            await _db.SaveChangesAsync();

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

            return Content("ok");
        }
    }
}