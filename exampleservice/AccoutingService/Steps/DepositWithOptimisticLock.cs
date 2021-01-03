using System;
using System.Threading.Tasks;
using exampleservice.AccoutingService.Controller;
using simplescript.Abstract;

namespace exampleservice.AccoutingService.Steps
{
    public class DepositWithOptimisticLock : ProcedureStepBase<DepositToAccountContext>
    {
        private IAccountServiceDataBaseRepository dataRepository;

        public DepositWithOptimisticLock(IAccountServiceDataBaseRepository dataRepository) => this.dataRepository =
            dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));

        protected async override Task<bool> StepSpecificExecute(DepositToAccountContext contextType)
        {
            // deposit -> +amount
            bool res = await dataRepository.ChangeAmount(
                contextType.Command.AccountId,
                +contextType.Command.Amount);

            if (!res)
            {
                await CompensatePredecssorOnly(contextType);
                contextType.WasCompensated = true;
                contextType.HasDeposited = false;
                return true;
            }

            contextType.HasDeposited = true;
            return false;
        }
    }
}