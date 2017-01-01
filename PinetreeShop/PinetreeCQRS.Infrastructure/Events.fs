module PinetreeCQRS.Infrastructure.Events

open System
open Types

let createEvent aggregateId (causationId, processId, correlationId) payload = 
    { AggregateId = aggregateId
      Payload = payload
      EventId = Guid.NewGuid() |> EventId
      ProcessId = processId
      CausationId = causationId
      CorrelationId = correlationId
      EventNumber = 0 }

let createEventMetadata payload command = 
    let (CommandId cmdGuid) = command.CommandId
    { AggregateId = command.AggregateId
      Payload = payload
      EventId = Guid.NewGuid() |> EventId
      ProcessId = command.ProcessId
      CausationId = CausationId cmdGuid
      CorrelationId = command.CorrelationId
      EventNumber = 0 }

let readAndHandleAllEvents loadEvents handler lastEventNumber = 
    let events = loadEvents lastEventNumber
    let result = Seq.map handler events
    let eventProcessed = Seq.fold (fun acc e -> e.EventNumber) lastEventNumber events
    (result, eventProcessed)

let readAndHandleTypeEvents<'TResult, 'TEvent when 'TEvent :> IEvent> loadEvents (handler:EventEnvelope<'TEvent> -> 'TResult) lastEventNumber =
    let events = loadEvents lastEventNumber
    let result = Seq.map handler events
    let eventProcessed = Seq.fold (fun acc e -> e.EventNumber) lastEventNumber events
    (result, eventProcessed)
