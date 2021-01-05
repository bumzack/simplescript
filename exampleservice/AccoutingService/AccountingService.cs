using System;
using System.Threading.Tasks;
using exampleservice.AccoutingService.Contract;
using exampleservice.AccoutingService.Controller;
using exampleservice.AccoutingService.Steps;
using exampleservice.Framework.Abstract;
using exampleservice.Framework.BaseFramework;
using simplescript;
using simplescript.DSL;

namespace exampleservice.AccoutingService
{
    public class AccountingService
    {
        private Lazy<Procedure<WithdrawFromAccountContext>> procedureWithDraw;
        private Lazy<Procedure<DepositToAccountContext>> procedureDeposit;
        private Lazy<Procedure<CreateAccountContext>> procedureCreateAccount;

        //  private IMessageBus bus;
        private IAccountServiceDataBaseRepository dataBaseRepository;

        public AccountingService(IMessageBus bus, IAccountServiceDataBaseRepository dataBaseRepository)
        {
            procedureWithDraw = new Lazy<Procedure<WithdrawFromAccountContext>>(() => this.GetProcedureWithdraw());
            procedureDeposit = new Lazy<Procedure<DepositToAccountContext>>(() => this.GetProcedureDeposit());
            procedureCreateAccount = new Lazy<Procedure<CreateAccountContext>>(() => this.GetProcedureCreateAccount());
            //   this.bus = bus ?? throw new ArgumentNullException(nameof(bus));
            this.dataBaseRepository = dataBaseRepository ?? throw new ArgumentNullException(nameof(dataBaseRepository));
        }

        public async Task<EventBase> Handle(WithdrawFromCustomerCommand command)
        {
            VerifyInputArgumentsWithdrawal(command);
            var context = new WithdrawFromAccountContext() {Command = command};
            await procedureWithDraw.Value.Execute(context);
            if (context.WasCompensated)
            {
                return new CouldNotWithdrawFromCustomerEvent
                {
                    Amount = command.Amount,
                    AccountId = command.AccountId
                };
            }

            return new WithdrawnFromCustomerEvent
            {
                Amount = command.Amount,
                AccountId = command.AccountId
            };
        }

        public async Task<EventBase> Handle(DepositToAccountCommand command)
        {
            VerifyInputArgumentsDeposit(command);
            var context = new DepositToAccountContext() {Command = command};
            await procedureDeposit.Value.Execute(context);
            if (context.WasCompensated)
            {
                return new CouldNotDepositToCustomerEvent()
                {
                    Amount = command.Amount,
                    AccountId = command.AccountId
                };
            }

            return new DepositedToCustomerEvent()
            {
                Amount = command.Amount,
                AccountId = command.AccountId
            };
        }

        public async Task<EventBase> Handle(CreateAccountCommand command)
        {
            VerifyInputArgumentsCreateAccount(command);
            var context = new CreateAccountContext {Command = command};
            await procedureCreateAccount.Value.Execute(context);
            if (context.WasCompensated)
            {
                return new CouldNotCreateAccountEvent()
                {
                    CustomerId = command.CustomerId
                };
            }

            return new AccountCreatedEvent
            {
                CustomerId = command.CustomerId,
                AccountId = context.NewAccountId
            };
        }

        private void VerifyInputArgumentsWithdrawal(WithdrawFromCustomerCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException();
            }
        }

        private void VerifyInputArgumentsDeposit(DepositToAccountCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException();
            }
        }

        private void VerifyInputArgumentsCreateAccount(CreateAccountCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException();
            }
        }

        private Procedure<WithdrawFromAccountContext> GetProcedureWithdraw()
        {
            var checkBalanceStep = new CheckBalanceStep(dataBaseRepository);
            var withdrawStep = new WithdrawWithOptimisticLock(dataBaseRepository);
            return ProcedureDescription<WithdrawFromAccountContext>
                .Start()
                .Then(checkBalanceStep)
                .Then(withdrawStep)
                .Finish();
        }

        private Procedure<DepositToAccountContext> GetProcedureDeposit()
        {
            var depositStep = new DepositWithOptimisticLock(dataBaseRepository);
            return ProcedureDescription<DepositToAccountContext>
                .Start()
                .Then(depositStep)
                .Finish();
        }

        private Procedure<CreateAccountContext> GetProcedureCreateAccount()
        {
            var createAccountStep = new CreateAccountWithOptimisticLock(dataBaseRepository);

            return ProcedureDescription<CreateAccountContext>
                .Start()
                .Then(createAccountStep)
                .Finish();
        }
    }
}