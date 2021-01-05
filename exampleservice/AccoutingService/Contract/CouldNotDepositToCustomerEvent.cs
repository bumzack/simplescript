using exampleservice.Framework.BaseFramework;

namespace exampleservice.AccoutingService.Contract
{
    public class CouldNotDepositToCustomerEvent : EventBase
    {
        public int Amount { get; set; }
        public int AccountId { get; set; }
    }
}