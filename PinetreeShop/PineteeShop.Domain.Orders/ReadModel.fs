module PinetreeShop.Domain.Orders.ReadModel

open PinetreeCQRS.Infrastructure.Types
open PinetreeCQRS.Infrastructure.Events
open PinetreeCQRS.Persistence.SqlServer
open PinetreeShop.Domain.Orders.OrderAggregate
open FSharp.Data.Sql
open System.Linq
open System
open System.Diagnostics
open Chessie.ErrorHandling

type OrderLineDTO = {
    ProductId:Guid
    ProductName:string
    Price:decimal
    Quantity:int
}

type OrderDTO = 
    { Id : Guid
      State : string
      ShippingAddress : string
      OrderLines : OrderLineDTO list }


module private DataAccess = 
    type dbSchema = SqlDataProvider< ConnectionStringName="Orders" >
    
    let ctx = dbSchema.GetDataContext()
    
    let loadLastEvent() = 
        let r = 
            query { 
                for p in ctx.Orders.Order do
                    sortByDescending p.LastEventNumber
                    take 1
                    select p.LastEventNumber
            }
            |> Seq.toList
        try 
            match r with
            | [] -> ok 0
            | _ -> ok (r.Head)
        with ex -> Bad [ Error ex.Message :> IError ]
    
    let insertOrder (event : EventEnvelope<Event>) (ShippingAddress shippingAddress) = 
        let (AggregateId id) = event.AggregateId
        let newOrder = ctx.Orders.Order.Create(event.EventNumber, shippingAddress, "Pending")
        try 
            ctx.SubmitUpdates()
            ok event
        with ex -> Bad [ Error ex.Message :> IError ]
    
    let addOrderLine (event : EventEnvelope<Event>) (orderLine:OrderLine) = 
        let (AggregateId id) = event.AggregateId        
       
        query { 
            for product in ctx.Orders.Order do
                where (product.Id = id)                
        }
        |> Seq.iter (fun f -> f.LastEventNumber <- event.EventNumber)
        try 
            ctx.SubmitUpdates()
            ok event
        with ex -> Bad [ Error ex.Message :> IError ]

    let setState (event:EventEnvelope<Event>) state =
        let (AggregateId id) = event.AggregateId        
        query { 
            for product in ctx.Orders.Order do
                where (product.Id = id)                
        }
        |> Seq.iter (fun f -> f.LastEventNumber <- event.EventNumber
                              f.State <- state)
        try 
            ctx.SubmitUpdates()
            ok event
        with ex -> Bad [ Error ex.Message :> IError ]
    
    let loadOrders() = 
        let toOrderLineDTO (_, ol:dbSchema.dataContext.``Orders.LineEntity``)  : OrderLineDTO = {
            ProductId = ol.ProductId
            ProductName = ol.ProductName
            Price = ol.Price
            Quantity = ol.Quantity
        }
        let toOrderDTO (o:dbSchema.dataContext.``Orders.OrderEntity``) orderLines : OrderDTO = {
            Id = o.Id
            State = o.State
            ShippingAddress = o.ShippingAddress
            OrderLines = List.map toOrderLineDTO orderLines
        }

        query { 
            for o in ctx.Orders.Order do
                for ol in o.``Orders.Line by Id`` do
                select (o, ol)
        } 
        |> Seq.toList
        |> List.groupBy (fun (o, ol) -> o)
        |> List.map (fun (key, values) -> toOrderDTO key values)

module Writer = 
    module private Helpers = 
        let updateReserved event = true
        let updateReservedAndQuantity event = true
        
        let handler (event : EventEnvelope<Event>) = 
            match event.Payload with
            | OrderCreated (_, shippingAddress) -> DataAccess.insertOrder event shippingAddress
            | OrderLineAdded orderLine -> DataAccess.addOrderLine event orderLine
            | OrderReadyForShipping -> DataAccess.setState event "ReadyForShipping"
            | OrderCancelled -> DataAccess.setState event "Cancelled"
            | OrderShipped -> DataAccess.setState event "Shipped"
            | OrderDelivered -> DataAccess.setState event "Delivered"
    
    let handleEvents() = 
        let events = DataAccess.loadLastEvent() >>= Events.loadTypeEvents orderCategory
        Seq.map Helpers.handler <!> events

module Reader = 
    let getOrders() = DataAccess.loadOrders() |> Seq.toList
