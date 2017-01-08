module PinetreeShop.Domain.Baskets.Tests.RemoveItem

open PinetreeShop.Domain.Tests.TestBase
open PinetreeShop.Domain.Baskets.BasketAggregate
open PinetreeShop.Domain.Baskets.Tests.Base
open PinetreeCQRS.Infrastructure.Commands
open PinetreeCQRS.Infrastructure.Events
open PinetreeCQRS.Infrastructure.Types
open Xunit
open System
open Chessie.ErrorHandling

let aggregateId = Guid.NewGuid() |> AggregateId
let productId = Guid.NewGuid() |> ProductId

let item : BasketItem = 
    { ProductId = productId
      ProductName = "Test"
      Price = 2m
      Quantity = 20 }

[<Theory>]
[<InlineData("Pending", true)>]
[<InlineData("Cancelled", false)>]
[<InlineData("CheckedOut", false)>]
[<InlineData("NotCreated", false)>]
let ``When RemoveItem`` state isSuccess = 
    let initialEvents1 = 
        [ BasketCreated
          BasketItemAdded item ]
    
    let initialEvents = 
        match state with
        | "Pending" -> initialEvents1
        | "Cancelled" -> initialEvents1 @ [ BasketCancelled ]
        | "CheckedOut" -> initialEvents1 @ [ BasketCheckedOut((ShippingAddress "a"), [ item ]) ]
        | _ -> []
    
    let command = RemoveItem(productId, 10) |> createCommand aggregateId (Irrelevant, None, None, None)
    let initialEvents' = List.map (fun e -> createInitialEvent aggregateId 0 e) initialEvents
    let error = sprintf "Wrong Basket state %s" state
    
    let checkResult r = 
        match isSuccess with
        | true -> checkSuccess (createExpectedEvent command 1 (BasketItemRemoved(productId, 10))) r
        | false -> checkFailure [ ValidationError error ] r
    handleCommand initialEvents' command |> checkResult

[<Theory>]
[<InlineData(0, 0)>]
[<InlineData(10, 10)>]
let ``When RemoveItem more than quantity`` added removed = 
    let initialEvents1 = [ BasketCreated ]
    let item' = { item with Quantity = added }
    
    let initialEvents = 
        match added with
        | 0 -> initialEvents1
        | _ -> initialEvents1 @ [ BasketItemAdded item' ]
    
    let command = RemoveItem(productId, 20) |> createCommand aggregateId (Irrelevant, None, None, None)
    let initialEvents' = List.map (fun e -> createInitialEvent aggregateId 0 e) initialEvents
    
    let checkResult (r : Result<EventEnvelope<Event> list, IError>) = 
        match removed with
        | 0 -> 
            match r with
            | Ok(r', _) -> Assert.Empty(r')
            | _ -> failwith "failed"
        | _ -> checkSuccess (createExpectedEvent command 1 (BasketItemRemoved(productId, removed))) r
    handleCommand initialEvents' command |> checkResult
