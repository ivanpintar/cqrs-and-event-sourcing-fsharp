module PinetreeShop.Domain.Tests.TestBase

open PinetreeCQRS.Infrastructure.Types
open System
open System.Collections.Generic
open Chessie.ErrorHandling
open Xunit

let onFailure f = f
let createComparableEvent e = { e with EventId = EventId Guid.Empty }

let failStrings (reasons:string seq) = String.concat "; " reasons |> failwith
let failErrors (reasons:IError seq) = 
    let getString s = s.ToString()
    Seq.map getString reasons |> failStrings 

let createExpectedEvent command eventNumber payload = 
    let (CommandId cmdGuid) = command.CommandId
    { AggregateId = command.AggregateId
      Payload = payload
      CausationId = CausationId cmdGuid
      CorrelationId = command.CorrelationId
      ProcessId = command.ProcessId
      EventId = EventId Guid.Empty
      EventNumber = eventNumber }

let createInitialEvent aggregateId eventNumber payload = 
    { AggregateId = aggregateId
      Payload = payload
      CausationId = Guid.NewGuid() |> CausationId
      CorrelationId = Guid.NewGuid() |> CorrelationId
      ProcessId = None
      EventId = EventId Guid.Empty
      EventNumber = eventNumber }

let checkSuccess expected result =
    match result with
    | Ok (s, _) -> 
        let actual = Seq.head s |> createComparableEvent
        Assert.Equal(expected, actual)
    | Bad f -> failErrors f

let checkFailure expected result =
    match result with
    | Ok (s,_) -> failwith "Did not fail"
    | Bad f -> Assert.Equal<IError>(expected, f)