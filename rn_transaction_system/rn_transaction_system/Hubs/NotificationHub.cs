using Microsoft.AspNetCore.SignalR;

namespace rn_transaction_system.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendBalanceChangeNotification(int accountId, decimal newBalance, string operation)
        {
            await Clients.All.SendAsync("ReceiveBalanceUpdate", accountId, newBalance, operation);
        }
    }
}
