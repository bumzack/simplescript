using exampleservice.Framework.BaseFramework;

namespace exampleservice.AccoutingService.Contract
{
    public class CouldNotCreateAccountEvent : EventBase
    {
        public int CustomerId { get; internal set; }

        // public string ErrorMessage { get; internal set;  }
    }
}
