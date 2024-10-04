using MediatR;
using rn_transaction_system.Queries;
using rn_transaction_system.Data;
using rn_transaction_system.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace rn_transaction_system.Handlers
{
    public class GetAccountBalanceHandler : IRequestHandler<GetAccountBalanceQuery, decimal>
    {
        private readonly DbCon _dbContext;
        private readonly ILogger<GetAccountBalanceHandler> _log;
        private readonly IHubContext<NotificationHub> _notificationHub;

        public GetAccountBalanceHandler(DbCon db, ILogger<GetAccountBalanceHandler> logger, IHubContext<NotificationHub> hubContext)
        {
            _dbContext = db;
            _log = logger;
            _notificationHub = hubContext;
        }

        public async Task<decimal> Handle(GetAccountBalanceQuery request, CancellationToken cancellationToken)
        {
            var accountDetails = await _dbContext.Accounts.FindAsync(request.AccountId);

            if (accountDetails == null)
            {
                _log.LogWarning("Balance retrieval failed: Account {AccountId} not found", request.AccountId);
                throw new ArgumentException("Account not found.");
            }

            _log.LogInformation("Retrieved balance for account {AccountId}: {Balance}", accountDetails.Id, accountDetails.Balance);

            await _notificationHub.Clients.All.SendAsync("ReceiveBalanceRetrieved", accountDetails.Id, accountDetails.Balance);

            return accountDetails.Balance;
        }
    }
}
