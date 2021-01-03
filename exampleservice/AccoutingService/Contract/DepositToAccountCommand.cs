namespace exampleservice.AccoutingService.Contract
{
    public class DepositToAccountCommand
    {
        public int Amount { get; internal set; }
        public int AccountId { get; internal set; }
    }
}
