module PinetreeShop.Domain.Products.ReadModel

open PinetreeCQRS.Infrastructure.Types
open PinetreeCQRS.Infrastructure.Events
open PinetreeCQRS.Persistence.SqlServer
open PinetreeShop.Domain.Products.ProductAggregate
open FSharp.Data.Sql
open System.Linq
open System
open System.Diagnostics
open Chessie.ErrorHandling

type ProductDTO = 
    { Id : Guid
      Name : string
      Price : decimal
      Quantity : int
      Reserved : int }

module private DataAccess = 
    type dbSchema = SqlDataProvider< ConnectionStringName="Products" >
    
    let ctx = dbSchema.GetDataContext()
    
    let loadLastEvent() = 
        let r = 
            query { 
                for p in ctx.Products.Product do
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
    
    let insertProduct (event : EventEnvelope<Event>) name price = 
        let (AggregateId id) = event.AggregateId
        let newProduct = ctx.Products.Product.Create()
        newProduct.Id <- id
        newProduct.Name <- name
        newProduct.Price <- price
        newProduct.Quantity <- 0
        newProduct.Reserved <- 0
        newProduct.LastEventNumber <- event.EventNumber
        try 
            ctx.SubmitUpdates()
            ok event
        with ex -> Bad [ Error ex.Message :> IError ]
    
    let updateQuantityAndReserved (event : EventEnvelope<Event>) qDiff rDiff = 
        let (AggregateId id) = event.AggregateId        
        let update (p : dbSchema.dataContext.``Products.ProductEntity``) = 
            p.Quantity <- p.Quantity + qDiff
            p.Reserved <- p.Reserved + rDiff
            p.LastEventNumber <- event.EventNumber
        query { 
            for product in ctx.Products.Product do
                where (product.Id = id)                
        }
        |> Seq.toList
        |> List.iter update
        try 
            ctx.SubmitUpdates()
            ok event
        with ex -> Bad [ Error ex.Message :> IError ]
    
    let loadProducts() = 
        query { 
            for p in ctx.Products.Product do
                select { Id = p.Id
                         Name = p.Name
                         Price = p.Price
                         Quantity = p.Quantity
                         Reserved = p.Reserved }
        }

module Writer = 
    module private Helpers = 
        let updateReserved event = true
        let updateReservedAndQuantity event = true
        
        let handler (event : EventEnvelope<Event>) = 
            match event.Payload with
            | ProductCreated(name, price) -> DataAccess.insertProduct event name price
            | ProductQuantityChanged diff -> DataAccess.updateQuantityAndReserved event diff 0
            | ProductReserved qty -> DataAccess.updateQuantityAndReserved event 0 qty
            | ProductReservationCanceled qty -> DataAccess.updateQuantityAndReserved event 0 -qty
            | ProductPurchased qty -> DataAccess.updateQuantityAndReserved event -qty -qty
            | _ -> Ok(event, [ Error "Skipped" :> IError ])
    
    let handleEvents() = 
        let events = DataAccess.loadLastEvent() >>= Events.loadTypeEvents productCategory
        Seq.map Helpers.handler <!> events

module Reader = 
    let getProducts() = DataAccess.loadProducts() |> Seq.toList
