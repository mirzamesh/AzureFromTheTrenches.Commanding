﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AccidentalFish.Commanding;
using AccidentalFish.Commanding.Abstractions;
using AccidentalFish.Commanding.Abstractions.Model;
using InMemoryCommanding.Actors;
using InMemoryCommanding.Commands;
using InMemoryCommanding.Results;
using Microsoft.Extensions.DependencyInjection;

namespace InMemoryCommanding
{
    internal class ConsoleAuditor : ICommandAuditor
    {
        public Task Audit(AuditItem item)
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"Type: {item.CommandType}");
            Console.WriteLine($"Correlation ID: {item.CorrelationId}");
            Console.WriteLine($"Depth: {item.Depth}");
            foreach (KeyValuePair<string, string> enrichedProperty in item.AdditionalProperties)
            {
                Console.WriteLine($"{enrichedProperty.Key}: {enrichedProperty.Value}");
            }
            Console.ForegroundColor = previousColor;
            return Task.FromResult(0);
        }

        public Task AuditWithNoPayload(Guid commandId, string commandType, ICommandDispatchContext dispatchContext)
        {
            throw new NotImplementedException();
        }
    }

    static class ConsoleAuditing
    {
        private static int _counter = -1;

        private static IServiceProvider ServiceProvider = null;

        public static async Task Run(bool auditRootOnly)
        {
            ICommandDispatcher dispatcher = Configure(auditRootOnly);
            ChainCommand command = new ChainCommand();
            await dispatcher.DispatchAsync(command);
            Console.WriteLine("\nPress a key to continue...");
        }

        private static ICommandDispatcher Configure(bool auditRootOnly)
        {
            // we use an enricher that simply updates a counter each time enrichment occurs
            // as enrichment only occurs when the context is created this will start at 0 when the console auditing example is first run and
            // will increment by 1 on each subsequent run
            IReadOnlyDictionary<string, object> Enricher(IReadOnlyDictionary<string, object> existing) => new Dictionary<string, object> {{"Counter", Interlocked.Increment(ref _counter)}};

            ServiceCollection serviceCollection = new ServiceCollection();
            CommandingDependencyResolver dependencyResolver = serviceCollection.GetCommandingDependencyResolver(() => ServiceProvider);

            Options options = new Options
            {
                CommandActorContainerRegistration = type => serviceCollection.AddTransient(type, type),
                Reset = true, // we reset the registry because we allow repeat runs, in a normal app this isn't required
                Enrichers = new[]
                    { new FunctionWrapperCommandDispatchContextEnricher(Enricher) },
                AuditRootCommandOnly = auditRootOnly
            };
            CommandingDependencies.UseCommanding(dependencyResolver, options) 
                .Register<ChainCommand, NoResult, ChainCommandActor>()
                .Register<OutputToConsoleCommand, CountResult, OutputWorldToConsoleCommandActor>()
                .Register<OutputToConsoleCommand, CountResult, OutputBigglesToConsoleCommandActor>();
            CommandingDependencies.UseCommandingAuditor<ConsoleAuditor>(dependencyResolver);
            ServiceProvider = serviceCollection.BuildServiceProvider();
            return ServiceProvider.GetService<ICommandDispatcher>();
        }
    }
}