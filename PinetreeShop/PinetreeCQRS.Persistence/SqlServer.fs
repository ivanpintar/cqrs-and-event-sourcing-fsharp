module PinetreeCQRS.Persistence.SqlServer

open System
open System.Linq
open PinetreeCQRS.Infrastructure.Types
open FSharp.Data.Sql
open Chessie.ErrorHandling

module private DataAccess =
    type Event = {
        Id : int
        AggregateId : Guid
        EventId : Guid
        CausationId : Guid
        CorrelationId : Guid
        EventPayload : string
        ProcessId : Guid
    }

    type dbSchema = SqlDataProvider< ConnectionStringName="EventStore" >
    let ctx = dbSchema.GetDataContext()

    let mapEventToTable event = 
//        let (AggregateId aggregateId) = event.AggregateId
//        let (EventId eventId) = event.EventId
//        let payload=event.Payload
        0

    let commitEvents events =        
        events |> ignore
        

let commitEvents<'TEvent when 'TEvent :> IEvent> (events : EventEnvelope<'TEvent> seq) : Result<EventEnvelope<'TEvent> seq, IError> = 
    Bad [ Error "not implemented" ]

let loadAllEvents (number:EventNumber) : Result<EventEnvelope<'TEvent> seq, IError> = 
    Bad [ Error "not implemented" ]

let loadTypeEvents<'TEvent when 'TEvent :> IEvent> (number:EventNumber) : Result<EventEnvelope<'TEvent> seq, IError> = 
    Bad [ Error "not implemented" ]

let loadAggregateEvents<'TEvent when 'TEvent :> IEvent> (number:EventNumber) (aggregateId:AggregateId): Result<EventEnvelope<'TEvent> seq, IError> = 
    Bad [ Error "not implemented" ]

let dequeueCommands<'TCommand>() : Result<CommandEnvelope<'TCommand> seq, IError> =
    Bad [ Error "not implemented" ]