using System;

namespace PinetreeShop.CQRS.Infrastructure.CommandsAndEvents
{
    public interface ICommand
    {
        Guid AggregateId { get; }
    }

    public class CommandBase : ICommand
    {
        public Guid AggregateId { get; set; }

        public CommandBase(Guid aggregateId)
        {
            AggregateId = aggregateId;
        }
    }
}
