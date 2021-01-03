using System;
using System.Threading.Tasks;
using exampleservice.AccoutingService.Controller;
using simplescript.Abstract;

namespace exampleservice.AccoutingService.Steps
{
    public class CreateAccountWithOptimisticLock : ProcedureStepBase<CreateAccountContext>
    {
        private IAccountServiceDataBaseRepository dataRepository;

        public CreateAccountWithOptimisticLock(IAccountServiceDataBaseRepository dataRepository) =>
            this.dataRepository =
                dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));

        protected async override Task<bool> StepSpecificExecute(CreateAccountContext contextType)
        {
            int accountId = await dataRepository.CreateAccount(contextType.Command.CustomerId);

            if (accountId < 0)
            {
                await CompensatePredecssorOnly(contextType);
                contextType.WasCompensated = true;
                contextType.AccountCreated = false;
                return true;
            }

            contextType.AccountCreated = true;
            contextType.NewAccountId = accountId;
            return false;
        }
    }
}