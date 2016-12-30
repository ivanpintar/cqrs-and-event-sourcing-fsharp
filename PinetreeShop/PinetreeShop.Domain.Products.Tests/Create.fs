module PinetreeShop.Domain.Products.Tests.Create

open PinetreeShop.Domain.Tests.TestBase
open PinetreeShop.Domain.Products.ProductAggregate
open PinetreeShop.Domain.Products.Tests.Base
open PinetreeCQRS.Infrastructure.Commands
open PinetreeCQRS.Infrastructure.Events
open Xunit
open FSharpx.Validation
open System

[<Fact>]
let ``When Create ProductCreated``() = 
    let aggregateId = Guid.NewGuid()
    let payload = Create("Test product", 15m)
    let command = createCommand aggregateId payload None None None

    let result = handleCommand [] command

    let expected = ProductCreated("Test product", 15m) |> createExpectedEvent command
    match result with
    | Success s -> 
        let actual = Seq.head s |> createComparableEvent
        Assert.Equal(expected, actual)
    | Failure f -> fail f.reasons

[<Fact>]
let ``When Create with negative price Failure``() = 
    let aggregateId = Guid.NewGuid()
    let payload = Create("Test product", -15m)
    let command = createCommand aggregateId payload None None None

    let result = handleCommand [] command
    
    let expected = createExpectedFailure command [ "Price must be a positive number" ]
    match result with
    | Success s -> failwith "Did not fail"
    | Failure f -> 
        let actual = createComparableFailure f
        Assert.Equal(expected, actual)

[<Fact>]
let ``When Create already created Failure``() = 
    let aggregateId = Guid.NewGuid()
    let initialEvent = ProductCreated("Test product", 15m) |> createInitialEvent aggregateId None
    let payload = Create("Test product", 15m)
    let command = createCommand aggregateId payload None None None
    
    let result = handleCommand [ initialEvent ] command
    
    let expected = createExpectedFailure command [ "Product already created" ]
    match result with
    | Success s -> failwith "Did not fail"
    | Failure f -> 
        let actual = createComparableFailure f
        Assert.Equal(expected, actual)
