module PinetreeCQRS.Infrastructure.Commands

open System
open PinetreeCQRS.Infrastructure.Types
open PinetreeCQRS.Infrastructure.Events
open FSharpx.Validation

let createCommand aggregateId (version, causationId, correlationId, processId) payload = 
    let commandId = Guid.NewGuid()
    
    let causationId' = 
        match causationId with
        | Some c -> c
        | _ -> CausationId commandId
    
    let correlationId' = 
        match correlationId with
        | Some c -> c
        | _ -> CorrelationId commandId

    { aggregateId = aggregateId
      payload = payload
      commandId = CommandId commandId
      processId = processId
      causationId = causationId'
      correlationId = correlationId'
      version = version }

let createFailedCommand command reasons = 
    let (CommandId cmdGuid) = command.commandId
    { aggregateId = command.aggregateId
      payload = command.payload
      reasons = reasons
      failureId = Guid.NewGuid() |> FailureId
      processId = command.processId
      causationId = CausationId cmdGuid
      correlationId = command.correlationId }

let makeHandler 
    (aggregate : Aggregate<'TState, 'TEvent, 'TCommand>) 
    (load : AggregateId -> EventEnvelope<'TEvent> seq) 
    (commit : EventEnvelope<'TEvent> seq -> EventEnvelope<'TEvent> seq) 
    (onFailure : CommandFailedEnvelope<'TCommand> -> CommandFailedEnvelope<'TCommand>) = 
    fun (command : CommandEnvelope<'TCommand>) -> 
        let id = command.aggregateId
        let events = load id
        let lastEventNumber = Seq.fold (fun acc e -> e.eventNumber) 0 events

        let e = lastEventNumber
        let v = 
            match command.version with
            | Irrelevant -> None
            | Expected v' -> Some(v')

        match e, v with
        | (x, Some(y)) when x > y -> 
            [ FailureReason "Version mismatch" ]
            |> createFailedCommand command
            |> Failure
        | _ -> 
            let eventPayloads = Seq.map (fun (e : EventEnvelope<'TEvent>) -> e.payload) events
            let state = Seq.fold aggregate.applyEvent aggregate.zero eventPayloads
            let result = aggregate.executeCommand state command.payload
            match result with
            | Success createdEvents -> 
                createdEvents
                |> Seq.map (fun e -> createEventMetadata e command)
                |> commit
                |> Success
            | Failure(reasons) -> 
                createFailedCommand command reasons
                |> onFailure
                |> Failure

let processCommandQueue dequeue handler =
    dequeue ()
    |> Seq.map handler
    