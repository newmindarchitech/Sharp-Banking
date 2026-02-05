
using BankingVault.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace BankingVault.BackgroundServices
{
    public class MaintenanceFeeService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MaintenanceFeeService> _logger;

        public MaintenanceFeeService(IServiceScopeFactory scopeFactory, ILogger<MaintenanceFeeService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
                try
                {
                    _logger.LogInformation("Running maintenance fee check background service...");
                    await using var scope = _scopeFactory.CreateAsyncScope();
                    var DbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                    var accountsToPenalize = await DbContext.AccountTypes.Where(less=>less.DeductionDate==DateTime.Today).ToListAsync(stoppingToken);
                    if (accountsToPenalize.Count != 0)
                    {
                        decimal MaintenanceFee;
                        var rand=new Random();
                        foreach(var account in accountsToPenalize)
                        {
                            if (account.Balance < account.DepositFee) {
                                switch (account.Context) {
                                    case AccountContext.Saving:
                                        MaintenanceFee = rand.Next(50000, 100000);
                                        account.Balance -= MaintenanceFee;
                                        break;
                                    case AccountContext.MoneyMarket:
                                        MaintenanceFee = rand.Next(200000, 500000);
                                        account.Balance -= MaintenanceFee;
                                        break;
                                    case AccountContext.CertificateOfDeposit:
                                        MaintenanceFee = rand.Next(600000, 2000000);
                                        account.Balance -= MaintenanceFee;
                                        break;
                                }
                            }
                        }
                        await DbContext.SaveChangesAsync(stoppingToken);
                    }
                } catch(DbException ex)
                {
                    _logger.LogError(ex, $"Error in penalty service: {ex}");
                }
            
            await Task.Delay(TimeSpan.FromDays(1),stoppingToken);
        }
    }
}
