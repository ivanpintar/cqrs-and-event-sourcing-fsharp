module PinetreeCQRS.Infrastructure.Types

open System
open Chessie.ErrorHandling

type AggregateId = AggregateId of Guid
type EventId = EventId of Guid
type CommandId = CommandId of Guid
type FailureId = FailureId of Guid
type CausationId = CausationId of Guid
type CorrelationId = CorrelationId of Guid
type ProcessId = ProcessId of Guid
type AggregateVersion =
    | Expected of int
    | Irrelevant
type EventNumber = int
type IEvent = interface end
type IError = interface end

type Error = 
    | Error of string 
    interface IError
    override e.ToString() = sprintf "%A" e

type EventEnvelope<'TEvent when 'TEvent :> IEvent> = 
    { AggregateId : AggregateId
      Payload : 'TEvent
      EventId : EventId
      ProcessId : ProcessId option
      CausationId : CausationId
      CorrelationId : CorrelationId
      EventNumber : EventNumber }

type CommandEnvelope<'TCommand> = 
    { AggregateId : AggregateId
      Payload : 'TCommand
      CommandId : CommandId
      ProcessId : ProcessId option
      CausationId : CausationId
      CorrelationId : CorrelationId 
      ExpectedVersion : AggregateVersion }

type Aggregate<'TState, 'TEvent, 'TCommand> = 
    { Zero : 'TState
      ApplyEvent : 'TState -> 'TEvent -> 'TState
      ExecuteCommand : 'TState -> 'TCommand -> Result<'TEvent list, IError> }
