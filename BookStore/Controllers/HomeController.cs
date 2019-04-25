﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BookStore.Models;
using Google.Apis.Books.v1;
using Google.Apis.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using BookStore.Data;

namespace BookStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;
        private readonly BookContext _db;

        public HomeController(IConfiguration config, BookContext db)
        {
            _config = config;
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Book(string q)
        {
            var service = new BooksService(new BaseClientService.Initializer
            {
                ApplicationName = "mihaRik's book store",
                ApiKey = _config["Data:APIKeys:GoogleBookAPI"],
            });

            var volumes = await service.Volumes.List(q).ExecuteAsync();

            return PartialView("_Test", volumes.Items);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
