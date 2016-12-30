module PinetreeShop.Domain.Tests.TestBase

open PinetreeCQRS.Infrastructure.Types
open System
open System.Collections.Generic
open FSharpx.Validation

let onFailure f = f

let createComparableEvent e : Event<'TEvent> = 
    { e with eventId = Guid.Empty
             eventNumber = None }

let createComparableFailure f : Failed<'TCommand> = 
    { f with eventId = Guid.Empty
             eventNumber = None }

let createExpectedEvent (command:Command<'TCommand>) payload : Event<'TEvent> = 
    { aggregateId = command.aggregateId
      payload = payload
      causationId = command.commandId
      correlationId = command.correlationId
      processId = command.processId
      eventId = Guid.Empty
      eventNumber = None }

let createExpectedFailure (command:Command<'TCommand>) reason : Failed<'TCommand> = 
    { aggregateId = command.aggregateId
      payload = command.payload
      reason = reason
      causationId = command.commandId
      correlationId = command.correlationId
      processId = command.processId
      eventId = Guid.Empty
      eventNumber = None }

let createInitialEvent aggregateId processId payload : Event<'TEvent> = 
    { aggregateId = aggregateId
      payload = payload
      causationId = Guid.NewGuid()
      correlationId = Guid.NewGuid()
      processId = processId
      eventId = Guid.Empty
      eventNumber = None }
