using MediatR;
using Microsoft.EntityFrameworkCore;
using rn_transaction_system.Commands;
using rn_transaction_system.Data;
using rn_transaction_system.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace rn_transaction_system.Handlers
{
    public class WithdrawHandler : IRequestHandler<WithdrawCommand, Unit>
    {
        private readonly DbCon _dbContext;
        private readonly ILogger<WithdrawHandler> _log;
        private readonly IHubContext<NotificationHub> _notificationHub;

        public WithdrawHandler(DbCon context, ILogger<WithdrawHandler> logger, IHubContext<NotificationHub> hubContext)
        {
            _dbContext = context;
            _log = logger;
            _notificationHub = hubContext;
        }

        public async Task<Unit> Handle(WithdrawCommand command, CancellationToken cancellationToken)
        {
            var existingAccount = await _dbContext.Accounts.FindAsync(command.AccountId);

            if (existingAccount == null)
            {
                _log.LogWarning("Withdrawal failed: Account with ID {AccountId} not found", command.AccountId);
                throw new ArgumentException("Account not found.");
            }

            if (existingAccount.Balance < command.Amount)
            {
                _log.LogWarning("Withdrawal failed: Insufficient funds in account {AccountId}. Requested: {Amount}, Available: {Balance}",
                                command.AccountId, command.Amount, existingAccount.Balance);
                throw new InvalidOperationException("Insufficient funds for this transaction.");
            }

            existingAccount.Balance -= command.Amount;

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
                await _notificationHub.Clients.All.SendAsync("ReceiveBalanceUpdate", existingAccount.Id, existingAccount.Balance, "Withdraw");

                _log.LogInformation("Withdrawal of {Amount} from account {AccountId} completed. New balance: {Balance}",
                                    command.Amount, existingAccount.Id, existingAccount.Balance);
            }
            catch (DbUpdateConcurrencyException concurrencyEx)
            {
                _log.LogError(concurrencyEx, "Concurrency conflict while processing withdrawal for account {AccountId}.", existingAccount.Id);
                throw new InvalidOperationException("Concurrent update detected. Please try the transaction again.");
            }

            return Unit.Value; 
        }
    }
}
