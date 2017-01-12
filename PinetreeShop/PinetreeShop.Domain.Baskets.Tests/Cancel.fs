module PinetreeShop.Domain.Baskets.Tests.Cancel

open PinetreeShop.Domain.Tests.TestBase
open PinetreeShop.Domain.Baskets.BasketAggregate
open PinetreeShop.Domain.Baskets.Tests.Base
open PinetreeCQRS.Infrastructure.Commands
open PinetreeCQRS.Infrastructure.Events
open PinetreeCQRS.Infrastructure.Types
open Xunit
open System

let aggregateId = Guid.NewGuid() |> AggregateId

let item : BasketItem = 
    { ProductId = Guid.NewGuid() |> ProductId
      ProductName = "Test"
      Price = 2m
      Quantity = 2 }

[<Theory>]
[<InlineData("Pending", true)>]
[<InlineData("Cancelled", false)>]
[<InlineData("CheckedOut", false)>]
[<InlineData("NotCreated", false)>]
let ``When Cancel`` state isSuccess = 
    let initialEvent1 = BasketCreated
    
    let initialEvents = 
        match state with
        | "Pending" -> [ initialEvent1 ]
        | "Cancelled" -> [ initialEvent1; BasketCancelled ]
        | "CheckedOut" -> 
            [ initialEvent1
              BasketCheckedOut((ShippingAddress "a"), [ item ]) ]
        | _ -> []
    
    let command = Cancel |> createCommand aggregateId (Irrelevant, None, None, None)
    let initialEvents' = List.map (fun e -> createInitialEvent aggregateId 0 e) initialEvents
    let error = sprintf "Wrong Basket state %s" state
    
    let checkResult r = 
        match isSuccess with
        | true -> checkSuccess [ createExpectedEvent command 1 BasketCancelled ] r
        | false -> checkFailure [ ValidationError error ] r
    handleCommand initialEvents' command |> checkResult
