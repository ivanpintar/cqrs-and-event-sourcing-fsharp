using PinetreeCQRS.Infrastructure.Commands;
using System.Collections.Generic;

namespace PinetreeCQRS.Infrastructure
{
    public interface ICommandQueue
    {
        IEnumerable<ICommand> DeQueueCommands(string queueName);
        void DispatchCommands(string queueName, IEnumerable<ICommand> commands);
    }
}
