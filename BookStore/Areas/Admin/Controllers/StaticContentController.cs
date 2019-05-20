using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStore.Extensions;
using static BookStore.Utility.DeleteFile;
using BookStore.Models.ViewModels;
using System.IO;

namespace BookStore.Areas.Admin.Controllers
{
    [Area(Roles.Admin)]
    [Authorize(Roles = Roles.Admin)]
    public class StaticContentController : Controller
    {
        private readonly BookContext _db;
        private readonly IHostingEnvironment _env;

        public StaticContentController(BookContext db, IHostingEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public async Task<IActionResult> About()
        {
            return View(await _db.Abouts.FirstOrDefaultAsync());
        }

        public async Task<IActionResult> AboutEdit()
        {
            var about = await _db.Abouts.FirstOrDefaultAsync();

            return View(about);
        }

        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> AboutEdit(About about)
        {
            if (!ModelState.IsValid) return NotFound();

            var aboutFromDb = await _db.Abouts.FirstOrDefaultAsync();

            if (about.Photo != null)
            {
                if (!about.Photo.IsPhoto())
                {
                    ModelState.AddModelError("Photo", "File must be image type.");
                    return View(aboutFromDb);
                }

                Delete(Path.Combine(_env.WebRootPath, aboutFromDb.Image));

                aboutFromDb.Image = await about.Photo.SavePhotoAsync(_env.WebRootPath, "about");
            }

            aboutFromDb.Title = about.Title;
            aboutFromDb.Description = about.Description;

            _db.Entry(aboutFromDb).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(About));
        }

        public async Task<IActionResult> Banner()
        {
            return View(await _db.Banners.FirstOrDefaultAsync());
        }

        public async Task<IActionResult> BannerEdit()
        {
            var banner = await _db.Banners.FirstOrDefaultAsync();

            return View(banner);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> BannerEdit(Banner banner)
        {
            if (!ModelState.IsValid) return NotFound();

            var bannerFormDb = await _db.Banners.FirstOrDefaultAsync();

            if(banner.Photo != null)
            {
                if (!banner.Photo.IsPhoto())
                {
                    ModelState.AddModelError("Photo", "File must be image type.");
                    return View(bannerFormDb);
                }

                Delete(Path.Combine(_env.WebRootPath, bannerFormDb.Image));

                bannerFormDb.Image = await banner.Photo.SavePhotoAsync(_env.WebRootPath, "banner");
                bannerFormDb.Image = bannerFormDb.Image.Replace(Convert.ToChar(@"\"), Convert.ToChar("/"));
            }

            bannerFormDb.Title = banner.Title;
            _db.Entry(bannerFormDb).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Banner));
        }
    }
}