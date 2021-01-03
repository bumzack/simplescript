using exampleservice.AccoutingService.Contract;

namespace exampleservice.AccoutingService
{
    public class WithdrawFromAccountContext
    {
        public bool WasCompensated { get; set; }

        public WithdrawFromCustomerCommand Command { get; set; }
        public bool HasWithdrawn { get; set; }
        public bool HasSufficientAmount { get; set; }

    }
}