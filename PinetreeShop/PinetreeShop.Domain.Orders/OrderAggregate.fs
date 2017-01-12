module PinetreeShop.Domain.Orders.OrderAggregate

open System
open PinetreeCQRS.Infrastructure.Types
open PinetreeCQRS.Infrastructure.Commands
open PinetreeCQRS.Infrastructure.Events
open System
open Chessie.ErrorHandling
open PinetreeCQRS.Infrastructure.Validation

let orderCategory = Category "Order"
let orderQueueName = QueueName "Order"

type OrderError = 
    | ValidationError of string
    interface IError
    override e.ToString() = sprintf "%A" e

type OrderState = 
    | NotCreated
    | Pending
    | ReadyForShipping
    | Shipped
    | Delivered
    | Cancelled
    
type Command = 
    | Create of BasketId * ShippingAddress * OrderLine list
    | ReserveOrderLineProduct of ProductId
    | PrepareForShipping
    | Ship
    | Deliver
    | Cancel 
    interface ICommand

type Event = 
    | OrderCreated of BasketId * ShippingAddress * OrderLine list
    | OrderLineProductReserved of ProductId
    | OrderReadyForShipping
    | OrderShipped
    | OrderDelivered
    | OrderCancelled
    interface IEvent

module private Handlers = 
    type State = 
        { OrderState : OrderState
          OrderLines : (OrderLine * bool) list
          ShippingAddress : ShippingAddress }
        static member Zero = 
            { OrderState = OrderState.NotCreated
              OrderLines = []
              ShippingAddress = ShippingAddress "" }
    
    let markReserved (ol, res) productId =
        if (ol.ProductId = productId) then (ol, true)
        else (ol, res)
        

    let applyEvent state event = 
        match event with
        | OrderCreated(basketId, shippingAddress, orderLines) -> 
            { state with OrderState = OrderState.Pending
                         ShippingAddress = shippingAddress
                         OrderLines = List.map (fun o -> (o, false)) orderLines }
        | OrderLineProductReserved productId -> { state with OrderLines = List.map (fun ol -> markReserved ol productId) state.OrderLines }
        | OrderReadyForShipping -> { state with OrderState = OrderState.ReadyForShipping }
        | OrderShipped -> { state with OrderState = OrderState.Shipped }
        | OrderDelivered -> { state with OrderState = OrderState.Delivered }
        | OrderCancelled -> { state with OrderState = OrderState.Cancelled }
    
    module private Validate = 
        let inCase predicate error value = 
            if predicate value then  Bad [ ValidationError error :> IError ]
            else ok value
        
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
        
        let canCreate (s, sa, ol) = Helpers.canCreate s <* Helpers.notEmptyShippingAddress sa <* Helpers.hasOrderLines ol
        let canMarkLineReserved s = Helpers.isOrderState s Pending
        let canPrepForShipping s = Helpers.isOrderState s Pending <* Helpers.hasOrderLines s.OrderLines
        let canShip s = Helpers.isOrderState s ReadyForShipping 
        let canDeliver s = Helpers.isOrderState s Shipped
        let canCancel s = Helpers.notOrderState s NotCreated <* Helpers.notOrderState s Cancelled <* Helpers.notOrderState s Delivered
    
    let executeCommand (state : State) command = 
        match command with
        | Create(basketId, shippingAddress, orderLines) -> 
            Validate.canCreate (state, shippingAddress, orderLines) <?> [ OrderCreated(basketId, shippingAddress, orderLines) ]
        | ReserveOrderLineProduct productId -> 
            let noOfOrders = state.OrderLines.Length
            let noOfReservedItems = List.filter (fun (ol, res) -> res = true) state.OrderLines |> List.length
            let events = 
                if (noOfOrders = noOfReservedItems + 1) then [ OrderLineProductReserved productId; OrderReadyForShipping ]
                else [ OrderLineProductReserved productId ]
            Validate.canMarkLineReserved state <?> events
        | PrepareForShipping -> Validate.canPrepForShipping (state) <?> [ OrderReadyForShipping ]
        | Ship -> Validate.canShip (state) <?> [ OrderShipped ]
        | Deliver -> Validate.canDeliver (state) <?> [ OrderDelivered ]
        | Cancel -> Validate.canCancel (state) <?> [ OrderCancelled ]

let makeOrderCommandHandler = 
    makeCommandHandler { Zero = Handlers.State.Zero
                         ApplyEvent = Handlers.applyEvent
                         ExecuteCommand = Handlers.executeCommand }
