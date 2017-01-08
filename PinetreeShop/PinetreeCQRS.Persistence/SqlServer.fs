module PinetreeCQRS.Persistence.SqlServer

open System
open System.Linq
open PinetreeCQRS.Infrastructure.Types
open FSharp.Data.Sql
open Chessie.ErrorHandling
open Newtonsoft.Json

type DataAccessError = 
    | DataAccessError of string
    interface IError

module private DataAccess = 
    type dbSchema = SqlDataProvider< ConnectionStringName="EventStore", UseOptionTypes=true >
    
    let ctx = dbSchema.GetDataContext()
    
    let entityToEvent<'TEvent when 'TEvent :> IEvent> (e : dbSchema.dataContext.``EventStore.EventEntity``) = 
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
          Payload = JsonConvert.DeserializeObject<'TEvent>(e.EventPayload) }
    
    let entityToCommand<'TCommand when 'TCommand :> ICommand> (e : dbSchema.dataContext.``EventStore.CommandEntity``) = 
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
          Payload = JsonConvert.DeserializeObject<'TCommand>(e.CommandPayload) }
    
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
        let payload = JsonConvert.SerializeObject(e.Payload)
        let entity = ctx.EventStore.Event.Create(aggId, category, causeId, corrId, evtId, payload)
        entity.ProcessId <- processIdToGuid e.ProcessId
        entity
    
    let commandToEntity<'TCommand when 'TCommand :> ICommand> (e : CommandEnvelope<'TCommand>) = 
        let queueName = typeof<'TCommand>.ToString()
        let (AggregateId aggId) = e.AggregateId
        let (CommandId cmdId) = e.CommandId
        let (CausationId causeId) = e.CausationId
        let (CorrelationId corrId) = e.CorrelationId
        let payload = JsonConvert.SerializeObject(e.Payload)
        let entity = ctx.EventStore.Command.Create(aggId, causeId, cmdId, payload, corrId, queueName)
        entity.ProcessId <- processIdToGuid e.ProcessId
        entity.ExpectedVersion <- match e.ExpectedVersion with
                                  | Expected vn -> Some vn
                                  | Irrelevant -> None
        entity
    
    let commitEvents events =
        let entities = Seq.toList events |> List.map eventToEntity 
        ctx.SubmitUpdates()
        Seq.map entityToEvent entities
    
    let loadEvents number = 
        query { 
            for e in ctx.EventStore.Event do
                where (e.Id > number)
                select e
        }
    
    let loadTypeEvents typ number = 
        query { 
            for e in (loadEvents number)do
                where (e.Category = typ.ToString())
                select e
        }
    
    let loadAggregateEvents typ number (AggregateId aggregateId) = 
        query { 
            for e in (loadTypeEvents typ number) do
                where (e.AggregateId = aggregateId)
                select e
        }

    let loadProcessEvents number (ProcessId processId) = 
        
        query { 
            for e in ctx.EventStore.Event do
                where (e.Id > number && e.ProcessId = Some processId)
                select e
        }
    
    let queueCommand commands = 
        let entities = Seq.toList commands |> List.map commandToEntity 
        ctx.SubmitUpdates()
        Seq.map entityToCommand entities
    
    let dequeueCommands queueName = 
        let cmds = 
            query { 
                for c in ctx.EventStore.Command do
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
    
    let loadAllEvents number : Result<EventEnvelope<'TEvent> seq, IError> = 
        try 
            DataAccess.loadEvents number
            |> Seq.toList
            |> Seq.map DataAccess.entityToEvent
            |> ok
        with ex -> Bad [ DataAccessError ex.Message :> IError ]
    
    let loadTypeEvents<'TEvent when 'TEvent :> IEvent> number : Result<EventEnvelope<'TEvent> seq, IError> = 
        try 
            DataAccess.loadTypeEvents typeof<'TEvent> number
            |> Seq.toList
            |> Seq.map DataAccess.entityToEvent
            |> ok
        with ex -> Bad [ DataAccessError ex.Message :> IError ]
    
    let loadAggregateEvents<'TEvent when 'TEvent :> IEvent> number aggregateId : Result<EventEnvelope<'TEvent> seq, IError> = 
        try 
            DataAccess.loadAggregateEvents typeof<'TEvent> number aggregateId 
            |> Seq.toList
            |> Seq.map DataAccess.entityToEvent
            |> ok
        with ex -> Bad [ DataAccessError ex.Message :> IError ]

    let loadProcessEvents number processId =
        try 
            DataAccess.loadProcessEvents number processId
            |> Seq.toList
            |> Seq.map DataAccess.entityToEvent
            |> ok
        with ex -> Bad [ DataAccessError ex.Message :> IError ]

module Commands = 
    let queueCommand commands : Result<CommandEnvelope<ICommand> seq, IError> = 
        try 
            DataAccess.queueCommand commands |> ok
        with ex -> Bad [ DataAccessError ex.Message :> IError ]
    
    let dequeueCommands<'TCommand when 'TCommand :> ICommand>() : Result<CommandEnvelope<'TCommand> seq, IError> = 
        try 
            typeof<'TCommand>.ToString()
            |> DataAccess.dequeueCommands
            |> ok
        with ex -> Bad [ DataAccessError ex.Message :> IError ]
