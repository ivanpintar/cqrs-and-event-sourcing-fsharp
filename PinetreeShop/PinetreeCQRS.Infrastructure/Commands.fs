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

let makeCommandHandler (aggregate : Aggregate<'TState, 'TEvent, 'TCommand>) 
    (load : AggregateId -> Result<EventEnvelope<'TEvent> seq, IError>) 
    (commit : EventEnvelope<'TEvent> seq -> Result<EventEnvelope<'TEvent> seq, IError>) = 
    let handleCommand command : Result<EventEnvelope<'TEvent> seq, IError> = 
        let processEvents events = 
            let lastEventNumber = Seq.fold (fun acc e' -> e'.EventNumber) 0 events
            let e = lastEventNumber
            
            let v = 
                match command.ExpectedVersion with
                | Expected v' -> Some(v')
                | Irrelevant -> None
            match e, v with
            | (x, Some(y)) when x > y -> 
                Bad [ Error "Version mismatch" :> IError ]
            | _ -> 
                let eventPayloads = Seq.map (fun (e : EventEnvelope<'TEvent>) -> e.Payload) events
                let state = Seq.fold aggregate.ApplyEvent aggregate.Zero eventPayloads
                let result = aggregate.ExecuteCommand state command.Payload
                Seq.map (fun e -> createEventMetadata e command) <!> result >>= commit

        let id = command.AggregateId
        let loadedEvents = load id
        processEvents <!> loadedEvents |> flatten

    handleCommand
            

let processCommandQueue (dequeue:_ -> Result<CommandEnvelope<'TCommand> seq, IError>) handler = 
    Seq.map handler <!> dequeue()
