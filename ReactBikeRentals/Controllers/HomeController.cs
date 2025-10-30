using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using ReactBikes.Models;
using System;
using System.Diagnostics;
using System.IO;
using static System.Net.WebRequestMethods;

using Microsoft.AspNetCore.Hosting;

namespace ReactBikes.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("index","Bikes");
            else
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