module PinetreeCQRS.Infrastructure.Tests.Helpers

open PinetreeCQRS.Infrastructure.Types
open System
open FSharpx.Validation
open PinetreeCQRS.Infrastructure.Commands
open PinetreeCQRS.Infrastructure.Events

let fail reasons = String.concat "; " reasons |> failwith
let comparableEvent e = { e with eventId = EventId Guid.Empty }
let comparableFailure f = { f with failureId = FailureId Guid.Empty }

let createExpectedEvent (aggregateId, command, eventNumber) payload = 
    let (CommandId cmdGuid) = command.commandId
    { aggregateId = aggregateId 
      eventId = EventId Guid.Empty 
      processId = None 
      causationId = CausationId cmdGuid
      correlationId = command.correlationId 
      eventNumber = eventNumber 
      payload = payload }

let createExpectedFailure (aggregateId, command) reasons = 
    let (CommandId cmdGuid) = command.commandId
    { aggregateId = aggregateId 
      failureId = FailureId Guid.Empty 
      reasons = List.map (fun r -> FailureReason r) reasons 
      processId = None 
      causationId = CausationId cmdGuid
      correlationId = command.correlationId 
      payload = command.payload }
