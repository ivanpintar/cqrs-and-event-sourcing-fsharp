module PinetreeShop.Domain.Products.Tests.AddToStock

open PinetreeShop.Domain.Tests.TestBase
open PinetreeShop.Domain.Products.ProductAggregate
open PinetreeShop.Domain.Products.Tests.Base
open PinetreeCQRS.Infrastructure.Commands
open PinetreeCQRS.Infrastructure.Events
open Xunit
open FSharpx.Validation
open System

[<Fact>]
let ``When AddToStock ProductQuantityChanged`` () =
//    let aggregateId = Guid.NewGuid()
//
//
//    let payload = AddToStock(15)
//    let command = createCommand aggregateId payload None None None
//
//    let result = handler command
//
//    let expected = ProductQuantityChanged(15) |> createExpectedEvent command
//
//    match result with 
//    | Success s ->
//        let actual = Seq.head s |> createComparableEvent 
//        Assert.Equal(expected, actual)
//    | Failure f -> 
//        failwith "failed"
    0