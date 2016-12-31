module PinetreeCQRS.Infrastructure.Events

open System
open Types

let createEvent aggregateId (causationId, processId, correlationId) payload = 
    { aggregateId = aggregateId
      payload = payload
      eventId = Guid.NewGuid() |> EventId
      processId = processId
      causationId = causationId
      correlationId = correlationId
      eventNumber = 0 }

let createEventMetadata payload command = 
    let (CommandId cmdGuid) = command.commandId
    { aggregateId = command.aggregateId
      payload = payload
      eventId = Guid.NewGuid() |> EventId
      processId = command.processId
      causationId = CausationId cmdGuid
      correlationId = command.correlationId
      eventNumber = 0 }

let readAndHandleAllEvents loadEvents handler lastEventNumber = 
    let events = loadEvents lastEventNumber
    let result = Seq.map handler events
    let eventProcessed = Seq.fold (fun acc e -> e.eventNumber) lastEventNumber events
    (result, eventProcessed)

let readAndHandleTypeEvents<'TResult, 'TEvent when 'TEvent :> IEvent> (loadEvents:EventNumber -> seq<EventEnvelope<'TEvent>>) (handler:EventEnvelope<'TEvent> -> 'TResult) lastEventNumber =
    let events = loadEvents lastEventNumber
    let result = Seq.map handler events
    let eventProcessed = Seq.fold (fun acc e -> e.eventNumber) lastEventNumber events
    (result, eventProcessed)
