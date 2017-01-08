module PinetreeCQRS.Persistence.SqlServer

open System
open System.Linq
open PinetreeCQRS.Infrastructure.Types
open FSharp.Data.Sql
open Chessie.ErrorHandling
open Newtonsoft.Json
open Microsoft.FSharp.Reflection
open System.Transactions

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
          Payload = JsonConvert.DeserializeObject(e.EventPayload, Type.GetType(e.PayloadType)) :?> 'TEvent}
    
    let entityToCommand<'TCommand when 'TCommand :> ICommand> (c : dbSchema.dataContext.``EventStore.CommandEntity``) = 
        let pid = 
            match c.ProcessId with
            | Some p -> Some(ProcessId p)
            | None -> None
        
        let version = 
            match c.ExpectedVersion with
            | Some vn -> Expected vn
            | _ -> Irrelevant
            
        { ExpectedVersion = version
          CommandId = CommandId c.CommandId
          AggregateId = AggregateId c.AggregateId
          CausationId = CausationId c.CausationId
          CorrelationId = CorrelationId c.CorrelationId
          ProcessId = pid
          Payload = JsonConvert.DeserializeObject(c.CommandPayload, Type.GetType(c.PayloadType)) :?> 'TCommand }
    
    let processIdToGuid pid = 
        match pid with
        | Some pid -> 
            let (ProcessId id) = pid
            Some id
        | _ -> None
    
    let eventToEntity<'TEvent when 'TEvent :> IEvent> (Category category) (e : EventEnvelope<'TEvent>) = 
        let typeName = e.Payload.GetType().AssemblyQualifiedName
        let (AggregateId aggId) = e.AggregateId
        let (EventId evtId) = e.EventId
        let (CausationId causeId) = e.CausationId
        let (CorrelationId corrId) = e.CorrelationId
        let payload = JsonConvert.SerializeObject(e.Payload)
        let entity = ctx.EventStore.Event.Create(aggId, category, causeId, corrId, evtId, payload, typeName)
        entity.ProcessId <- processIdToGuid e.ProcessId
        entity
    
    let commandToEntity<'TCommand when 'TCommand :> ICommand> (QueueName queueName) (c : CommandEnvelope<'TCommand>) = 
        let typeName = c.Payload.GetType().AssemblyQualifiedName
        let (AggregateId aggId) = c.AggregateId
        let (CommandId cmdId) = c.CommandId
        let (CausationId causeId) = c.CausationId
        let (CorrelationId corrId) = c.CorrelationId
        let payload = JsonConvert.SerializeObject(c.Payload)
        let entity = ctx.EventStore.Command.Create(aggId, causeId, cmdId, payload, corrId, typeName, queueName)
        entity.ProcessId <- processIdToGuid c.ProcessId
        entity.ExpectedVersion <- match c.ExpectedVersion with
                                  | Expected vn -> Some vn
                                  | Irrelevant -> None
        entity
    
    let commitEvents category events = 
        let entities = List.map (eventToEntity category) events
        ctx.SubmitUpdates()
        List.map entityToEvent entities
    
    let loadEvents number = 
        query { 
            for e in ctx.EventStore.Event do
                where (e.Id > number)
                select e
        }
    
    let loadTypeEvents (Category category) fromNumber = 
        query { 
            for e in (loadEvents fromNumber) do
                where (e.Category = category)
                select e
        }
    
    let loadAggregateEvents category fromNumber (AggregateId aggregateId) = 
        query { 
            for e in (loadTypeEvents category fromNumber) do
                where (e.AggregateId = aggregateId)
                select e
        }
    
    let loadProcessEvents fromNumber toNumber (ProcessId processId) = 
        query { 
            for e in (loadEvents fromNumber) do
                where (e.Id <= toNumber && e.ProcessId = Some processId)
                select e
        }
    
    let queueCommands commands = 
        let entities = List.map (fun (queueName, c) -> commandToEntity queueName c) commands
        ctx.SubmitUpdates()
        List.map entityToCommand entities
    
    let dequeueCommands (QueueName queueName) = 
        use scope = new TransactionScope()
        let cmds = 
            query { 
                for c in ctx.EventStore.Command do
                    where (c.QueueName = queueName)
                    select c
            }
            |> Seq.toList
        cmds |> List.iter (fun c -> c.Delete())
        ctx.SubmitUpdates()
        scope.Complete()
        scope.Dispose()
        cmds

module Events = 
    let commitEvents<'TEvent when 'TEvent :> IEvent> category (events : EventEnvelope<'TEvent> list) : Result<EventEnvelope<'TEvent> list, IError> = 
        try 
            DataAccess.commitEvents category events |> ok
        with ex -> Bad [ DataAccessError ex.Message :> IError ]
    
    let loadAllEvents number : Result<EventEnvelope<IEvent> list, IError> = 
        try 
            DataAccess.loadEvents number
            |> Seq.toList
            |> List.map DataAccess.entityToEvent
            |> ok
        with ex -> Bad [ DataAccessError ex.Message :> IError ]
    
    let loadTypeEvents category number : Result<EventEnvelope<'TEvent> list, IError> = 
        try 
            DataAccess.loadTypeEvents category number
            |> Seq.toList
            |> List.map DataAccess.entityToEvent
            |> ok
        with ex -> Bad [ DataAccessError ex.Message :> IError ]
    
    let loadAggregateEvents category number aggregateId : Result<EventEnvelope<'TEvent> list, IError> = 
        try 
            DataAccess.loadAggregateEvents category number aggregateId
            |> Seq.toList
            |> List.map DataAccess.entityToEvent
            |> ok
        with ex -> Bad [ DataAccessError ex.Message :> IError ]
    
    let loadProcessEvents fromNumber toNumber processId = 
        try 
            DataAccess.loadProcessEvents fromNumber toNumber processId
            |> Seq.toList
            |> List.map DataAccess.entityToEvent
            |> ok
        with ex -> Bad [ DataAccessError ex.Message :> IError ]

module Commands = 
    let queueCommands<'TCommand when 'TCommand :> ICommand> (commands :(QueueName * CommandEnvelope<'TCommand>) list): Result<CommandEnvelope<'TCommand> list, IError> = 
        try 
            DataAccess.queueCommands commands |> ok
        with ex -> Bad [ DataAccessError ex.Message :> IError ]
    
    let dequeueCommands<'TCommand when 'TCommand :> ICommand> queueName : Result<CommandEnvelope<'TCommand> list, IError> = 
        try 
            DataAccess.dequeueCommands queueName
            |> List.map DataAccess.entityToCommand<'TCommand>
            |> ok
        with ex -> Bad [ DataAccessError ex.Message :> IError ]
