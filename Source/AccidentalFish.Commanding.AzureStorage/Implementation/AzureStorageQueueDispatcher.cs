﻿using System.Threading.Tasks;
using AccidentalFish.Commanding.Abstractions;
using AccidentalFish.Commanding.Abstractions.Model;
using AccidentalFish.Commanding.Model;
using Microsoft.WindowsAzure.Storage.Queue;

namespace AccidentalFish.Commanding.AzureStorage.Implementation
{
    class AzureStorageQueueDispatcher : ICommandDispatcher
    {
        private readonly CloudQueue _queue;
        private readonly IAzureStorageQueueSerializer _serializer;

        public AzureStorageQueueDispatcher(CloudQueue queue, IAzureStorageQueueSerializer serializer)
        {
            _queue = queue;
            _serializer = serializer;
        }

        public async Task<CommandResult<TResult>> DispatchAsync<TCommand, TResult>(TCommand command) where TCommand : class
        {
            string serializedCommand = _serializer.Serialize(command);
            await _queue.AddMessageAsync(new CloudQueueMessage(serializedCommand));

            return new CommandResult<TResult>(default(TResult), true);
        }

        public Task<CommandResult<NoResult>> DispatchAsync<TCommand>(TCommand command) where TCommand : class
        {
            return DispatchAsync<TCommand, NoResult>(command);
        }

        public Task<CommandResult<TResult>> DispatchAsync<TResult>(ICommand<TResult> command)
        {
            throw new System.NotImplementedException();
        }

        public ICommandExecuter AssociatedExecuter => null;
    }
}
