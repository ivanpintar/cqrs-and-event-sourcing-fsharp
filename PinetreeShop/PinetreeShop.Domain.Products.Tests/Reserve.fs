module PinetreeShop.Domain.Products.Tests.Reserve

open PinetreeShop.Domain.Tests.TestBase
open PinetreeShop.Domain.Products.ProductAggregate
open PinetreeShop.Domain.Products.Tests.Base
open PinetreeCQRS.Infrastructure.Commands
open PinetreeCQRS.Infrastructure.Events
open PinetreeCQRS.Infrastructure.Types
open Xunit
open FSharpx.Validation
open System

let aggregateId = Guid.NewGuid() |> AggregateId

let initialEvents = 
    [ ProductCreated("Test product", 15m) |> createInitialEvent aggregateId 1
      ProductQuantityChanged(15) |> createInitialEvent aggregateId 2 ]

[<Fact>]
let ``When Reserve ProductReserved``() = 
    let command = Reserve(5) |> createCommand aggregateId (Expected(2), None, None, None)
    let result = handleCommand initialEvents command
    let expected = ProductReserved(5) |> createExpectedEvent command 3
    result |> checkSuccess expected



[<Fact>]
let ``When Reserve not created ProductReservationFailed``() =
    let command = Reserve(7) |> createCommand aggregateId (Expected(0), None, None, None)
    let result = handleCommand [] command
    let expected = ProductReservationFailed(7, [ FailureReason "Product must be created"; FailureReason "Not enough available items"]) |> createExpectedEvent command 1
    result |> checkSuccess expected


[<Fact>]
let ``When Reserve more than available ProductReservationFailed``() =
    let reserved = ProductReserved(10) |> createInitialEvent aggregateId 3
    let initialEvents' = initialEvents @ [reserved]
    let command = Reserve(7) |> createCommand aggregateId (Expected(3), None, None, None)
    let result = handleCommand initialEvents' command
    let expected = ProductReservationFailed(7, [ FailureReason "Not enough available items"]) |> createExpectedEvent command 4
    result |> checkSuccess expected


[<Fact>]
let ``When Reserve no items added ProductReservationFailed``() =
    let initialEvents' = [initialEvents.Head]
    let command = Reserve(7) |> createCommand aggregateId (Expected(1), None, None, None)
    let result = handleCommand initialEvents' command
    let expected = ProductReservationFailed(7, [ FailureReason "Not enough available items"]) |> createExpectedEvent command 2
    result |> checkSuccess expected

