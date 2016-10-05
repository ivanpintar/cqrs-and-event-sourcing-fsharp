using System;

namespace PinetreeShop.CQRS.Infrastructure.Commands
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

    public class RevertCommandBase : CommandBase
    {
        public string Reason { get; set; }

        public RevertCommandBase(Guid aggregateId, string reason) : base(aggregateId)
        {
            Reason = reason;
        }
    }
}
