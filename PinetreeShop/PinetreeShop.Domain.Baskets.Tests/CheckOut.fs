module PinetreeShop.Domain.Baskets.Tests.CheckOut

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
[<InlineData("Pending", false)>]
[<InlineData("PendingWithItems", true)>]
[<InlineData("Cancelled", false)>]
[<InlineData("CheckedOut", false)>]
[<InlineData("NotCreated", false)>]
let ``When CheckOut`` state isSuccess = 
    let initialEvent1 = BasketCreated
    
    let initialEvents = 
        match state with
        | "Pending" -> [ initialEvent1 ]
        | "PendingWithItems" -> 
            [ initialEvent1
              BasketItemAdded item ]
        | "Cancelled" -> 
            [ initialEvent1
              BasketItemAdded item
              BasketCancelled ]
        | "CheckedOut" -> 
            [ initialEvent1
              BasketItemAdded item
              BasketCheckedOut((ShippingAddress "a"), [ item ]) ]
        | _ -> []
    
    let command = CheckOut(ShippingAddress "a") |> createCommand aggregateId (Irrelevant, None, None, None)
    let initialEvents' = List.map (fun e -> createInitialEvent aggregateId 0 e) initialEvents
    
    let errors = 
        match state with
        | "Pending" -> [ ValidationError "No items" :> IError ]
        | "NotCreated" -> 
            [ ValidationError "Wrong Basket state NotCreated" :> IError
              ValidationError "No items" :> IError ]
        | _ -> [ ValidationError(sprintf "Wrong Basket state %s" state) :> IError ]
    
    let checkResult r = 
        match isSuccess with
        | true -> 
            let expectedEvent = BasketCheckedOut((ShippingAddress "a"), [ item ]) |> createExpectedEvent command 1
            checkSuccess expectedEvent r
        | false -> checkFailure errors r
    
    handleCommand initialEvents' command |> checkResult

[<Fact>]
let ``When CheckOut empty address fail``() = 
    let initialEvents = 
        [ BasketCreated
          BasketItemAdded item ]
        |> List.map (fun ie -> createInitialEvent aggregateId 1 ie)
    CheckOut(ShippingAddress "")
    |> createCommand aggregateId (Irrelevant, None, None, None)
    |> handleCommand initialEvents
    |> checkFailure [ ValidationError "Shipping Address cannot be empty" ]
