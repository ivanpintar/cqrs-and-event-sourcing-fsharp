module PinetreeShop.Domain.Products.Tests.Reserve

open PinetreeShop.Domain.Tests.TestBase
open PinetreeShop.Domain.Products.ProductAggregate
open PinetreeShop.Domain.Products.Tests.Base
open PinetreeCQRS.Infrastructure.Commands
open PinetreeCQRS.Infrastructure.Events
open PinetreeCQRS.Infrastructure.Types
open Xunit
open System

let aggregateId = Guid.NewGuid() |> AggregateId

let initialEvents = 
    [ ProductCreated("Test product", 15m) |> createInitialEvent aggregateId 1
      ProductQuantityChanged(15) |> createInitialEvent aggregateId 2 ]

[<Fact>]
let ``When Reserve ProductReserved``() = 
    let command = Reserve(5) |> createCommand aggregateId (Expected(2), None, None, None)
    let expected = ProductReserved(5) |> createExpectedEvent command 3
    handleCommand initialEvents command |> checkSuccess [ expected ]

[<Fact>]
let ``When Reserve not created ProductReservationFailed``() = 
    let command = Reserve(7) |> createCommand aggregateId (Expected(0), None, None, None)
    
    let expectedErrors = 
        [ ValidationError "Product must be created" :> IError
          ValidationError "Not enough available items" :> IError ]
    
    let expected = ProductReservationFailed(7) |> createExpectedEvent command 1
    handleCommand [] command |> checkSuccess [ expected ]

[<Fact>]
let ``When Reserve more than available ProductReservationFailed``() = 
    let reserved = ProductReserved(10) |> createInitialEvent aggregateId 3
    let initialEvents' = initialEvents @ [ reserved ]
    let command = Reserve(7) |> createCommand aggregateId (Expected(3), None, None, None)
    let expected = ProductReservationFailed(7) |> createExpectedEvent command 4
    handleCommand initialEvents' command |> checkSuccess [ expected ]

[<Fact>]
let ``When Reserve no items added ProductReservationFailed``() = 
    let initialEvents' = [ initialEvents.Head ]
    let command = Reserve(7) |> createCommand aggregateId (Expected(1), None, None, None)
    let expected = ProductReservationFailed(7) |> createExpectedEvent command 2
    handleCommand initialEvents' command |> checkSuccess [ expected ]
