using System;

namespace PinetreeShop.CQRS.Infrastructure.Commands
{
    public interface ICommand
    {
        Guid AggregateId { get; }
        Metadata Metadata { get; }
    }

    public class CommandBase : ICommand
    {
        public Guid AggregateId { get; private set; }
        public Metadata Metadata { get; private set; }

        public CommandBase(Guid aggregateId)
        {
            AggregateId = aggregateId;
            Metadata = new Metadata();
        }
    }

    public class RevertCommandBase : CommandBase
    {
        public string Reason { get; private set; }

        public RevertCommandBase(Guid aggregateId, string reason) : base(aggregateId)
        {
            Reason = reason;
        }
    }

    public class Metadata
    {
        public Guid CommandId { get; private set; }
        public Guid CausationId { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid ProcessId { get; set; }

        public Metadata()
        {
            CommandId = Guid.NewGuid();
            ProcessId = Guid.NewGuid();
            CausationId = CommandId;
            CorrelationId = CommandId;
        }
    }
}
