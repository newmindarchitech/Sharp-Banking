
using BankingVault.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace BankingVault.BackgroundServices
{
    public class PenaltyCheckService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<PenaltyCheckService> _logger;

        public PenaltyCheckService(IServiceScopeFactory scopeFactory, ILogger<PenaltyCheckService> logger)
        {
            this._scopeFactory = scopeFactory;
            this._logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
                decimal? PenaltyFeePercentage;
                try
                {
                    _logger.LogInformation("Running penalty check background service...");
                    await using var scope = _scopeFactory.CreateAsyncScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                    var withDrawalChecks= await dbContext.AccountTypes.ToListAsync(stoppingToken);
                   
                        foreach (var check in withDrawalChecks)
                        {
                            if (check.Context!=AccountContext.Checking && check.WithDrawalLimits==0)
                            {
                                
                                switch (check.Context)
                                {
                                    case AccountContext.Saving:
                                        PenaltyFeePercentage = 10.0m;
                                        check.PenaltyFees = PenaltyFeePercentage;
                                        break;
                                    case AccountContext.MoneyMarket:
                                        PenaltyFeePercentage = 20.0m;
                                        check.PenaltyFees = PenaltyFeePercentage;
                                        break;
                                    case AccountContext.CertificateOfDeposit:
                                        PenaltyFeePercentage = 40.0m;
                                        check.PenaltyFees = PenaltyFeePercentage;
                                        break;
                                }
                            }
                        }
                        await dbContext.SaveChangesAsync(stoppingToken);                  
                }
                catch (DbException ex)
                {
                    _logger.LogError(ex, $"Error in penalty service: {ex}");
                }
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            
        }
    }
}
