using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinetreeShop.CQRS.Infrastructure
{
    public class CommandDispatcher
    {
        private Dictionary<Type, Func<object, IAggregate>> _routes;
        private IDomainRepository _domainRepository;
        private readonly IEnumerable<Action<ICommand>> _preExecutionPipe;
        private readonly IEnumerable<Action<object>> _postExecutionPipe;

        public CommandDispatcher(
            IDomainRepository domainRepository,
            IEnumerable<Action<ICommand>> preExecutionPipe,
            IEnumerable<Action<object>> postExecutionPipe)
        {
            _domainRepository = domainRepository;
            _preExecutionPipe = preExecutionPipe ?? Enumerable.Empty<Action<ICommand>>();
            _postExecutionPipe = postExecutionPipe ?? Enumerable.Empty<Action<object>>();
            _routes = new Dictionary<Type, Func<object, IAggregate>>();
        }

        public void RegisterHander<TCommand>(IHandle<TCommand> handler) where TCommand : class, ICommand
        {
            _routes.Add(typeof(TCommand), command => handler.Handle(command as TCommand));
        }

        public void ExecuteCommand<TCommand>(TCommand command) where TCommand : ICommand
        {
            var commandType = command.GetType();

            RunPreExecutionPipe(command);

            if (!_routes.ContainsKey(commandType))
            {
                throw new ApplicationException($"Missing handler for {commandType.Name}");
            }

            var aggregate = _routes[commandType](command);
            var savedEvents = _domainRepository.Save(aggregate);

            RunPostExecutionPipe(savedEvents);
        }

        private void RunPreExecutionPipe<TCommand>(TCommand command) where TCommand : ICommand
        {
            foreach (var action in _preExecutionPipe)
            {
                action(command);
            }
        }

        private void RunPostExecutionPipe(IEnumerable<IEvent> savedEvents)
        {
            foreach (var savedEvent in savedEvents)
            {
                foreach(var action in _postExecutionPipe)
                {
                    action(savedEvent);
                }
            }
        }
    }
}
