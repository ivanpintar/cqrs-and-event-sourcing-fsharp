using System;
using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.Events;
using PinetreeShop.Domain.Baskets.Events;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.Domain.Products.Events;

namespace PinetreeShop.Domain.ShoppingProcess
{
    public class ShoppingProcessEventHandler
    {
        private IProcessManagerRepository _processManagerRepository;

        public ShoppingProcessEventHandler(IProcessManagerRepository processManagerRepository)
        {
            _processManagerRepository = processManagerRepository;
        }        
    }
}
