using PinetreeCQRS.Infrastructure.Commands;
using System;

namespace PinetreeShop.Domain.OrderProcess.Commands
{
    public class NotifyAdmin : CommandBase
    {
        public NotifyAdmin(Guid notificationId) : base(notificationId)
        {
        }
    }

    public class NotifyCustomer : CommandBase
    {
        public NotifyCustomer(Guid notificationId) : base(notificationId)
        {
        }
    }
}
