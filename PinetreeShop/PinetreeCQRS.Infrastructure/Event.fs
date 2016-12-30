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

let readAndHandleEvents loadEvents handler lastEventNumber = 
    let processEvents handler = List.map (fun (e : EventEnvelope<'TEvent>) -> handler e.payload)
    let events = loadEvents lastEventNumber
    let result = processEvents handler events
    let eventProcessed = List.fold (fun acc e -> e.eventNumber) lastEventNumber events
    (result, eventProcessed)
