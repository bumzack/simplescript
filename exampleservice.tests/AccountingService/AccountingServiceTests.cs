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
        public async Task WithdrawSucceed()
        {
            int availableAmount = 10;
            int withdrawAmount = 5;
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
                resultedEvent.Should().BeOfType(typeof(WithdrawnFromCustomerEvent));
                var withdrawnFromCustomerEvent = (WithdrawnFromCustomerEvent) resultedEvent;
                withdrawnFromCustomerEvent.Amount.Should().Be(withdrawAmount);
            }
        }

        [Test]
        public async Task Withdraw_BalanceTooLow_should_fail()
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
        public async Task Withdraw_UnknownAccountIt_should_fail()
        {
            int availableAmount = 10;
            int withdrawAmount = availableAmount + 1;
            int accountId = 1;
            int unknownAccountId = 2;

            var busMock = new Mock<IMessageBus>();

            var dataBaseMock = new Mock<IAccountServiceDataBaseRepository>();
            dataBaseMock.Setup(d => d.GetAmount(It.IsAny<int>())).ReturnsAsync(availableAmount);
            dataBaseMock.Setup(d => d.ChangeAmount(accountId, It.IsAny<int>())).ReturnsAsync(true);
            dataBaseMock.Setup(d => d.ChangeAmount(unknownAccountId, It.IsAny<int>())).ReturnsAsync(false);

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

        [Test]
        public async Task DepositSucceed()
        {
            int depositAmount = 5;
            int accountId = 1;

            var busMock = new Mock<IMessageBus>();
            var dataBaseMock = new Mock<IAccountServiceDataBaseRepository>();
            dataBaseMock.Setup(d => d.ChangeAmount(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

            var instanceUnderTest = new AccoutingService.AccountingService(busMock.Object, dataBaseMock.Object);
            var depositToAccountCommand = new DepositToAccountCommand()
            {
                Amount = depositAmount,
                AccountId = accountId
            };

            var resultedEvent = await instanceUnderTest.Handle(depositToAccountCommand);

            using (new AssertionScope())
            {
                resultedEvent.Should().BeOfType(typeof(DepositedToCustomerEvent));
                var depositedToCustomerEvent = (DepositedToCustomerEvent) resultedEvent;
                depositedToCustomerEvent.Amount.Should().Be(depositAmount);
            }
        }

        [Test]
        public async Task Deposit_UnknownAccountIt_should_fail()
        {
            int depositAmount = 1;
            int accountId = 1;
            int unknownAccountId = 2;

            var busMock = new Mock<IMessageBus>();

            var dataBaseMock = new Mock<IAccountServiceDataBaseRepository>();
            dataBaseMock.Setup(d => d.ChangeAmount(accountId, It.IsAny<int>())).ReturnsAsync(true);
            dataBaseMock.Setup(d => d.ChangeAmount(unknownAccountId, It.IsAny<int>())).ReturnsAsync(false);

            var instanceUnderTest = new AccoutingService.AccountingService(busMock.Object, dataBaseMock.Object);
            var depositToAccountCommand = new DepositToAccountCommand()
            {
                Amount = depositAmount,
                AccountId = unknownAccountId
            };

            var resultedEvent = await instanceUnderTest.Handle(depositToAccountCommand);

            using (new AssertionScope())
            {
                resultedEvent.Should().BeOfType(typeof(CouldNotDepositToCustomerEvent));
                var couldNotDepositToCustomerEvent = (CouldNotDepositToCustomerEvent) resultedEvent;
                couldNotDepositToCustomerEvent.Amount.Should().Be(depositAmount);
            }
        }

        [Test]
        public async Task CreateAccount_success()
        {
            var CustomerId = 1;
            var AccountId = CustomerId;

            var busMock = new Mock<IMessageBus>();

            var dataBase = new AccountServiceDataBaseRepository();

            var instanceUnderTest = new AccoutingService.AccountingService(busMock.Object, dataBase);
            var createAccountCommand = new CreateAccountCommand()
            {
                CustomerId = CustomerId
            };

            var resultedEvent = await instanceUnderTest.Handle(createAccountCommand);

            using (new AssertionScope())
            {
                resultedEvent.Should().BeOfType(typeof(AccountCreatedEvent));
                var accountCreatedEvent = (AccountCreatedEvent) resultedEvent;
                accountCreatedEvent.AccountId.Should().Be(AccountId);
            }
        }
    }
}