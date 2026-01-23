using BankingVault.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingVault.Controllers
{
    [Authorize("UserAuth")]
    public class TransactionController : Controller
    {
        private readonly DatabaseContext _db;
        public TransactionController(DatabaseContext db)
        {
            this._db = db;
        }
        public IActionResult Index(Guid id)
        {
            TempData["recordId"] = id; //UploadFile
            TempData["Index"] = id; //Delete File
            var transactionRecords=_db.Transactions.Where(trans=>trans.RecordID == id).ToList();
            return View(transactionRecords);
        }

        public IActionResult PrepareRecord(Guid id) { 

            return View();
        }
    }
}
