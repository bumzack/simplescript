﻿namespace exampleservice.AccoutingService.Contract
{
    public class DepositToCustomerCommand
    {
        public int Amount { get; internal set; }
        public int AccountId { get; internal set; }
    }
}
