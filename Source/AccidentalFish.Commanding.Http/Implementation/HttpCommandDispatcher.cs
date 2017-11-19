﻿using System.Threading.Tasks;
using AccidentalFish.Commanding.Abstractions;
using AccidentalFish.Commanding.Abstractions.Model;

namespace AccidentalFish.Commanding.Http.Implementation
{
    class HttpCommandDispatcher : ICommandDispatcher
    {
        public HttpCommandDispatcher(ICommandExecuter httpCommandExecuter)
        {
            AssociatedExecuter = httpCommandExecuter;
        }

        public Task<CommandResult<TResult>> DispatchAsync<TResult>(ICommand<TResult> command)
        {
            return Task.FromResult(new CommandResult<TResult>(default(TResult), false));
        }

        public Task<CommandResult> DispatchAsync(ICommand command)
        {
            return Task.FromResult(new CommandResult(false));
        }

        public ICommandExecuter AssociatedExecuter { get; }
    }
}
