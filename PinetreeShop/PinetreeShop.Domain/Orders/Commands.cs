using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.Domain.Types;
using System;
using System.Collections.Generic;

namespace PinetreeShop.Domain.Orders.Commands
{
    public class CreateOrder : CommandBase
    {
        public Guid BasketId { get; set; }
        public IEnumerable<OrderLine> Lines { get; set; }

        public CreateOrder(Guid aggregateId, Guid basketId, IEnumerable<OrderLine> lines) : base(aggregateId)
        {
            BasketId = basketId;
            Lines = lines;
        }
    }

    public class CancelOrder : CommandBase
    {
        public CancelOrder(Guid aggregateId) : base(aggregateId)
        {
        }
    }

    public class ShipOrder : CommandBase
    {
        public ShipOrder(Guid aggregateId) : base(aggregateId)
        {
        }
    }

    public class DeliverOrder : CommandBase
    {
        public DeliverOrder(Guid aggregateId) : base(aggregateId)
        {
        }

    }
}
