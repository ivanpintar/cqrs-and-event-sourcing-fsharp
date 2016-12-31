module PinetreeCQRS.Persistence.SqlServer

open System
open System.Linq
open PinetreeCQRS.Infrastructure.Types


let commitEvents<'TEvent when 'TEvent :> IEvent> (events : EventEnvelope<'TEvent> seq) : EventEnvelope<'TEvent> seq = 
    failwith "not implemented"

let loadAllEvents (number:EventNumber) : EventEnvelope<IEvent> seq = 
    failwith "not implemented"

let loadTypeEvents<'TEvent when 'TEvent :> IEvent> (number:EventNumber) : EventEnvelope<'TEvent> seq = 
    failwith "not implemented"

let loadAggregateEvents<'TEvent when 'TEvent :> IEvent> (number:EventNumber) (aggregateId:AggregateId): EventEnvelope<'TEvent> seq = 
    failwith "not implemented"

let dequeueCommands<'TCommand>() : CommandEnvelope<'TCommand> seq =
    failwith "not implemented"