using exampleservice.AccoutingService.Contract;

namespace exampleservice.AccoutingService
{
    public class CreateAccountContext
    {
        public bool WasCompensated { get; set; }

        public CreateAccountCommand Command { get; set; }
        public int NewAccountId { get; set; }
        public bool AccountCreated { get; set; }
    }
}
