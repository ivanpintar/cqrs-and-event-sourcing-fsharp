module PinetreeCQRS.Infrastructure.Tests.EventTests

open PinetreeCQRS.Infrastructure.Types
open System
open Xunit
open PinetreeCQRS.Infrastructure.Events

type TestEvent = 
    | ToLower of string
    | ToUpper of string
    | NotInteresting of int
    interface IEvent

let handler (event : EventEnvelope<TestEvent>) = 
    match event.Payload with
    | ToUpper e' -> e'.ToUpper()
    | ToLower e' -> e'.ToLower()
    | _ -> "not interested"

let events = 
    [ ToLower("Lower")
      ToUpper("Upper")
      NotInteresting(2) ]

let loadEvents lastEventNumber = 
    let aggregateId = Guid.NewGuid() |> AggregateId
    let causationId = Guid.NewGuid() |> CausationId
    let correlationId = Guid.NewGuid() |> CorrelationId
    let guid = Guid.NewGuid()
    let evt = createEvent aggregateId (causationId, None, correlationId)
    events |> Seq.map evt

let handleEvents = readAndHandleTypeEvents<string, TestEvent> loadEvents handler

[<Fact>]
let ``When Handling Events Results Are Returned``() = 
    let (res, num) = handleEvents 0
    let actual = (res |> Seq.toList, num)
    let expected = ([ "lower"; "UPPER"; "not interested" ], 0)
    Assert.Equal(expected, actual)
