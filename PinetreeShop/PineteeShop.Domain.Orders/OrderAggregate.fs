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
          ShippingAddress : ShippingAddress
          Created : bool }
        static member Zero = 
            { OrderState = OrderState.Pending
              OrderLines = []
              ShippingAddress = ShippingAddress ""
              Created = false }
    
    let applyEvent state event = 
        match event with
        | OrderCreated(basketId, shippingAddress) -> 
            { state with OrderState = OrderState.Pending
                         ShippingAddress = shippingAddress
                         Created = true }
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
            let notCreated = inCase (fun s -> s.Created) "Order already created"
            let created = inCase (fun s -> not s.Created) "Order not created"
            let notEmptyShippingAddress (ShippingAddress sa) = 
                inCase (fun sa -> String.IsNullOrWhiteSpace(sa)) "Shipping Address cannot be empty" sa
            let isPending s = 
                inCase (fun s -> s.OrderState <> OrderState.Pending) (sprintf "Wrong Order state %A" s.OrderState) s
        
        let canCreate (s, sa) = Helpers.notCreated s <* Helpers.notEmptyShippingAddress sa
        let canAddLine s = Helpers.created s <* Helpers.isPending s
        let canPrepForShipping s = failwith "not implemented"
        let isReadyForShipping s = failwith "not implemented"
        let isShipped s = failwith "not implemented"
        let canCancel s = failwith "not implemented"
    
    let executeCommand (state : State) command = 
        match command with
        | Create(basketId, shippingAddress) -> 
            Validate.canCreate (state, shippingAddress) <?> [ OrderCreated(basketId, shippingAddress) ]
        | AddOrderLine(orderLine) -> Validate.canAddLine state <?> [ OrderLineAdded(orderLine) ]
        | PrepareForShipping -> Validate.canPrepForShipping (state) <?> [ OrderReadyForShipping ]
        | Ship -> Validate.isReadyForShipping (state) <?> [ OrderShipped ]
        | Deliver -> Validate.isShipped (state) <?> [ OrderDelivered ]
        | Cancel -> Validate.canCancel (state) <?> [ OrderCancelled ]

let makeProductCommandHandler = 
    makeCommandHandler { Zero = Handlers.State.Zero
                         ApplyEvent = Handlers.applyEvent
                         ExecuteCommand = Handlers.executeCommand }
