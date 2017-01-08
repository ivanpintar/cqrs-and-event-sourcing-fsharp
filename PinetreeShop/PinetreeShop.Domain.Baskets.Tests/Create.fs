module PinetreeShop.Domain.Baskets.Tests.Create

open PinetreeShop.Domain.Tests.TestBase
open PinetreeShop.Domain.Baskets.BasketAggregate
open PinetreeShop.Domain.Baskets.Tests.Base
open PinetreeCQRS.Infrastructure.Commands
open PinetreeCQRS.Infrastructure.Events
open PinetreeCQRS.Infrastructure.Types
open Xunit
open System

let aggregateId = Guid.NewGuid() |> AggregateId

[<Fact>]
let ``When Create OrderCreated``() = 
    let command = Create |> createCommand aggregateId (Expected(0), None, None, None)
    let expected = BasketCreated |> createExpectedEvent command 1
    handleCommand [] command |> checkSuccess expected

[<Fact>]
let ``When Create created fail``() = 
    let initialEvent = BasketCreated |> createInitialEvent aggregateId 1
    Create
    |> createCommand aggregateId (Irrelevant, None, None, None)
    |> handleCommand [ initialEvent ]
    |> checkFailure [ ValidationError "Wrong Basket state Pending" ]
