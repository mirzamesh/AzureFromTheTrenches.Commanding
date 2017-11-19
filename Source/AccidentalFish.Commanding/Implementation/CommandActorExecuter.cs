﻿using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using AccidentalFish.Commanding.Abstractions;

namespace AccidentalFish.Commanding.Implementation
{
    internal class CommandActorExecuter : ICommandActorExecuter
    {
        private readonly ConcurrentDictionary<Type, Delegate> _commandActorExecuters = 
            new ConcurrentDictionary<Type, Delegate>();

        public async Task<TResult> ExecuteAsync<TResult>(ICommandActor actor, ICommand<TResult> command, TResult previousResult)
        {
            // we compile this expression to enable command actors to be written with a strongly typed
            // command type syntax e.g.:
            //  class MyCommandActor : ICommandActor<MyCommand, MyResult>
            // Without this command actors would need to be of the form:
            //  class MyCommandActor : ICommandActor<ICommand<MyResult>>
            // Which would lead to lots of casting inside actors. During registration of commands we can guarantee
            // type safety.

            Delegate dlg = _commandActorExecuters.GetOrAdd(actor.GetType(), (actorType) =>
            {
                Type castCommandActor = typeof(ICommandActor<,>);
                Type[] typeArgs = new[] { command.GetType(), typeof(TResult) };
                Type genericType = castCommandActor.MakeGenericType(typeArgs);

                MethodInfo methodInfo = genericType.GetRuntimeMethod("ExecuteAsync", typeArgs);
                ParameterExpression actorParameter = Expression.Parameter(typeof(ICommandActor));
                ParameterExpression commandParameter = Expression.Parameter(typeof(ICommand<TResult>));
                ParameterExpression previousResultParameter = Expression.Parameter(typeof(TResult));

                var lambda = Expression.Lambda<Func<ICommandActor, ICommand<TResult>, TResult, Task<TResult>>>(
                    Expression.Call(Expression.Convert(actorParameter, genericType),
                        methodInfo,
                        Expression.Convert(commandParameter, command.GetType()), previousResultParameter),
                    actorParameter, commandParameter, previousResultParameter);
                Func<ICommandActor, ICommand<TResult>, TResult, Task<TResult>> executer = lambda.Compile();
                return executer;
            });

            Func<ICommandActor, ICommand<TResult>, TResult, Task<TResult>> func = (Func<ICommandActor, ICommand<TResult>, TResult, Task<TResult>>)dlg;
            return await func(actor, command, previousResult);
        }
    }
}