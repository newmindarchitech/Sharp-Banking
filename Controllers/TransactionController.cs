using BankingVault.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

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

        public IActionResult PrepareRecord() {
            var accountRecordID = TempData["recordId"];
            var fill = _db.AccountTypes.FirstOrDefault(contx => contx.AccountTransactionRecordID == Guid.Parse(accountRecordID.ToString()));
            var fill_form = new TransactionForm
            {
                RecordID = fill.AccountTransactionRecordID,
            };
            return View(fill_form);
        }

        [HttpPost]
        public async Task<IActionResult> PrepareRecord(TransactionForm model)
        {
            var accountContextID = TempData["Index"];
            var recordToWrite = _db.AccountTypes.FirstOrDefaultAsync(rec => rec.AccountTransactionRecordID == Guid.Parse(accountContextID.ToString()));
            if (recordToWrite != null) {
                switch (model.TransactionContext)
                {
                    case TransactionType.Transfer:
                        if(recordToWrite.Result.Balance > model.TransactionAmount)
                        {
                            try
                            {
                                var record = new Transaction
                                {
                                    TransactionID = Guid.NewGuid(),
                                    TransactionAmount = model.TransactionAmount,
                                    RecordID = recordToWrite.Result.AccountTransactionRecordID,
                                    TransactionContext = TransactionType.Transfer,
                                    CreatedDate = DateTime.Now
                                };
                                recordToWrite.Result.Balance-=record.TransactionAmount;
                                _db.Transactions.Add(record);
                                await _db.SaveChangesAsync();
                                ModelState.Clear();
                                return RedirectToAction("Index", "Transaction", new {id=Guid.Parse(accountContextID.ToString())});
                            }catch(DbException e)
                            {
                                e.ToString();
                            }
                        }
                        else
                        {
                            ModelState.Clear();
                            ModelState.AddModelError("", "Current account Balance is not enough for this transaction");
                            var fill_form = new TransactionForm
                            {
                                RecordID=recordToWrite.Result.AccountTransactionRecordID,
                            };
                            return View(fill_form);
                        }
                        break;
                    case TransactionType.Deposit:
                        if (recordToWrite.Result.Balance > model.TransactionAmount)
                        {
                            try
                            {
                                var record = new Transaction
                                {
                                    TransactionID = Guid.NewGuid(),
                                    TransactionAmount = model.TransactionAmount,
                                    RecordID = recordToWrite.Result.AccountTransactionRecordID,
                                    TransactionContext = TransactionType.Deposit,
                                    CreatedDate = DateTime.Now
                                };
                                recordToWrite.Result.Balance -= record.TransactionAmount;
                                _db.Transactions.Add(record);
                                await _db.SaveChangesAsync();
                                ModelState.Clear();
                                return RedirectToAction("Index", "Transaction", new { id = Guid.Parse(accountContextID.ToString()) });
                            }
                            catch (DbException e)
                            {
                                e.ToString();
                            }
                        }
                        else
                        {
                            ModelState.Clear();
                            ModelState.AddModelError("", "Current account Balance is not enough for this transaction");
                            var fill_form = new TransactionForm
                            {
                                RecordID = recordToWrite.Result.AccountTransactionRecordID,
                            };
                            return View(fill_form);
                        }
                        break;
                    case TransactionType.Withdrawal:
                        if (recordToWrite.Result.Balance > model.TransactionAmount)
                        {
                            try
                            {
                                var record = new Transaction
                                {
                                    TransactionID = Guid.NewGuid(),
                                    TransactionAmount = model.TransactionAmount,
                                    RecordID = recordToWrite.Result.AccountTransactionRecordID,
                                    TransactionContext = TransactionType.Withdrawal,
                                    CreatedDate = DateTime.Now
                                };
                                recordToWrite.Result.Balance -= record.TransactionAmount;
                                _db.Transactions.Add(record);
                                await _db.SaveChangesAsync();
                                ModelState.Clear();
                                return RedirectToAction("Index", "Transaction", new { id = Guid.Parse(accountContextID.ToString()) });
                            }
                            catch (DbException e)
                            {
                                e.ToString();
                            }
                        }
                        else
                        {
                            ModelState.Clear();
                            ModelState.AddModelError("", "Current account Balance is not enough for this transaction");
                            var fill_form = new TransactionForm
                            {
                                RecordID = recordToWrite.Result.AccountTransactionRecordID,
                            };
                            return View(fill_form);
                        }
                        break;
                }
            }
            return View();
        }
    }
}
