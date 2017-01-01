module PinetreeShop.Domain.Products.Tests.CancelReservation

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
      ProductQuantityChanged(15) |> createInitialEvent aggregateId 2
      ProductReserved(10) |> createInitialEvent aggregateId 3 ]

[<Fact>]
let ``When CancelReservation ReservationCanceled``() = 
    let command = CancelReservation(5) |> createCommand aggregateId (Expected(3), None, None, None)
    let result = handleCommand initialEvents command
    let expected = ProductReservationCanceled(5) |> createExpectedEvent command 4
    result |> checkSuccess expected

[<Fact>]
let ``When CancelReservation not created fail``() = 
    CancelReservation(5)
    |> createCommand aggregateId (Expected(0), None, None, None)
    |> handleCommand []
    |> checkFailure [ ValidationError "Product must be created"; ValidationError "Not enough reserved items" ]

[<Fact>]
let ``When CancelReservation more than reserved fail``() = 
    CancelReservation(12)
    |> createCommand aggregateId (Expected(3), None, None, None)
    |> handleCommand initialEvents
    |> checkFailure [ ValidationError "Not enough reserved items" ]
