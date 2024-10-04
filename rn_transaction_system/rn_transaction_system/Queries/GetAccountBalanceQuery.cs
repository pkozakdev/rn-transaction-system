using MediatR;

namespace rn_transaction_system.Queries
{
    public class GetAccountBalanceQuery : IRequest<decimal>
    {
        public int AccountId { get; set; }
    }
}
