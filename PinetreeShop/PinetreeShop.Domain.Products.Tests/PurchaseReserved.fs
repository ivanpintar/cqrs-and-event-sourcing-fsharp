module PinetreeShop.Domain.Products.Tests.PurchaseReserved

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
let ``When PurchaseReserved ProductPurchased``() = 
    let command = PurchaseReserved(10) |> createCommand aggregateId (Expected(3), None, None, None)
    let expected = ProductPurchased(10) |> createExpectedEvent command 4
    handleCommand initialEvents command |> checkSuccess [ expected ]

[<Fact>]
let ``When PurchaseReserved not created fail``() = 
    PurchaseReserved(5)
    |> createCommand aggregateId (Expected(0), None, None, None)
    |> handleCommand []
    |> checkFailure [ ValidationError "Product must be created"
                      ValidationError "Not enough reserved items" ]

[<Fact>]
let ``When PurchaseReserved more than reserved fail``() = 
    PurchaseReserved(12)
    |> createCommand aggregateId (Expected(3), None, None, None)
    |> handleCommand initialEvents
    |> checkFailure [ ValidationError "Not enough reserved items" ]
