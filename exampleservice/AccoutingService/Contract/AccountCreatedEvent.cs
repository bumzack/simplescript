using exampleservice.Framework.BaseFramework;

namespace exampleservice.AccoutingService.Contract
{
    public class AccountCreatedEvent : EventBase
    {
        public int CustomerId { get; internal set; }
        public int AccountId { get; internal set; }
    }
}
