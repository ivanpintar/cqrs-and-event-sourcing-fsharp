using PinetreeShop.CQRS.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PinetreeShop.Domain.Baskets.Events;
using PinetreeShop.Domain.Products.Commands;
using PinetreeShop.Domain.Products.Events;
using PinetreeShop.Domain.Baskets.Commands;

namespace PinetreeShop.Domain.ShoppingProcess
{
    public class ShoppingProcessManager : ProcessManagerBase
    {
        private Guid _basketId;

        public ShoppingProcessManager()
        {
            
        }
    }
}
