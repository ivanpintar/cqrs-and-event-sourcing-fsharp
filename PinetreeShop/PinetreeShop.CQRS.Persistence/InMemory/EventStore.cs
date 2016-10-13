using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Events;
using System.Collections.Generic;
using System.Linq;

namespace PinetreeShop.CQRS.Persistence.InMemory
{
    public class TestEventStore : IEventStore
    {
        private List<IEvent> _events = new List<IEvent>();
        private List<IEvent> _latestEvents;


        private List<ICommand> _commands = new List<ICommand>();
        private List<ICommand> _latestCommands;

        public IEnumerable<IEvent> Events { get { return _events; } }

        public IEnumerable<ICommand> Commands { get { return _commands; } }


        public IEnumerable<IEvent> GetLatestEvents()
        {
            return _latestEvents;
        }

        public IEnumerable<ICommand> GetLatestCommands()
        {
            return _latestCommands;
        }

        public void CommitEvents(IEnumerable<IEvent> events)
        {
            _events.AddRange(events);
            _latestEvents = events.ToList();
        }

        public void DispatchCommands(IEnumerable<ICommand> commands)
        {
            _commands.AddRange(commands);
            _latestCommands = commands.ToList();
        }

        public void AddPreviousEvents(IEnumerable<IEvent> events)
        {
            _events.AddRange(events);
        }
    }
}
