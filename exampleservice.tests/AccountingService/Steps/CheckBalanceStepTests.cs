using System.Threading.Tasks;
using exampleservice.AccoutingService;
using exampleservice.AccoutingService.Contract;
using exampleservice.AccoutingService.Controller;
using exampleservice.AccoutingService.Steps;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using NUnit.Framework;

namespace exampleservice.tests.AccountingService.Steps
{
    [TestFixture]
    public class CheckBalanceStepTests
    {
        [Test]
        public async Task GetAmount_Success()
        {
            int AccountId = 23;
            int AvailableAmount = 2;
            int RequestedAmount = 1;

            var dataBaseRepositoryMock = new Mock<IAccountServiceDataBaseRepository>();
            // dataBaseRepositoryMock.Setup(s => s.CreateAccount(AccountId)).ReturnsAsync(AccountId);
            dataBaseRepositoryMock.Setup(s => s.GetAmount(AccountId)).ReturnsAsync(AvailableAmount);

            var instanceUnderTest =
                new CheckBalanceStep(dataBaseRepositoryMock.Object);
            var context = new WithdrawFromAccountContext
                {Command = new WithdrawFromCustomerCommand {Amount = RequestedAmount, AccountId = AccountId}};

            await instanceUnderTest.Execute(context);

            using (new AssertionScope())
            {
                context.WasCompensated.Should().BeFalse();
                context.HasSufficientAmount.Should().BeTrue();
            }

            dataBaseRepositoryMock.Verify(s => s.GetAmount(AccountId));
        }

        [Test]
        public async Task GetAmount_insufficientBalance_should_fail()
        {
            int AccountId = 23;
            int AvailableAmount = 1;
            int RequestedAmount = 11111;

            var dataBaseRepositoryMock = new Mock<IAccountServiceDataBaseRepository>();
            // dataBaseRepositoryMock.Setup(s => s.CreateAccount(AccountId)).ReturnsAsync(AccountId);
            dataBaseRepositoryMock.Setup(s => s.GetAmount(AccountId)).ReturnsAsync(AvailableAmount);

            var instanceUnderTest =
                new CheckBalanceStep(dataBaseRepositoryMock.Object);
            var context = new WithdrawFromAccountContext
                {Command = new WithdrawFromCustomerCommand {Amount = RequestedAmount, AccountId = AccountId}};

            await instanceUnderTest.Execute(context);

            using (new AssertionScope())
            {
                context.WasCompensated.Should().BeTrue();
                context.HasSufficientAmount.Should().BeFalse();
            }

            dataBaseRepositoryMock.Verify(s => s.GetAmount(AccountId));
        }

        
        // TODO: is this test useful?
        // maybe add another event AccountNotFoundEvent?
        [Test]
        public async Task GetAmount_unknownAccount_should_fail()
        {
            int ExistingAccountId = 23;
            int RequestedAccountId = 45;
            int Amount = 1;

            var dataBaseRepositoryMock = new Mock<IAccountServiceDataBaseRepository>();
            // dataBaseRepositoryMock.Setup(s => s.CreateAccount(AccountId)).ReturnsAsync(AccountId);
            dataBaseRepositoryMock.Setup(s => s.GetAmount(ExistingAccountId)).ReturnsAsync(Amount);

            var instanceUnderTest =
                new CheckBalanceStep(dataBaseRepositoryMock.Object);
            var context = new WithdrawFromAccountContext
                {Command = new WithdrawFromCustomerCommand {Amount = Amount, AccountId = RequestedAccountId}};

            await instanceUnderTest.Execute(context);

            using (new AssertionScope())
            {
                context.WasCompensated.Should().BeTrue();
                context.HasSufficientAmount.Should().BeFalse();
            }

            dataBaseRepositoryMock.Verify(s => s.GetAmount(RequestedAccountId));
        }
    }
}