using System.Threading.Tasks;
using exampleservice.AccoutingService.Contract;
using exampleservice.AccoutingService.Controller;
using exampleservice.Framework.Abstract;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using NUnit.Framework;

namespace exampleservice.tests.AccountingService
{
    [TestFixture]
    public class AccountingServiceTests
    {
        [Test]
        public async Task AllStepSucceed()
        {
            int availableAmount = 10;
            int withdrawAmount = 5;
            int accountId = 1;

            var busMock = new Mock<IMessageBus>();

            // busMock.Setup(s => s.RequestAndReply<WithdrawFromCustomerCommand>(It.IsAny<WithdrawFromCustomerCommand>())).
            //     ReturnsAsync(new WithdrawnFromCustomerEvent());
            // busMock.Setup(s => s.RequestAndReply<DepositToCustomerCommand>(It.IsAny<DepositToCustomerCommand>())).
            //     ReturnsAsync(new DepositedToCustomerEvent());
            // busMock.Setup(s => s.RequestAndReply<FlagTicketAsSoldCommand>(It.IsAny<FlagTicketAsSoldCommand>())).
            //     ReturnsAsync(new FlagedTicketAsSoldEvent());

            var dataBaseMock = new Mock<IAccountServiceDataBaseRepository>();
            dataBaseMock.Setup(d => d.GetAmount(It.IsAny<int>())).ReturnsAsync(availableAmount);
            dataBaseMock.Setup(d => d.ChangeAmount(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

            var instanceUnderTest = new AccoutingService.AccountingService(busMock.Object, dataBaseMock.Object);
            var withdrawFromCustomerCommand = new WithdrawFromCustomerCommand
            {
                Amount = withdrawAmount,
                AccountId = accountId
            };

            var resultedEvent = await instanceUnderTest.Handle(withdrawFromCustomerCommand);

            using (new AssertionScope())
            {
                resultedEvent.Should().BeOfType(typeof(WithdrawnFromCustomerEvent));
                var withdrawnFromCustomerEvent = (WithdrawnFromCustomerEvent) resultedEvent;
                withdrawnFromCustomerEvent.Amount.Should().Be(withdrawAmount);
            }
        }

        [Test]
        public async Task BalanceTooLow_should_fail()
        {
            int availableAmount = 10;
            int withdrawAmount = availableAmount + 1;
            int accountId = 1;

            var busMock = new Mock<IMessageBus>();

            var dataBaseMock = new Mock<IAccountServiceDataBaseRepository>();
            dataBaseMock.Setup(d => d.GetAmount(It.IsAny<int>())).ReturnsAsync(availableAmount);
            dataBaseMock.Setup(d => d.ChangeAmount(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

            var instanceUnderTest = new AccoutingService.AccountingService(busMock.Object, dataBaseMock.Object);
            var withdrawFromCustomerCommand = new WithdrawFromCustomerCommand
            {
                Amount = withdrawAmount,
                AccountId = accountId
            };

            var resultedEvent = await instanceUnderTest.Handle(withdrawFromCustomerCommand);

            using (new AssertionScope())
            {
                resultedEvent.Should().BeOfType(typeof(CouldNotWithdrawFromCustomerEvent));
                var couldNotWithdrawFromCustomerEvent = (CouldNotWithdrawFromCustomerEvent) resultedEvent;
                couldNotWithdrawFromCustomerEvent.Amount.Should().Be(withdrawAmount);
            }
        }

        [Test]
        public async Task UnknownAccountIt_should_fail()
        {
            int availableAmount = 10;
            int withdrawAmount = availableAmount + 1;
            int accountId = 1;
            int unknownAccountId = 2;

            var busMock = new Mock<IMessageBus>();

            var dataBaseMock = new Mock<IAccountServiceDataBaseRepository>();
            dataBaseMock.Setup(d => d.GetAmount(It.IsAny<int>())).ReturnsAsync(availableAmount);
            dataBaseMock.Setup(d => d.ChangeAmount(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

            var instanceUnderTest = new AccoutingService.AccountingService(busMock.Object, dataBaseMock.Object);
            var withdrawFromCustomerCommand = new WithdrawFromCustomerCommand
            {
                Amount = withdrawAmount,
                AccountId = unknownAccountId
            };

            var resultedEvent = await instanceUnderTest.Handle(withdrawFromCustomerCommand);

            using (new AssertionScope())
            {
                resultedEvent.Should().BeOfType(typeof(CouldNotWithdrawFromCustomerEvent));
                var couldNotWithdrawFromCustomerEvent = (CouldNotWithdrawFromCustomerEvent) resultedEvent;
                couldNotWithdrawFromCustomerEvent.Amount.Should().Be(withdrawAmount);
            }
        }
    }
}