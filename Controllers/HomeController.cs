using System.Diagnostics;
using BankingVault.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankingVault.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DatabaseContext _db;

        public HomeController(ILogger<HomeController> logger, DatabaseContext db)
        {
            _logger = logger;
            _db = db;
        }

        [Authorize("UserAuth")]
        public async Task<IActionResult> Index()
        {
            var userEmail = User.Identity?.Name;
            if (userEmail != null)
            {
                // Get user info
                var user = await _db.UserAccounts.FirstOrDefaultAsync(u => u.EmailAddress == userEmail);
                
                // Get user's accounts
                var accounts = await _db.AccountTypes
                    .AsNoTracking()
                    .Where(a => a.OwnerEmail == user.EmailAddress)
                    .ToListAsync();

                // Pass data to view
                ViewBag.TotalBalance = user?.TotalBalance ?? 0m;
                ViewBag.MonthlyChange = 0m; // You can calculate this based on transactions
                
                return View(accounts);
            }

            return RedirectToAction("Login", "User");
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
