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
            TempData["recordId"] = id; //FillForm
            TempData["Index"] = id; //Index
            var transactionRecords=_db.Transactions.AsNoTracking().Where(trans=>trans.RecordID == id).ToList();
            return View(transactionRecords);
        }

        public IActionResult PrepareRecord() {
            var accountRecordID = TempData["recordId"];
            var fill = _db.AccountTypes.FirstOrDefault(contx => contx.AccountTransactionRecordID == Guid.Parse(accountRecordID.ToString()));
            if (fill.WithDrawalLimits != 0)
            {
                var fill_form = new TransactionForm
                {
                    RecordID = fill.AccountTransactionRecordID,
                };
                return View(fill_form);
            }
            else
            {
                var fill_form = new TransactionForm
                {
                    RecordID = fill.AccountTransactionRecordID,
                };
                ModelState.AddModelError("", $"this transaction will be charged by {fill.PenaltyFees}%");
                return View(fill_form);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PrepareRecord(TransactionForm model)
        {
            var accountContextID = TempData["Index"];
            var recordToWrite =await _db.AccountTypes.FirstOrDefaultAsync(rec => rec.AccountTransactionRecordID == Guid.Parse(accountContextID.ToString()));
            var origin_account = await _db.UserAccounts.FirstOrDefaultAsync(acc => acc.EmailAddress == recordToWrite.OwnerEmail);
            if (recordToWrite != null) {
               
                    switch (model.TransactionContext)
                    {
                        case TransactionType.Transfer:
                            if (recordToWrite.Balance > model.TransactionAmount)
                            {
                                try
                                {
                                    if (model.RecepientAccountID != null)
                                    {
                                        var transferTarget = await _db.UserAccounts.FirstOrDefaultAsync(target => target.Id == model.RecepientAccountID);
                                        if (transferTarget == null)
                                        {
                                            ModelState.Clear();
                                            ModelState.AddModelError("", "This receipient doesn't exist");
                                            TempData["Index"] = recordToWrite.AccountTransactionRecordID;
                                            var refreshform = new TransactionForm
                                            {
                                                RecordID = recordToWrite.AccountTransactionRecordID,
                                            };
                                            return View(refreshform);
                                        }
                                        else if (recordToWrite.Context == AccountContext.Saving || recordToWrite.Context == AccountContext.MoneyMarket || recordToWrite.Context == AccountContext.CertificateOfDeposit)
                                        {

                                            var record = new Transaction
                                            {
                                                TransactionID = Guid.CreateVersion7(),
                                                TransactionAmount = model.TransactionAmount,
                                                RecordID = recordToWrite.AccountTransactionRecordID,
                                                TransactionContext = TransactionType.Transfer,
                                                CreatedDate = DateTime.Now
                                            };
                                            if (recordToWrite.WithDrawalLimits == 0)
                                            {
                                                recordToWrite.Balance -= record.TransactionAmount + (record.TransactionAmount * ((decimal)recordToWrite.PenaltyFees)) / 100;
                                                transferTarget.TotalBalance += model.TransactionAmount;
                                                _db.Transactions.Add(record);
                                                await _db.SaveChangesAsync();
                                                ModelState.Clear();
                                                return RedirectToAction("Index", "Transaction", new { id = Guid.Parse(accountContextID.ToString()) });
                                            }
                                            if (recordToWrite.WithDrawalLimits > 0)
                                            {
                                                recordToWrite.Balance -= record.TransactionAmount;
                                                transferTarget.TotalBalance += model.TransactionAmount;
                                                recordToWrite.WithDrawalLimits--;
                                                _db.Transactions.Add(record);
                                                await _db.SaveChangesAsync();
                                                ModelState.Clear();
                                                return RedirectToAction("Index", "Transaction", new { id = Guid.Parse(accountContextID.ToString()) });
                                            }
                                        }
                                        else
                                        {
                                            var record = new Transaction
                                            {
                                                TransactionID = Guid.CreateVersion7(),
                                                TransactionAmount = model.TransactionAmount,
                                                RecordID = recordToWrite.AccountTransactionRecordID,
                                                TransactionContext = TransactionType.Transfer,
                                                CreatedDate = DateTime.Now
                                            };
                                            recordToWrite.Balance -= record.TransactionAmount;
                                            transferTarget.TotalBalance += model.TransactionAmount;
                                            _db.Transactions.Add(record);
                                            await _db.SaveChangesAsync();
                                            ModelState.Clear();
                                            return RedirectToAction("Index", "Transaction", new { id = Guid.Parse(accountContextID.ToString()) });
                                        }
                                    }
                                    else
                                    {
                                        ModelState.Clear();
                                        ModelState.AddModelError("", "An existing accountID is required to finish this type of transaction");
                                        TempData["Index"] = recordToWrite.AccountTransactionRecordID;
                                        var refreshform = new TransactionForm
                                        {
                                            RecordID = recordToWrite.AccountTransactionRecordID,
                                        };
                                        return View(refreshform);
                                    }
                                }
                                catch (DbException e)
                                {
                                    Console.WriteLine(e.ToString());
                                }
                            }
                            else
                            {
                                ModelState.Clear();
                                ModelState.AddModelError("", "Your account balance is below the transaction amount");
                                return RedirectToAction("UserAccountBalance", "BankAccount", new { id = accountContextID.ToString() });
                            }
                            break;
                        case TransactionType.Deposit:
                            if (origin_account.TotalBalance > model.TransactionAmount)
                            {
                                try
                                {
                                    var record = new Transaction
                                    {
                                        TransactionID = Guid.CreateVersion7(),
                                        TransactionAmount = model.TransactionAmount,
                                        RecordID = recordToWrite.AccountTransactionRecordID,
                                        TransactionContext = TransactionType.Deposit,
                                        CreatedDate = DateTime.Now
                                    };
                                    recordToWrite.Balance += record.TransactionAmount;
                                    origin_account.TotalBalance -= model.TransactionAmount;
                                    _db.Transactions.Add(record);
                                    await _db.SaveChangesAsync();
                                    ModelState.Clear();
                                    return RedirectToAction("Index", "Transaction", new { id = accountContextID.ToString() });
                                }
                                catch (DbException e)
                                {
                                    Console.WriteLine(e.ToString());
                                }
                            }
                            else
                            {
                                ModelState.Clear();
                                ModelState.AddModelError("", "Your account balance is below the transaction amount");
                                return RedirectToAction("UserAccountBalance", "BankAccount", new { id = accountContextID.ToString() });
                            }
                            break;
                        case TransactionType.Withdrawal:
                            if (recordToWrite.Balance > model.TransactionAmount)
                            {
                                if (recordToWrite.Context == AccountContext.Checking)
                                {
                                    try
                                    {
                                        var record = new Transaction
                                        {
                                            TransactionID = Guid.CreateVersion7(),
                                            TransactionAmount = model.TransactionAmount,
                                            RecordID = recordToWrite.AccountTransactionRecordID,
                                            TransactionContext = TransactionType.Withdrawal,
                                            CreatedDate = DateTime.Now
                                        };
                                        recordToWrite.Balance -= record.TransactionAmount;
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
                                else if (recordToWrite.Context == AccountContext.Saving || recordToWrite.Context == AccountContext.MoneyMarket || recordToWrite.Context == AccountContext.CertificateOfDeposit)
                                {
                                    try
                                    {
                                        var record = new Transaction
                                        {
                                            TransactionID = Guid.CreateVersion7(),
                                            TransactionAmount = model.TransactionAmount,
                                            RecordID = recordToWrite.AccountTransactionRecordID,
                                            TransactionContext = TransactionType.Withdrawal,
                                            CreatedDate = DateTime.Now
                                        };
                                        if (recordToWrite.WithDrawalLimits == 0)
                                        {
                                            recordToWrite.Balance -= record.TransactionAmount + (record.TransactionAmount * ((decimal)recordToWrite.PenaltyFees)) / 100;
                                            _db.Transactions.Add(record);
                                            await _db.SaveChangesAsync();
                                            ModelState.Clear();
                                            return RedirectToAction("Index", "Transaction", new { id = Guid.Parse(accountContextID.ToString()) });
                                        }
                                        if (recordToWrite.WithDrawalLimits > 0)
                                        {
                                            recordToWrite.Balance -= record.TransactionAmount;
                                            recordToWrite.WithDrawalLimits--;
                                            _db.Transactions.Add(record);
                                            await _db.SaveChangesAsync();
                                            ModelState.Clear();
                                            return RedirectToAction("Index", "Transaction", new { id = Guid.Parse(accountContextID.ToString()) });
                                        }

                                    }
                                    catch (DbException e)
                                    {
                                        e.ToString();
                                    }
                                }
                            }
                            else
                            {
                                ModelState.Clear();
                                ModelState.AddModelError("", "Your account balance is below the transaction amount");
                                return RedirectToAction("UserAccountBalance", "BankAccount", new { id = accountContextID.ToString() });
                            }
                            break;
                    } 
            }
            return View();
        }

        public IActionResult Redirect()
        {
            var transaction_redirect =TempData["Index"];
            return RedirectToAction("Index", "Transaction", new {id=transaction_redirect});
        }
    }
}
