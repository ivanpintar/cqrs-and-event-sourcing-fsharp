using System;

namespace PinetreeShop.CQRS.Infrastructure
{
    public interface ICommand
    {
        Guid AggregateId { get; }
    }

    public class CommandBase : ICommand
    {
        public Guid AggregateId { get; private set; }

        public CommandBase(Guid aggregateId)
        {
            AggregateId = aggregateId;
        }
    }
}
