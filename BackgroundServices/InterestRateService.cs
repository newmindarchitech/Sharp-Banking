
using BankingVault.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingVault.BackgroundServices
{
    public class InterestRateService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<InterestRateService> _logger;

        public InterestRateService(IServiceScopeFactory scopeFactory, ILogger<InterestRateService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
                _logger.LogInformation("Running Interest rate backgorund service");
                await using var scope=_scopeFactory.CreateAsyncScope();
                var DbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

                var accountsWithDueInterests = await DbContext.AccountTypes.Where(date => date.DeductionDate == DateTime.Today).ToListAsync(stoppingToken);
                if (accountsWithDueInterests.Count!=0) {
                    decimal result;
                    try
                    {
                        var rand = new Random();
                        foreach (var account in accountsWithDueInterests)
                        {
                            switch (account.Context)
                            {
                                case AccountContext.Saving:
                                    result = (account.Balance * account.InterestRate) / 100;
                                    account.Balance += result;
                                    account.DeductionDate = DateTime.Today.AddMonths(1);
                                    break;
                                case AccountContext.MoneyMarket:
                                    result = (account.Balance * account.InterestRate) / 100;
                                    account.Balance += result;
                                    account.DeductionDate = DateTime.Today.AddMonths(1);
                                    break;
                                case AccountContext.CertificateOfDeposit:
                                    result = (account.Balance * account.InterestRate) / 100;
                                    account.Balance += result;
                                    account.DeductionDate = DateTime.Today.AddMonths(rand.Next(3, 37));
                                    break;
                            }
                        }
                        await DbContext.SaveChangesAsync(stoppingToken);
                    }
                    catch (Exception ex) {
                        _logger.LogError(ex, "Error in notification background service");
                    }
                    
                }
            
            await Task.Delay(TimeSpan.FromDays(1),stoppingToken);
        }
    }
}
