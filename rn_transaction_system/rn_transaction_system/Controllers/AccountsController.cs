using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rn_transaction_system.Commands;
using rn_transaction_system.Queries;


namespace rn_transaction_system.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IMediator _mediatorService;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IMediator mediator, ILogger<AccountsController> logger)
        {
            _mediatorService = mediator;
            _logger = logger;
        }

        // POST: api/accounts/deposit
        [HttpPost("deposit")]
        public async Task<IActionResult> AddFunds([FromBody] DepositCommand command)
        {
            try
            {
                await _mediatorService.Send(command);
                _logger.LogInformation("Deposit of {Amount} was successful for account {AccountId}.", command.Amount, command.AccountId);
                return Ok("Funds added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing deposit for account {AccountId}", command.AccountId);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // POST: api/accounts/withdraw
        [HttpPost("withdraw")]
        public async Task<IActionResult> WithdrawFunds([FromBody] WithdrawCommand command)
        {
            try
            {
                await _mediatorService.Send(command);
                _logger.LogInformation("Withdrawal of {Amount} completed successfully for account {AccountId}.", command.Amount, command.AccountId);
                return Ok("Withdrawal completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during withdrawal for account {AccountId}", command.AccountId);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/accounts/{id}/balance
        [HttpGet("{id}/balance")]
        public async Task<IActionResult> GetAccountBalance([FromRoute] int id)
        {
            try
            {
                var balance = await _mediatorService.Send(new GetAccountBalanceQuery { AccountId = id });
                _logger.LogInformation("Balance retrieved for account {AccountId}: {Balance}", id, balance);
                return Ok(new { balance });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving balance for account {AccountId}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
