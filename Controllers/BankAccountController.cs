using BankingVault.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace BankingVault.Controllers
{
    [Authorize("UserAuth")]
    public class BankAccountController : Controller
    {
        private readonly DatabaseContext _db;
        public BankAccountController(DatabaseContext db)
        {
            this._db = db;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var current_user=_db.UserAccounts.FirstOrDefaultAsync(account=>account.EmailAddress==User.Identity.Name);
            var list_owner_Accounts=_db.AccountTypes.AsNoTracking().Where(account=>account.OwnerEmail==current_user.Result.EmailAddress).ToList();
            return View(list_owner_Accounts);
        }
        public IActionResult UserAccountBalance(string id)
        {
            var user_id=Guid.Parse(id);
            var fill = _db.UserAccounts.FindAsync(user_id);

            var fill_form_ID = new AccountBalanceForm
            {
               UserID = fill.Result.Id,
               AccountBalance=fill.Result.TotalBalance
            };
            return View(fill_form_ID);
        }
        [HttpPost]
        public async Task<IActionResult> UserAccountBalance(string id,AccountBalanceForm model)
        {
            var user_id=Guid.Parse(id);
            var update_user=await _db.UserAccounts.FindAsync(user_id);
            if (update_user != null) {
                if (update_user.TotalBalance > 0)
                {
                    update_user.TotalBalance += model.DepositAmount;
                    update_user.UpdatedDate = DateTime.Now;
                    await _db.SaveChangesAsync();
                    ModelState.Clear();
                    return RedirectToAction("Index", "BankAccount");
                }
                else
                {
                    update_user.TotalBalance = model.DepositAmount;
                    await _db.SaveChangesAsync();
                    ModelState.Clear();
                    return RedirectToAction("Index", "BankAccount");
                }
            }
            return View(model);
        }
        public IActionResult CreateAccount(string id)
        {
            var user_id=Guid.Parse(id);
            var fill= _db.UserAccounts.Find(user_id);
            if (fill == null)
            {
                return RedirectToAction("Index", "BankAccount");
            }
            var fill_Form_OwnerID = new BankAccountCreationForm
            {
                TotalBalance = fill.TotalBalance,
                OwnerID = fill.Id
            };
            return View(fill_Form_OwnerID);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAccount(string id,BankAccountCreationForm model)
        {
            decimal insertInterestRate;
            int insertWithDrawalLimits;
            decimal DepositeFee;
            decimal PenaltyFees;
            DateTime DeductionDate=DateTime.Today.AddMonths(1);
            var user_Balance= await _db.UserAccounts.FindAsync(Guid.Parse(id));
            var new_Account_Record = new TransactionRecord
            {
                AccountRecordID = Guid.NewGuid(),
                AccountContextID = Guid.NewGuid(),
                OwnerRecordID=user_Balance.RecordID
            };
            _db.TransactionsRecords.Add(new_Account_Record);
            switch (model.AccountContext)
            {
                case AccountContext.Checking:
                    if (user_Balance.TotalBalance > model.DepositAmount)
                    {
                        insertWithDrawalLimits = 0;
                        insertInterestRate = 0;
                        DepositeFee = 0;
                        PenaltyFees = 0;
                        var new_account_type = new AccountType
                        {
                            AccountID = new_Account_Record.AccountContextID,
                            OwnerEmail = user_Balance.EmailAddress,
                            Balance = model.DepositAmount,
                            CreatedDate = DateTime.Now,
                            DeductionDate = DeductionDate,
                            Context = AccountContext.Checking,
                            WithDrawalLimits = insertWithDrawalLimits,
                            InterestRate = insertInterestRate,
                            DepositFee = DepositeFee,
                            PenaltyFees = PenaltyFees,
                            AccountTransactionRecordID = new_Account_Record.AccountRecordID,
                        };
                        user_Balance.TotalBalance -= model.DepositAmount;
                        _db.AccountTypes.Add(new_account_type);
                        await _db.SaveChangesAsync();
                        ModelState.Clear();
                        return RedirectToAction("Index", "BankAccount",new {id=id});
                    }
                    else
                    {
                        return RedirectToAction("UserAccountBalance", "BankAccount", new {id=id});
                    }
                case AccountContext.Saving:
                    if (user_Balance.TotalBalance > model.DepositAmount)
                    {
                        insertWithDrawalLimits = 6;
                        insertInterestRate = 2.0m;
                        DepositeFee = 250000;
                        if (model.DepositAmount > DepositeFee)
                        {
                            var new_account_type_2 = new AccountType
                            {
                                AccountID = new_Account_Record.AccountContextID,
                                OwnerEmail = user_Balance.EmailAddress,
                                Balance = model.DepositAmount,
                                CreatedDate = DateTime.Now,
                                DeductionDate = DeductionDate,
                                Context = AccountContext.Saving,
                                WithDrawalLimits = insertWithDrawalLimits,
                                InterestRate = insertInterestRate,
                                DepositFee = DepositeFee,
                                AccountTransactionRecordID = new_Account_Record.AccountRecordID,
                            };
                            user_Balance.TotalBalance -= model.DepositAmount;
                            _db.AccountTypes.Add(new_account_type_2);
                            await _db.SaveChangesAsync();
                            ModelState.Clear();
                            return RedirectToAction("Index", "BankAccount", new { id = id });
                        }
                        else
                        {
                            ModelState.Clear();
                            ModelState.AddModelError("", "Desposit Amount Below Requirement: 250000");
                            var fill_Form_OwnerID = new BankAccountCreationForm
                            {
                                TotalBalance = user_Balance.TotalBalance,
                                OwnerID = user_Balance.Id,
                            };
                            return View(fill_Form_OwnerID);
                        }
                    }
                    else
                    {
                        return RedirectToAction("UserAccountBalance", "BankAccount", new { id = id });
                    }
                case AccountContext.MoneyMarket:
                    if (user_Balance.TotalBalance > model.DepositAmount)
                    {
                        insertWithDrawalLimits = 6;
                        insertInterestRate = 5.4m;
                        DepositeFee = 2500000;
                        if (model.DepositAmount > DepositeFee)
                        {
                            var new_account_type_3 = new AccountType
                            {
                                AccountID = new_Account_Record.AccountContextID,
                                OwnerEmail = user_Balance.EmailAddress,
                                Balance = model.DepositAmount,
                                CreatedDate = DateTime.Now,
                                DeductionDate = DeductionDate,
                                Context = AccountContext.MoneyMarket,
                                WithDrawalLimits = insertWithDrawalLimits,
                                InterestRate = insertInterestRate,
                                DepositFee = DepositeFee,
                                AccountTransactionRecordID = new_Account_Record.AccountRecordID,
                            };
                            user_Balance.TotalBalance -= model.DepositAmount;
                            _db.AccountTypes.Add(new_account_type_3);
                            await _db.SaveChangesAsync();
                            ModelState.Clear();
                            return RedirectToAction("Index", "BankAccount");
                        }
                        else
                        {
                            ModelState.Clear();
                            ModelState.AddModelError("", "Desposit Amount Below Requirement:2500000");
                            var fill_Form_OwnerID = new BankAccountCreationForm
                            {
                                TotalBalance = user_Balance.TotalBalance,
                                OwnerID = user_Balance.Id,
                                DepositAmount = 0,
                                AccountContext = AccountContext.Checking,
                            };
                            return View(fill_Form_OwnerID);
                        }
                    }
                    else
                    {
                        return RedirectToAction("UserAccountBalance", "BankAccount");
                    }
                case AccountContext.CertificateOfDeposit:

                    break;
            }
            return View();
        }
        [HttpPost]
        public IActionResult Delete(Guid id)
        {
            var RecordToDelete=_db.TransactionsRecords.FirstOrDefault(Rec=>Rec.AccountRecordID==id);
            var accountToDelete = _db.AccountTypes.FirstOrDefault(acc => acc.AccountTransactionRecordID == RecordToDelete.AccountContextID);
            try
            {
                _db.TransactionsRecords.Remove(RecordToDelete);
                _db.AccountTypes.Remove(accountToDelete);
                _db.SaveChanges();
                return RedirectToAction("Index", "BankAccount");
            }catch(DbException e)
            {
                e.ToString();
            }
            return View();
        }
    }
}
