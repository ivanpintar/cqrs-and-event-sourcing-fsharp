using PinetreeShop.CQRS.Infrastructure.Commands;
using System.Collections.Generic;

namespace PinetreeShop.CQRS.Infrastructure
{
    public interface ICommandQueue
    {
        IEnumerable<ICommand> DeQueueCommands(string queueName);
        void DispatchCommands(string queueName, IEnumerable<ICommand> commands);
    }
}
