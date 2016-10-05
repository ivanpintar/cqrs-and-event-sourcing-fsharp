using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinetreeShop.Domain.ShopProcess
{
    public class ShopProcess
    {
        // on TryAddItemToBasket : ReserveProduct
        // on ProductReserved : ConfirmAddItemToBasket
        // on ProductReservationFailed : RevertAddItemToBasket
        // on RemoveItemFromBasket : CancelProductReservation
        // on BasketCheckedOut : CreteOrder
    }
}
