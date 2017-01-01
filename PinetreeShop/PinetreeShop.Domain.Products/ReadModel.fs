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
    { id : Guid
      name : string
      price : decimal
      quantity : int
      reserved : int }

module private DataAccess = 
    type dbSchema = SqlDataProvider< ConnectionStringName="Products" >
    
    let ctx = dbSchema.GetDataContext()
    
    let loadLastEvent() = 
        let r = 
            query { 
                for p in ctx.Dbo.Product do
                    sortByDescending p.LastEventNumber
                    take 1
                    select p.LastEventNumber
            }
        r.FirstOrDefault()
    
    let insertProduct (event : EventEnvelope<Event>) name price = 
        let (AggregateId id) = event.AggregateId
        let newProduct = ctx.Dbo.Product.Create()
        newProduct.Id <- id
        newProduct.Name <- name
        newProduct.Price <- price
        newProduct.Quantity <- 0
        newProduct.Reserved <- 0
        newProduct.LastEventNumber <- event.EventNumber
        try 
            ctx.SubmitUpdates()
            true
        with exn -> false
    
    let updateQuantityAndReserved (event : EventEnvelope<Event>) qDiff rDiff = 
        let (AggregateId id) = event.AggregateId
        
        let update (p : dbSchema.dataContext.``dbo.ProductEntity``) = 
            p.Quantity <- p.Quantity + qDiff
            p.Reserved <- p.Reserved + rDiff
            p.LastEventNumber <- event.EventNumber
        query { 
            for product in ctx.Dbo.Product do
                where (product.Id = id)
        }
        |> Seq.iter update
        try 
            ctx.SubmitUpdates()
            true
        with exn -> false
    
    let loadProducts() = 
        query { 
            for p in ctx.Dbo.Product do
                select { id = p.Id
                         name = p.Name
                         price = p.Price
                         quantity = p.Quantity
                         reserved = p.Reserved }
        }

module Writer = 
    module private Helpers = 
        let updateReserved event = true
        let updateReservedAndQuantity event = true
        let loadEvents eventNumber = 
            let res = loadTypeEvents<Event> eventNumber
            match res with
            | Ok (r, _)-> r
            | Bad _ -> Seq.empty
        
        let handler (event : EventEnvelope<Event>) = 
            match event.Payload with
            | ProductCreated(name, price) -> DataAccess.insertProduct event name price
            | ProductQuantityChanged diff -> DataAccess.updateQuantityAndReserved event diff 0
            | ProductReserved qty -> DataAccess.updateQuantityAndReserved event 0 qty
            | ProductReservationCanceled qty -> DataAccess.updateQuantityAndReserved event 0 -qty
            | ProductPurchased qty -> DataAccess.updateQuantityAndReserved event -qty -qty
            | _ -> true
    
    let handleEvents() = 
        DataAccess.loadLastEvent() |> readAndHandleTypeEvents<bool, Event> Helpers.loadEvents Helpers.handler

module Reader = 
    let getProducts() = DataAccess.loadProducts() |> Seq.toList
