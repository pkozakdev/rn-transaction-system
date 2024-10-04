using Moq;
using FluentAssertions;
using rn_transaction_system.Queries;
using rn_transaction_system.Handlers;
using rn_transaction_system.Data;
using rn_transaction_system.Entities;
using rn_transaction_system.Hubs;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace rn_transaction_system.Tests.Handlers
{
    public class GetAccountBalanceHandlerTests
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
        public async Task GetBalance_ShouldReturnCorrectBalance()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            var loggerMock = new Mock<ILogger<GetAccountBalanceHandler>>();
            var hubContextMock = CreateMockHubContext();

            context.Accounts.Add(new Account { Id = 1, Balance = 200, UserId = "user123", RowVersion = new byte[] { 1, 2, 3, 4 } });
            await context.SaveChangesAsync();

            var handler = new GetAccountBalanceHandler(context, loggerMock.Object, hubContextMock.Object);
            var query = new GetAccountBalanceQuery { AccountId = 1 };

            // Act
            var balance = await handler.Handle(query, CancellationToken.None);

            // Assert
            balance.Should().Be(200);
            hubContextMock.Verify(x => x.Clients.All.SendCoreAsync("ReceiveBalanceRetrieved", new object[] { 1, balance }, default), Times.Once);
        }
    }
}
