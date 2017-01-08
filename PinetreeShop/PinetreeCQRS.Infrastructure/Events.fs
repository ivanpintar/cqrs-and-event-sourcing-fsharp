module PinetreeCQRS.Infrastructure.Events

open System
open Types
open Chessie.ErrorHandling

let createEvent aggregateId (causationId, processId, correlationId) payload = 
    { AggregateId = aggregateId
      Payload = payload
      EventId = Guid.NewGuid() |> EventId
      ProcessId = processId
      CausationId = causationId
      CorrelationId = correlationId
      EventNumber = 0 }

let createEventMetadata payload command = 
    let (CommandId cmdGuid) = command.CommandId
    { AggregateId = command.AggregateId
      Payload = payload
      EventId = Guid.NewGuid() |> EventId
      ProcessId = command.ProcessId
      CausationId = CausationId cmdGuid
      CorrelationId = command.CorrelationId
      EventNumber = 0 }

let makeEventProcessor (processManager : ProcessManager<'TState>) 
    (load : ProcessId -> Result<EventEnvelope<IEvent> list, IError>) 
    (enqueue : (QueueName * CommandEnvelope<ICommand>) list -> Result<CommandEnvelope<ICommand> list, IError>) = 
    let handleEvent (event:EventEnvelope<IEvent>) : Result<CommandEnvelope<ICommand> list, IError> = 
        let processEvents events = 
            let state = List.fold processManager.ApplyEvent processManager.Zero events
            let result = processManager.ProcessEvent state event
            result >>= enqueue
        
        match event.ProcessId with
        | Some pid ->
            let loadedEvents = load pid
            processEvents <!> loadedEvents |> flatten
        | _ -> Ok ([], [ Error "No process id on event" ])
    handleEvent