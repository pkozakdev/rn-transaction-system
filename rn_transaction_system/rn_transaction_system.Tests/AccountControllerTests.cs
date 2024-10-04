using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using rn_transaction_system.Controllers;
using rn_transaction_system.Commands;
using rn_transaction_system.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace rn_transaction_system.Tests.Controllers
{
    public class AccountsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<AccountsController>> _loggerMock;
        private readonly AccountsController _controller;

        public AccountsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<AccountsController>>();
            _controller = new AccountsController(_mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task AddFunds_ShouldReturnOk_WhenDepositSucceeds()
        {
            // Arrange
            var depositCommand = new DepositCommand { AccountId = 1, Amount = 100 };
            _mediatorMock.Setup(m => m.Send(It.IsAny<DepositCommand>(), It.IsAny<CancellationToken>()))
                         .Returns(Task.FromResult(Unit.Value));

            // Act
            var result = await _controller.AddFunds(depositCommand);

            // Assert
            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be("Funds added successfully.");
            _mediatorMock.Verify(m => m.Send(It.IsAny<DepositCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddFunds_ShouldReturn500_WhenExceptionOccurs()
        {
            // Arrange
            var depositCommand = new DepositCommand { AccountId = 1, Amount = 100 };
            _mediatorMock.Setup(m => m.Send(It.IsAny<DepositCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new System.Exception("Unexpected error"));

            // Act
            var result = await _controller.AddFunds(depositCommand);

            // Assert
            result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
            _mediatorMock.Verify(m => m.Send(It.IsAny<DepositCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task WithdrawFunds_ShouldReturnOk_WhenWithdrawalSucceeds()
        {
            // Arrange
            var withdrawCommand = new WithdrawCommand { AccountId = 1, Amount = 50 };
            _mediatorMock.Setup(m => m.Send(It.IsAny<WithdrawCommand>(), It.IsAny<CancellationToken>()))
                         .Returns(Task.FromResult(Unit.Value));

            // Act
            var result = await _controller.WithdrawFunds(withdrawCommand);

            // Assert
            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be("Withdrawal completed successfully.");
            _mediatorMock.Verify(m => m.Send(It.IsAny<WithdrawCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task WithdrawFunds_ShouldReturn500_WhenExceptionOccurs()
        {
            // Arrange
            var withdrawCommand = new WithdrawCommand { AccountId = 1, Amount = 50 };
            _mediatorMock.Setup(m => m.Send(It.IsAny<WithdrawCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new System.Exception("Unexpected error"));

            // Act
            var result = await _controller.WithdrawFunds(withdrawCommand);

            // Assert
            result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
            _mediatorMock.Verify(m => m.Send(It.IsAny<WithdrawCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAccountBalance_ShouldReturnBalance_WhenAccountExists()
        {
            // Arrange
            var accountId = 1;
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAccountBalanceQuery>(), It.IsAny<CancellationToken>()))
                         .Returns(Task.FromResult(150m));

            // Act
            var result = await _controller.GetAccountBalance(accountId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(new { balance = 150m });
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetAccountBalanceQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAccountBalance_ShouldReturn500_WhenExceptionOccurs()
        {
            // Arrange
            var accountId = 1;
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAccountBalanceQuery>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new System.Exception("Unexpected error"));

            // Act
            var result = await _controller.GetAccountBalance(accountId);

            // Assert
            result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetAccountBalanceQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
