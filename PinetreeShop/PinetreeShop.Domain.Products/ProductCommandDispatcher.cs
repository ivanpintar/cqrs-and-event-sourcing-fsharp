using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.CQRS.Infrastructure.Repositories;
using PinetreeShop.Domain.Products.Commands;
using PinetreeShop.Domain.Shared.Exceptions;
using System;

namespace PinetreeShop.Domain.Products
{
    public class ProductCommandDispatcher : CommandDispatcher
    {
        public ProductCommandDispatcher(IAggregateRepository aggregateRepository) : base(aggregateRepository)
        {
            RegisterHandler(Create);
            RegisterHandler(SetQuantity);
            RegisterHandler(AddToStock);
            RegisterHandler(RemoveFromStock);
            RegisterHandler(Reserve);
            RegisterHandler(CancelReservation);
        }

        private Func<ProductAggregate, CreateProduct, ProductAggregate> Create = (product, command) =>
        {
            if (product != null)
            {
                throw new AggregateExistsException(product.AggregateId, "Product already exists");
            }

            return ProductAggregate.Create(command as CreateProduct);
        };

        private Func<ProductAggregate, SetProductQuantity, ProductAggregate> SetQuantity = (product, command) =>
        {
            (product as ProductAggregate).SetQuantity(command as SetProductQuantity);
            return product;
        };

        private Func<ProductAggregate, AddProductToStock, ProductAggregate> AddToStock = (product, command) =>
        {
            (product as ProductAggregate).AddToStock(command as AddProductToStock);
            return product;
        };

        private Func<ProductAggregate, RemoveProductFromStock, ProductAggregate> RemoveFromStock = (product, command) =>
        {
            (product as ProductAggregate).RemoveFromStock(command as RemoveProductFromStock);
            return product;
        };

        private Func<ProductAggregate, ReserveProduct, ProductAggregate> Reserve = (product, command) =>
        {
            (product as ProductAggregate).Reserve(command as ReserveProduct);
            return product;
        };

        private Func<ProductAggregate, CancelProductReservation, ProductAggregate> CancelReservation = (product, command) =>
        {
            (product as ProductAggregate).CancelReservation(command as CancelProductReservation);
            return product;
        };
    }
}
