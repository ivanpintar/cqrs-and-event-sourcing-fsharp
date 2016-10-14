using PinetreeShop.CQRS.Infrastructure;
using PinetreeShop.CQRS.Infrastructure.Commands;
using PinetreeShop.Domain.Products.Commands;
using PinetreeShop.Domain.Shared.Exceptions;
using System;

namespace PinetreeShop.Domain.Products
{
    public static class CommandHandlers
    {
        public static Func<ProductAggregate, CreateProduct, ProductAggregate> Create = (product, command) =>
        {
            if (product != null)
            {
                throw new AggregateExistsException(product.AggregateId, "Product already exists");
            }

            return ProductAggregate.Create(command as CreateProduct);
        };

        public static Func<ProductAggregate, ChangeProductQuantity, ProductAggregate> ChangeQuantity = (product, command) =>
        {
            (product as ProductAggregate).ChangeQuantity(command as ChangeProductQuantity);
            return product;
        };

        public static Func<ProductAggregate, ReserveProduct, ProductAggregate> Reserve = (product, command) =>
        {
            (product as ProductAggregate).Reserve(command as ReserveProduct);
            return product;
        };

        public static Func<ProductAggregate, CancelProductReservation, ProductAggregate> CancelReservation = (product, command) =>
        {
            (product as ProductAggregate).CancelReservation(command as CancelProductReservation);
            return product;
        };
    }
}
