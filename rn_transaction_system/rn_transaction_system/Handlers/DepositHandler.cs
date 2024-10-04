using MediatR;
using Microsoft.EntityFrameworkCore;
using rn_transaction_system.Commands;
using rn_transaction_system.Data;
using rn_transaction_system.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace rn_transaction_system.Handlers
{
    public class DepositHandler : IRequestHandler<DepositCommand, Unit>
    {
        private readonly DbCon _db;
        private readonly ILogger<DepositHandler> _logger;
        private readonly IHubContext<NotificationHub> _notificationHub;

        public DepositHandler(DbCon dbContext, ILogger<DepositHandler> logger, IHubContext<NotificationHub> hubContext)
        {
            _db = dbContext;
            _logger = logger;
            _notificationHub = hubContext;
        }

        public async Task<Unit> Handle(DepositCommand request, CancellationToken token)
        {
            var account = await _db.Accounts.FindAsync(request.AccountId);

            if (account == null)
            {
                _logger.LogWarning("Deposit failed: No account found with ID {AccountId}", request.AccountId);
                throw new ArgumentException("The specified account does not exist.");
            }

            account.Balance += request.Amount;

            try
            {
                await _db.SaveChangesAsync(token);
                await _notificationHub.Clients.All.SendAsync("ReceiveBalanceUpdate", account.Id, account.Balance, "Deposit");

                _logger.LogInformation("Deposited {Amount} to account {AccountId}. New balance: {Balance}",
                                       request.Amount, account.Id, account.Balance);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency conflict during deposit for account {AccountId}.", account.Id);
                throw new InvalidOperationException("A concurrency issue occurred. Please try the transaction again.");
            }

            return Unit.Value;
        }
    }
}
