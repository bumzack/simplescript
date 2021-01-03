using exampleservice.AccoutingService.Contract;

namespace exampleservice.AccoutingService
{
    public class DepositToAccountContext
    {
        public bool WasCompensated { get; set; }

        public DepositToAccountCommand Command { get; set; }
        public bool HasDeposited { get; set; }
    }
}