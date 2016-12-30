module PinetreeCQRS.Infrastructure.Types

open System

let Success = Choice1Of2
let Failure = Choice2Of2

type Event<'TEvent> = 
    { aggregateId : Guid
      payload : 'TEvent
      eventId : Guid
      processId : Guid Option
      causationId : Guid
      correlationId : Guid
      eventNumber : int Option }

type Command<'TCommand> = 
    { aggregateId : Guid
      payload : 'TCommand
      commandId : Guid
      processId : Guid Option
      causationId : Guid
      correlationId : Guid }

type FailureReason = string
type Failed<'TCommand> = 
    { aggregateId : Guid
      payload : 'TCommand
      reason : FailureReason list
      eventId : Guid
      processId : Guid Option
      causationId : Guid
      correlationId : Guid
      eventNumber : int Option }

type CommandResult<'a, 'b> = Choice<'a, 'b>

type Aggregate<'TState, 'TEvent, 'TCommand> = 
    { zero : 'TState
      applyEvent : 'TState -> 'TEvent -> 'TState
      executeCommand : 'TState -> 'TCommand -> CommandResult<'TEvent list, 'TCommand * FailureReason list> }
