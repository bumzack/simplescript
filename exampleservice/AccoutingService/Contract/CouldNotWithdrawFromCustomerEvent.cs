﻿using exampleservice.Framework.BaseFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace exampleservice.AccoutingService.Contract
{
    public class CouldNotWithdrawFromCustomerEvent : EventBase
    {
        public int Amount { get; internal set; }
        public int AccountId { get; internal set; }
    }
}