using BankingVault.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankingVault.Controllers
{
    [Authorize("AdminOnly")]
    public class AdminController : Controller
    {
        private readonly DatabaseContext _db;
        public AdminController(DatabaseContext db) { 
            this._db = db;
        }
        public IActionResult Index()
        {
            var list_users=_db.UserAccounts.AsNoTracking().ToList();
            return View(list_users);
        }
    }
}
