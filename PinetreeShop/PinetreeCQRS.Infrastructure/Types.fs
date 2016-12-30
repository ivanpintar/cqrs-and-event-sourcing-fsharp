module PinetreeCQRS.Infrastructure.Types

open System

let Success = Choice1Of2
let Failure = Choice2Of2

type AggregateId = AggregateId of Guid
type EventId = EventId of Guid
type CommandId = CommandId of Guid
type FailureId = FailureId of Guid
type FailureReason = FailureReason of string

type CausationId = CausationId of Guid
type CorrelationId = CorrelationId of Guid
type ProcessId = 
    | ProcessId of Guid
    | NoProcessId
type AggregateVersion =
    | Expected of int
    | Irrelevant

type EventNumber = int

type CommandResult<'a, 'b> = Choice<'a, 'b>

type EventEnvelope<'TEvent> = 
    { aggregateId : AggregateId
      payload : 'TEvent
      eventId : EventId
      processId : ProcessId option
      causationId : CausationId
      correlationId : CorrelationId
      eventNumber : EventNumber }

type CommandEnvelope<'TCommand> = 
    { aggregateId : AggregateId
      payload : 'TCommand
      commandId : CommandId
      processId : ProcessId option
      causationId : CausationId
      correlationId : CorrelationId 
      version : AggregateVersion }

type CommandFailedEnvelope<'TCommand> = 
    { aggregateId : AggregateId
      payload : 'TCommand
      reasons : FailureReason list
      failureId : FailureId
      processId : ProcessId option
      causationId : CausationId
      correlationId : CorrelationId }

type Aggregate<'TState, 'TEvent, 'TCommand> = 
    { zero : 'TState
      applyEvent : 'TState -> 'TEvent -> 'TState
      executeCommand : 'TState -> 'TCommand -> CommandResult<'TEvent list, FailureReason list> }
