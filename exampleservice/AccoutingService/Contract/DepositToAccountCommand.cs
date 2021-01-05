namespace exampleservice.AccoutingService.Contract
{
    public class DepositToAccountCommand
    {
        public int Amount { get;   set; }
        public int AccountId { get;   set; }
    }
}
