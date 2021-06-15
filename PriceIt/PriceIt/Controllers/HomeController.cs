﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PriceIt.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using PriceIt.Core.Interfaces;
using PriceIt.Core.Models;
using PriceIt.Core.Services;
using PriceIt.Data.Interfaces;

namespace PriceIt.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebScraping _webScrapingService;
        private readonly IProductsRepository _productsRepository;

        public HomeController(ILogger<HomeController> logger, IWebScraping webScrapingService,IProductsRepository productsRepository)
        {
            _logger = logger;
            _webScrapingService = webScrapingService;
            _productsRepository = productsRepository;
        }

        public async Task<ActionResult> Index()
        {
            //ViewData["test"] = await _webScrapingService.GetAmazonProducts();

            ViewData["test"] = await _productsRepository.GetProducts();

            return View();
        }

        public IActionResult Privacy()
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
