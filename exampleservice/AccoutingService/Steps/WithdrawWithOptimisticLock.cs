using System;
using System.Threading.Tasks;
using exampleservice.AccoutingService.Controller;
using simplescript.Abstract;

namespace exampleservice.AccoutingService.Steps
{
    public class WithdrawWithOptimisticLock : ProcedureStepBase<WithdrawFromAccountContext>
    {
        private IAccountServiceDataBaseRepository dataRepository;

        public WithdrawWithOptimisticLock(IAccountServiceDataBaseRepository dataRepository) => this.dataRepository =
            dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));

        protected async override Task<bool> StepSpecificExecute(WithdrawFromAccountContext contextType)
        {
            // withdraw -> -amount
            bool res = await dataRepository.ChangeAmount(
                contextType.Command.AccountId,
                -contextType.Command.Amount);

            if (!res)
            {
                await CompensatePredecssorOnly(contextType);
                contextType.WasCompensated = true;
                contextType.HasWithdrawn = false;
                return true;
            }

            contextType.HasWithdrawn = true;
            return false;
        }
    }
}