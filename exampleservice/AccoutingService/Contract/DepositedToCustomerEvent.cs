using exampleservice.Framework.BaseFramework;

namespace exampleservice.AccoutingService.Contract
{
    public class DepositedToCustomerEvent : EventBase
    {
        public int Amount { get; internal set; }
        public int AccountId { get; internal set; } 
    }
}
