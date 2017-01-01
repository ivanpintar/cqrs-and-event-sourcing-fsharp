module PinetreeCQRS.Persistence.SqlServer

open System
open System.Linq
open PinetreeCQRS.Infrastructure.Types
open FSharp.Data.Sql
open Chessie.ErrorHandling

type DataAccessError = 
    | DataAccessError of string
    interface IError

module private DataAccess = 
    type dbSchema = SqlDataProvider< ConnectionStringName="EventStore", UseOptionTypes=true >
    
    let ctx = dbSchema.GetDataContext()
    let deserialize<'TResult> payload : 'TResult = failwith ""
    let serialize payload : string = ""
    
    let entityToEvent<'TEvent when 'TEvent :> IEvent> (e : dbSchema.dataContext.``dbo.EventEntityEntity``) = 
        let pid = 
            match e.ProcessId with
            | Some p -> Some(ProcessId p)
            | None -> None
        { EventNumber = e.Id
          EventId = EventId e.EventId
          AggregateId = AggregateId e.AggregateId
          CausationId = CausationId e.CausationId
          CorrelationId = CorrelationId e.CorrelationId
          ProcessId = pid
          Payload = deserialize<'TEvent> e.EventPayload }
    
    let entityToCommand<'TCommand> (e : dbSchema.dataContext.``dbo.CommandEntityEntity``) = 
        let pid = 
            match e.ProcessId with
            | Some p -> Some(ProcessId p)
            | None -> None
        
        let version = 
            match e.ExpectedVersion with
            | Some vn -> Expected vn
            | _ -> Irrelevant
        
        { ExpectedVersion = version
          CommandId = CommandId e.CommandId
          AggregateId = AggregateId e.AggregateId
          CausationId = CausationId e.CausationId
          CorrelationId = CorrelationId e.CorrelationId
          ProcessId = pid
          Payload = deserialize<'TCommand> e.CommandPayload }
    
    let processIdToGuid pid = 
        match pid with
        | Some pid -> 
            let (ProcessId id) = pid
            Some id
        | _ -> None
    
    let eventToEntity<'TEvent when 'TEvent :> IEvent> (e : EventEnvelope<'TEvent>) = 
        let category = typeof<'TEvent>.ToString()
        let (AggregateId aggId) = e.AggregateId
        let (EventId evtId) = e.EventId
        let (CausationId causeId) = e.CausationId
        let (CorrelationId corrId) = e.CorrelationId
        let payload = serialize e.Payload
        let entity = ctx.Dbo.EventEntity.Create(aggId, category, causeId, corrId, evtId, payload)
        entity.ProcessId <- processIdToGuid e.ProcessId
    
    let commandToEntity<'TCommand> (e : CommandEnvelope<'TCommand>) = 
        let queueName = typeof<'TCommand>.ToString()
        let (AggregateId aggId) = e.AggregateId
        let (CommandId cmdId) = e.CommandId
        let (CausationId causeId) = e.CausationId
        let (CorrelationId corrId) = e.CorrelationId
        let payload = serialize e.Payload
        let entity = ctx.Dbo.CommandEntity.Create(aggId, causeId, cmdId, payload, corrId, queueName)
        entity.ProcessId <- processIdToGuid e.ProcessId
        entity.ExpectedVersion <- match e.ExpectedVersion with
                                  | Expected vn -> Some vn
                                  | Irrelevant -> None
    
    let commitEvents events = 
        Seq.map eventToEntity events |> ignore
        ctx.SubmitUpdates()
        events
    
    let loadEvents = 
        query { 
            for e in ctx.Dbo.EventEntity do
                select e
        }
    
    let loadTypeEvents typ = 
        query { 
            for e in loadEvents do
                where (e.Category = typ.ToString())
                select e
        }
    
    let loadAggregateEvents typ (AggregateId aggregateId) = 
        query { 
            for e in (loadTypeEvents typ) do
                where (e.AggregateId = aggregateId)
                select e
        }
    
    let queueCommand commands = 
        Seq.map commandToEntity commands |> ignore
        ctx.SubmitUpdates()
        commands
    
    let dequeueCommands queueName = 
        let cmds = 
            query { 
                for c in ctx.Dbo.CommandEntity do
                    where (c.QueueName = queueName)
                    select c
            }
            |> Seq.toList
        cmds |> List.iter (fun c -> c.Delete())
        Seq.map entityToCommand cmds

module Events = 
    let commitEvents<'TEvent when 'TEvent :> IEvent> (events : EventEnvelope<'TEvent> seq) : Result<EventEnvelope<'TEvent> seq, IError> = 
        try 
            DataAccess.commitEvents events |> ok
        with ex -> Bad [ DataAccessError ex.Message :> IError ]
    
    let loadAllEvents (number : EventNumber) : Result<EventEnvelope<'TEvent> seq, IError> = 
        try 
            DataAccess.loadEvents
            |> Seq.toList
            |> Seq.map DataAccess.entityToEvent
            |> ok
        with ex -> Bad [ DataAccessError ex.Message :> IError ]
    
    let loadTypeEvents<'TEvent when 'TEvent :> IEvent> (number : EventNumber) : Result<EventEnvelope<'TEvent> seq, IError> = 
        try 
            DataAccess.loadTypeEvents typeof<'TEvent>
            |> Seq.toList
            |> Seq.map DataAccess.entityToEvent
            |> ok
        with ex -> Bad [ DataAccessError ex.Message :> IError ]
    
    let loadAggregateEvents<'TEvent when 'TEvent :> IEvent> (number : EventNumber) (aggregateId : AggregateId) : Result<EventEnvelope<'TEvent> seq, IError> = 
        try 
            DataAccess.loadAggregateEvents typeof<'TEvent> aggregateId
            |> Seq.toList
            |> Seq.map DataAccess.entityToEvent
            |> ok
        with ex -> Bad [ DataAccessError ex.Message :> IError ]

module Commands = 
    let queueCommand commands : Result<CommandEnvelope<'TCommand> list, IError> = 
        try 
            DataAccess.queueCommand commands |> ok
        with ex -> Bad [ DataAccessError ex.Message :> IError ]
    
    let dequeueCommands<'TCommand>() : Result<CommandEnvelope<'TCommand> seq, IError> = 
        try 
            typeof<'TCommand>.ToString()
            |> DataAccess.dequeueCommands
            |> ok
        with ex -> Bad [ DataAccessError ex.Message :> IError ]
