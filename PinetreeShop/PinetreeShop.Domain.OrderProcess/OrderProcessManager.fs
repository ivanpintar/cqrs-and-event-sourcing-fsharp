module PinetreeShop.Domain.OrderProcess.OrderProcessManager

module Order = PinetreeShop.Domain.Orders.OrderAggregate
module Basket = PinetreeShop.Domain.Baskets.BasketAggregate
module Product = PinetreeShop.Domain.Products.ProductAggregate

open PinetreeCQRS.Infrastructure.Types
open PinetreeCQRS.Infrastructure.Events
open Chessie.ErrorHandling
open System

type ProcessError = 
    | ProcessError of string
    interface IError
    override e.ToString() = sprintf "%A" e

module private Handlers = 
    type ProcessState = 
        | NotCreated
        | BasketCheckedOut
        | OrderPending
        | AllItemsAdded
        | ItemReservationFailed
        | OrderCancelled
        | OrderReadyForShipping
        | OrderShipped
        | OrderDelivered
    
    type Reserved = bool
    
    type State = 
        { OrderLines : Map<ProductId, OrderLine>
          Reserved : ProductId list
          ProcessState : ProcessState
          OrderId : OrderId }
        static member Zero = 
            { OrderLines = Map.empty
              Reserved = []
              ProcessState = NotCreated
              OrderId = OrderId Guid.Empty }
    
    let itemToOrderLine (item : BasketItem) = 
        { ProductId = item.ProductId
          ProductName = item.ProductName
          Price = item.Price
          Quantity = item.Quantity }
    
    let applyEvent state (event : EventEnvelope<IEvent>) = 
        match event.Payload with
        | :? Basket.Event as be -> 
            match be with
            | Basket.BasketCheckedOut(_, items) -> 
                { state with OrderLines = 
                                 List.map (fun (i : BasketItem) -> i.ProductId, (itemToOrderLine i)) items |> Map.ofList }
            | _ -> state
        | :? Product.Event as pe -> 
            match pe with
            | Product.ProductReserved qty -> 
                let (AggregateId productGuid) = event.AggregateId
                let productId = ProductId productGuid
                { state with Reserved = productId :: state.Reserved }
            | _ -> state
        | :? Order.Event as oe -> 
            match oe with
            | Order.OrderCreated _ -> 
                { state with ProcessState = OrderPending
                             OrderId = OrderId.FromAggregateId(event.AggregateId) }
            | Order.OrderReadyForShipping -> { state with ProcessState = OrderReadyForShipping }
            | Order.OrderCancelled -> { state with ProcessState = OrderCancelled }
            | Order.OrderShipped -> { state with ProcessState = OrderShipped }
            | Order.OrderDelivered -> { state with ProcessState = OrderDelivered }
            | _ -> state
        | _ -> state
        
    let createCommand queueName aggregateId processId (event : EventEnvelope<IEvent>) command = 
        let cmd = 
            { AggregateId = aggregateId
              CommandId = Guid.NewGuid() |> CommandId
              Payload = command
              ProcessId = Some(processId)
              CausationId = event.CausationId
              CorrelationId = event.CorrelationId
              ExpectedVersion = AggregateVersion.Irrelevant }
        (queueName, cmd)
    
    let processEvent state (event : EventEnvelope<IEvent>) : Result<(QueueName * CommandEnvelope<ICommand>) list, IError> = 
        match event.ProcessId with
        | Some pid -> 
            match event.Payload with
            | :? Basket.Event as be -> 
                match be with
                | Basket.BasketCheckedOut(address, items) -> 
                    let orderLines = List.map itemToOrderLine items
                    let command = 
                        Order.Create(BasketId.FromAggregateId(event.AggregateId), address, orderLines) :> ICommand 
                        |> createCommand Order.orderQueueName (Guid.NewGuid() |> AggregateId) pid event
                    ok [ command ]
                | _ -> ok []
            | :? Product.Event as pe -> 
                match pe with
                | Product.ProductReserved qty -> 
                    let productId = ProductId.FromAggregateId(event.AggregateId)
                    let (OrderId orderGuid) = state.OrderId
                    let command = 
                        Order.ReserveOrderLineProduct productId :> ICommand 
                        |> createCommand Order.orderQueueName (AggregateId orderGuid) pid event
                    ok [ command ]
                | _ -> ok []
            | :? Order.Event as oe -> 
                match oe with
                | Order.OrderCreated (_,_,orderLines) -> 
                    orderLines
                    |> List.map 
                           (fun ol -> 
                           let (ProductId productGuid) = ol.ProductId
                           Product.Reserve(ol.Quantity) :> ICommand 
                           |> createCommand Product.productQueueName (AggregateId productGuid) pid event)
                    |> ok
                | Order.OrderCancelled -> 
                    state.Reserved
                    |> List.map 
                           (fun i -> 
                           let ol = state.OrderLines.[i]
                           let (ProductId productGuid) = ol.ProductId
                           Product.CancelReservation(ol.Quantity) :> ICommand 
                           |> createCommand Product.productQueueName (AggregateId productGuid) pid event)
                    |> ok
                | Order.OrderShipped -> 
                    state.Reserved
                    |> List.map 
                           (fun i -> 
                           let ol = state.OrderLines.[i]
                           let (ProductId productGuid) = ol.ProductId
                           Product.PurchaseReserved(ol.Quantity) :> ICommand 
                           |> createCommand Product.productQueueName (AggregateId productGuid) pid event)
                    |> ok
                | _ -> ok []
            | _ -> ok []
        | _ -> ok []

let makeOrderProcessHandler = 
    makeEventProcessor { Zero = Handlers.State.Zero
                         ApplyEvent = Handlers.applyEvent
                         ProcessEvent = Handlers.processEvent }
