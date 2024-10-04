using Moq;
using FluentAssertions;
using rn_transaction_system.Commands;
using rn_transaction_system.Handlers;
using rn_transaction_system.Data;
using rn_transaction_system.Entities;
using rn_transaction_system.Hubs;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace rn_transaction_system.Tests.Handlers
{
    public class DepositHandlerTests
    {
        private DbCon CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<DbCon>()
                          .UseInMemoryDatabase(Guid.NewGuid().ToString())
                          .Options;

            return new DbCon(options);
        }

        private Mock<IHubContext<NotificationHub>> CreateMockHubContext()
        {
            var hubContextMock = new Mock<IHubContext<NotificationHub>>();
            var mockClients = new Mock<IHubClients>();
            var mockClientProxy = new Mock<IClientProxy>();

            hubContextMock.Setup(x => x.Clients).Returns(mockClients.Object);
            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);
            mockClientProxy.Setup(clientProxy => clientProxy.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), default))
                           .Returns(Task.CompletedTask);

            return hubContextMock;
        }

        [Fact]
        public async Task Deposit_ShouldIncreaseBalance()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            var loggerMock = new Mock<ILogger<DepositHandler>>();
            var hubContextMock = CreateMockHubContext();

            context.Accounts.Add(new Account { Id = 1, Balance = 100, UserId = "user123", RowVersion = new byte[] { 1, 2, 3, 4 } });
            await context.SaveChangesAsync();

            var handler = new DepositHandler(context, loggerMock.Object, hubContextMock.Object);
            var command = new DepositCommand { AccountId = 1, Amount = 50 };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            var account = await context.Accounts.FindAsync(1);
            account.Balance.Should().Be(150);
            hubContextMock.Verify(x => x.Clients.All.SendCoreAsync("ReceiveBalanceUpdate", new object[] { account.Id, account.Balance, "Deposit" }, default), Times.Once);
        }
    }
}
