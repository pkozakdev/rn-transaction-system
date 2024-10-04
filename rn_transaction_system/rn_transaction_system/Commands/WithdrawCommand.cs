using MediatR;

namespace rn_transaction_system.Commands
{
    public class WithdrawCommand : IRequest<Unit>  
    {
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
    }
}
