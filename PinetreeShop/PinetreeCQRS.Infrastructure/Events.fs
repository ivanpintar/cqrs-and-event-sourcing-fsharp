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