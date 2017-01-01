module PinetreeCQRS.Infrastructure.Tests.Helpers

open PinetreeCQRS.Infrastructure.Types
open System
open PinetreeCQRS.Infrastructure.Commands
open PinetreeCQRS.Infrastructure.Events

let fail reasons = String.concat "; " reasons |> failwith
let comparableEvent e = { e with EventId = EventId Guid.Empty }

let createExpectedEvent (aggregateId, command, eventNumber) payload = 
    let (CommandId cmdGuid) = command.CommandId
    { AggregateId = aggregateId 
      EventId = EventId Guid.Empty 
      ProcessId = None 
      CausationId = CausationId cmdGuid
      CorrelationId = command.CorrelationId 
      EventNumber = eventNumber 
      Payload = payload }
