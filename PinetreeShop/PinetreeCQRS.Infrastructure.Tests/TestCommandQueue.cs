using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PinetreeCQRS.Infrastructure.Commands;

namespace PinetreeCQRS.Infrastructure.Tests
{
    public class TestCommandQueue : ICommandQueue
    {
        private Dictionary<string, List<ICommand>> _commandQueues = new Dictionary<string, List<ICommand>>();
        public List<ICommand> LatestCommands = new List<ICommand>();

        public IEnumerable<ICommand> DeQueueCommands(string queueName)
        {
            if (!_commandQueues.ContainsKey(queueName) || _commandQueues[queueName] == null)
            {
                return Enumerable.Empty<ICommand>();
            }

            var commands = _commandQueues[queueName].ToList();
            _commandQueues[queueName].Clear();
            return commands;
        }

        public void DispatchCommands(string queueName, IEnumerable<ICommand> commands)
        {
            foreach (var cmd in commands)
            {
                if (!_commandQueues.ContainsKey(queueName))
                    _commandQueues[queueName] = new List<ICommand>();

                _commandQueues[queueName].Add(cmd);
                LatestCommands.Add(cmd);
            }
        }

    }
}
