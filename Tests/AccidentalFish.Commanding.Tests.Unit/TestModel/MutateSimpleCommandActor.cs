﻿using System.Threading.Tasks;
using AccidentalFish.Commanding.Abstractions;

namespace AccidentalFish.Commanding.Tests.Unit.TestModel
{
    class MutateSimpleCommandActor : ICommandActor<SimpleCommand, SimpleResult>
    {
        public Task<SimpleResult> ExecuteAsync(SimpleCommand command, SimpleResult previousResult)
        {
            command.Message = "i did mutate it";
            return Task.FromResult(previousResult);
        }
    }
}
