module PinetreeShop.Domain.Orders.OrderAggregate

open System
open PinetreeCQRS.Infrastructure.Types
open PinetreeCQRS.Infrastructure.Commands
open PinetreeCQRS.Infrastructure.Events
open System
open Chessie.ErrorHandling
open PinetreeCQRS.Infrastructure.Validation

type OrderError = 
    | ValidationError of string
    interface IError
    override e.ToString() = sprintf "%A" e

type BasketId = 
    | BasketId of Guid

type ProductId = 
    | ProductId of Guid

type ShippingAddress = 
    | ShippingAddress of string

type OrderState = 
    | NotCreated
    | Pending
    | ReadyForShipping
    | Shipped
    | Delivered
    | Cancelled

type OrderLine = 
    { ProductId : ProductId
      ProductName : string
      Price : decimal
      Quantity : int }

type Command = 
    | Create of BasketId * ShippingAddress
    | AddOrderLine of OrderLine
    | PrepareForShipping
    | Ship
    | Deliver
    | Cancel

type Event = 
    | OrderCreated of BasketId * ShippingAddress
    | OrderLineAdded of OrderLine
    | OrderReadyForShipping
    | OrderShipped
    | OrderDelivered
    | OrderCancelled
    interface IEvent

module private Handlers = 
    type State = 
        { OrderState : OrderState
          OrderLines : OrderLine list
          ShippingAddress : ShippingAddress }
        static member Zero = 
            { OrderState = OrderState.NotCreated
              OrderLines = []
              ShippingAddress = ShippingAddress "" }
    
    let applyEvent state event = 
        match event with
        | OrderCreated(basketId, shippingAddress) -> 
            { state with OrderState = OrderState.Pending
                         ShippingAddress = shippingAddress }
        | OrderLineAdded orderLine -> { state with OrderLines = orderLine :: state.OrderLines }
        | OrderReadyForShipping -> { state with OrderState = OrderState.ReadyForShipping }
        | OrderShipped -> { state with OrderState = OrderState.Shipped }
        | OrderDelivered -> { state with OrderState = OrderState.Delivered }
        | OrderCancelled -> { state with OrderState = OrderState.Cancelled }
    
    module private Validate = 
        let inCase predicate error value = 
            let result = predicate value
            match result with
            | false -> ok value
            | true -> Bad [ ValidationError error :> IError ]
        
        module private Helpers = 
            let notEmptyShippingAddress (ShippingAddress sa) = 
                inCase (fun sa -> String.IsNullOrWhiteSpace(sa)) "Shipping Address cannot be empty" sa
            let isOrderState s os = 
                inCase (fun s -> s.OrderState <> os) (sprintf "Wrong Order state %A" s.OrderState) s
            let notOrderState s os = 
                inCase (fun s -> s.OrderState = os) (sprintf "Wrong Order state %A" s.OrderState) s
            let hasOrderLines ol = inCase (fun ol -> ol = []) "No order lines" ol
            let canCreate s = isOrderState s NotCreated
            let created s = notOrderState s NotCreated
        
        let canCreate (s, sa) = Helpers.canCreate s <* Helpers.notEmptyShippingAddress sa
        let canAddLine s = Helpers.isOrderState s Pending
        let canPrepForShipping s = Helpers.isOrderState s Pending <* Helpers.hasOrderLines s.OrderLines
        let canShip s = Helpers.isOrderState s ReadyForShipping 
        let canDeliver s = Helpers.isOrderState s Shipped
        let canCancel s = Helpers.notOrderState s NotCreated <* Helpers.notOrderState s Cancelled <* Helpers.notOrderState s Delivered
    
    let executeCommand (state : State) command = 
        match command with
        | Create(basketId, shippingAddress) -> 
            Validate.canCreate (state, shippingAddress) <?> [ OrderCreated(basketId, shippingAddress) ]
        | AddOrderLine(orderLine) -> Validate.canAddLine state <?> [ OrderLineAdded(orderLine) ]
        | PrepareForShipping -> Validate.canPrepForShipping (state) <?> [ OrderReadyForShipping ]
        | Ship -> Validate.canShip (state) <?> [ OrderShipped ]
        | Deliver -> Validate.canDeliver (state) <?> [ OrderDelivered ]
        | Cancel -> Validate.canCancel (state) <?> [ OrderCancelled ]

let makeProductCommandHandler = 
    makeCommandHandler { Zero = Handlers.State.Zero
                         ApplyEvent = Handlers.applyEvent
                         ExecuteCommand = Handlers.executeCommand }
