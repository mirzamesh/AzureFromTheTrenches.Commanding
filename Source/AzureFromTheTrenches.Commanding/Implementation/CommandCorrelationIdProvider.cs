﻿using System;
using System.Threading;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    class CommandCorrelationIdProvider : ICommandCorrelationIdProvider
    {
        public string Create()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
