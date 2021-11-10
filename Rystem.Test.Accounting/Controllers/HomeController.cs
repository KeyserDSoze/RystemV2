using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Rystem.Test.Accounting.Models;
using System.Diagnostics;

namespace Rystem.Test.Accounting.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RoleManager<IdentityRole> RoleManager;

        public HomeController(ILogger<HomeController> logger, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            RoleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
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