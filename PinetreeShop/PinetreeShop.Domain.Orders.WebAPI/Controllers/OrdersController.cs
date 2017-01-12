using PinetreeShop.Domain.Orders.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using static PinetreeShop.Domain.Orders.OrderAggregate;
using static PinetreeCQRS.Infrastructure.Commands;
using static PinetreeCQRS.Infrastructure.Types;
using static PinetreeShop.Domain.Orders.ReadModel;
using Chessie.ErrorHandling;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;

namespace PinetreeShop.Domain.Orders.WebAPI.Controllers
{
    [RoutePrefix("")]
    public class OrdersController : ApiController
    {
        [Route("list"), HttpGet]
        public IHttpActionResult GetOrders()
        {
            var orders = Reader.getOrders().Select(OrderModel.FromDTO).ToList();
            return Ok(orders);
        }

        [Route("cancel"), HttpPost]
        public IHttpActionResult CancelOrder([FromBody] GuidModel model)
        {
            var id = AggregateId.NewAggregateId(model.OrderId);
            var processId = FSharpOption<ProcessId>.Some(ProcessId.NewProcessId(model.ProcessId));
            var cmd = Command.Cancel;
            var envelope = createCommand(id, AggregateVersion.Irrelevant, null, null, processId, cmd);
            QueueCommand(envelope);

            return Ok();
        }

        [Route("ship"), HttpPost]
        public IHttpActionResult ShipOrder([FromBody] GuidModel model)
        {
            var id = AggregateId.NewAggregateId(model.OrderId);
            var processId = FSharpOption<ProcessId>.Some(ProcessId.NewProcessId(model.ProcessId));
            var cmd = Command.Ship;
            var envelope = createCommand(id, AggregateVersion.Irrelevant, null, null, processId, cmd);
            QueueCommand(envelope);

            return Ok();
        }

        [Route("deliver"), HttpPost]
        public IHttpActionResult DeliverOrder([FromBody] GuidModel model)
        {
            var id = AggregateId.NewAggregateId(model.OrderId);
            var processId = FSharpOption<ProcessId>.Some(ProcessId.NewProcessId(model.ProcessId));
            var cmd = Command.Deliver;
            var envelope = createCommand(id, AggregateVersion.Irrelevant, null, null, processId, cmd);
            QueueCommand(envelope);

            return Ok();
        }

        private void QueueCommand(CommandEnvelope<Command> cmd)
        {
            var list = new List<Tuple<QueueName, CommandEnvelope<Command>>> { Tuple.Create(QueueName.NewQueueName("Order"), cmd) };
            var res = PinetreeCQRS.Persistence.SqlServer.Commands.queueCommands(ListModule.OfSeq(list));

            if (res.IsOk)
            {
                return;
            }

            var f = (res as Result<FSharpList<CommandEnvelope<Command>>, IError>.Bad).Item;

            var reasons = f.Select(x => x.ToString()).ToArray();
            var reason = string.Join("; ", reasons);
            throw new Exception(reason);
        }
    }
}
