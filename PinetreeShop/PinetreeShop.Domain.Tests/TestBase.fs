module PinetreeShop.Domain.Tests.TestBase

open PinetreeCQRS.Infrastructure.Types
open System
open System.Linq
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

let checkSuccess<'TEvent when 'TEvent :> IEvent> (expected:EventEnvelope<'TEvent> list) (result:Result<EventEnvelope<'TEvent> list, _>) =
    match result with
    | Ok (s, _) -> 
        List.zip expected s
        |> List.iter (fun (exp, act) ->
             let act' = createComparableEvent act
             Assert.Equal(exp, act'))
    | Bad f -> failErrors f

let checkSuccessList expected result = failwith "fail"

let checkFailure (expected:IError list) result =
    match result with
    | Ok (s,_) -> failwith "Did not fail"
    | Bad f -> Assert.Equal<IError>((expected.ToList()), (f.ToList()))