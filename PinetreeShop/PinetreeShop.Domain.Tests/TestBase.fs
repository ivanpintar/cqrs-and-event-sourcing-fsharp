module PinetreeShop.Domain.Tests.TestBase

open PinetreeCQRS.Infrastructure.Types
open System
open System.Collections.Generic
open FSharpx.Validation
open Xunit

let onFailure f = f
let createComparableEvent e = { e with eventId = EventId Guid.Empty }
let createComparableFailure f = { f with failureId = FailureId Guid.Empty }

let failStrings (reasons:string list) = String.concat "; " reasons |> failwith
let failReasons (reasons:FailureReason list) = 
    let getString fr = 
        let (FailureReason s) = fr
        s

    let rs = List.map getString reasons 
    failStrings rs

let createExpectedEvent command eventNumber payload = 
    let (CommandId cmdGuid) = command.commandId
    { aggregateId = command.aggregateId
      payload = payload
      causationId = CausationId cmdGuid
      correlationId = command.correlationId
      processId = command.processId
      eventId = EventId Guid.Empty
      eventNumber = eventNumber }

let createExpectedFailure command reasons = 
    let (CommandId cmdGuid) = command.commandId
    { aggregateId = command.aggregateId
      payload = command.payload
      reasons = List.map (fun r -> FailureReason r) reasons
      causationId = CausationId cmdGuid
      correlationId = command.correlationId
      processId = command.processId
      failureId = FailureId Guid.Empty }

let createInitialEvent aggregateId eventNumber payload = 
    { aggregateId = aggregateId
      payload = payload
      causationId = Guid.NewGuid() |> CausationId
      correlationId = Guid.NewGuid() |> CorrelationId
      processId = None
      eventId = EventId Guid.Empty
      eventNumber = eventNumber }

let checkSuccess expected result =
    match result with
    | Success s -> 
        let actual = Seq.head s |> createComparableEvent
        Assert.Equal(expected, actual)
    | Failure f -> failReasons f.reasons

let checkFailure expected result =
    match result with
    | Success s -> failwith "Did not fail"
    | Failure f -> 
        let actual = createComparableFailure f
        Assert.Equal(expected, actual)