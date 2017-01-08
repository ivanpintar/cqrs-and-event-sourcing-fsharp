module PinetreeCQRS.Infrastructure.Commands

open System
open PinetreeCQRS.Infrastructure.Types
open PinetreeCQRS.Infrastructure.Events
open Chessie.ErrorHandling

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
            
    { AggregateId = aggregateId
      Payload = payload
      CommandId = CommandId commandId
      ProcessId = processId
      CausationId = causationId'
      CorrelationId = correlationId'
      ExpectedVersion = version }

let makeCommandHandler (aggregate : Aggregate<'TState, 'TCommand, 'TEvent>) 
    (load : AggregateId -> Result<EventEnvelope<'TEvent> list, IError>) 
    (commit : EventEnvelope<'TEvent> list -> Result<EventEnvelope<'TEvent> list, IError>) = 
    let handleCommand command : Result<EventEnvelope<'TEvent> list, IError> = 
        let processEvents events = 
            let lastEventNumber = List.fold (fun acc e' -> e'.EventNumber) 0 events
            let e = lastEventNumber
            
            let v = 
                match command.ExpectedVersion with
                | Expected v' -> Some(v')
                | Irrelevant -> None
            match e, v with
            | (x, Some(y)) when x > y -> Bad [ Error "Version mismatch" :> IError ]
            | _ -> 
                let eventPayloads = List.map (fun (e : EventEnvelope<'TEvent>) -> e.Payload) events
                let state = List.fold aggregate.ApplyEvent aggregate.Zero eventPayloads
                let result = aggregate.ExecuteCommand state command.Payload
                List.map (fun e -> createEventMetadata e command) <!> result >>= commit
        
        let id = command.AggregateId
        let loadedEvents = load id
        processEvents <!> loadedEvents |> flatten
    handleCommand