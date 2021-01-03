using System;
using System.Threading.Tasks;
using exampleservice.AccoutingService.Controller;
using simplescript.Abstract;

namespace exampleservice.AccoutingService.Steps
{
    public class CheckBalanceStep : ProcedureStepBase<WithdrawFromAccountContext>
    {
        private IAccountServiceDataBaseRepository dataRepository;

        public CheckBalanceStep(IAccountServiceDataBaseRepository dataRepository) => this.dataRepository =
            dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));

        protected async override Task<bool> StepSpecificExecute(WithdrawFromAccountContext contextType)
        {
            int availableAmount = await dataRepository.GetAmount(
                contextType.Command.AccountId);

            if (availableAmount < contextType.Command.Amount)
            {
                await CompensatePredecssorOnly(contextType);
                contextType.WasCompensated = true;
                contextType.HasSufficientAmount = false;
                return true;
            }

            contextType.HasSufficientAmount = true;
            return false;
        }
    }
}