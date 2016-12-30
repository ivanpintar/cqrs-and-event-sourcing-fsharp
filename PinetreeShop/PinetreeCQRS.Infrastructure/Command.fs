module PinetreeCQRS.Infrastructure.Commands

open System
open PinetreeCQRS.Infrastructure.Types
open PinetreeCQRS.Infrastructure.Events
open FSharpx.Validation

let createCommand aggregateId commandData causationId processId correlationId = 
    let commandId = Guid.NewGuid()
    
    let causationId' = 
        match causationId with
        | Some c -> c
        | _ -> commandId
    
    let correlationId' = 
        match correlationId with
        | Some c -> c
        | _ -> commandId
    
    { aggregateId = aggregateId
      payload = commandData
      commandId = commandId
      processId = processId
      causationId = causationId'
      correlationId = correlationId' }

let createMetadata payload (command:Command<'TCommand>) : Event<'TEvent> = 
    { aggregateId = command.aggregateId
      payload = payload
      eventId = Guid.NewGuid()
      processId = command.processId
      causationId = command.commandId
      correlationId = command.correlationId
      eventNumber = None }

let createFailedCommand payload reason (command:Command<'TCommand>) : Failed<'TCommand> = 
    { aggregateId = command.aggregateId
      payload = payload
      reason = reason
      eventId = Guid.NewGuid()
      processId = command.processId
      causationId = command.commandId
      correlationId = command.correlationId
      eventNumber = None }


let makeHandler (aggregate : Aggregate<'TState, 'TEvent, 'TCommand>) (load : Type -> Guid -> Event<'TEvent> seq) 
    (commit : Event<'TEvent> seq -> Event<'TEvent> seq) (onFailure : Failed<'TCommand> -> Failed<'TCommand>) = 
    fun (command:Command<'TCommand>) -> 
        let id = command.aggregateId
        let events = load typeof<'TState> id |> Seq.map (fun e -> e.payload)
        let state = Seq.fold aggregate.applyEvent aggregate.zero events
        let result = aggregate.executeCommand state command.payload
        match result with
        | Success createdEvents -> 
            createdEvents
            |> Seq.map (fun e -> createMetadata e command) 
            |> commit
            |> Success
        | Failure (failedCommand, reason) -> 
            createFailedCommand failedCommand reason command
            |> onFailure
            |> Failure