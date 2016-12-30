module PinetreeCQRS.Infrastructure.Events

open System
open Types
    
let createEvent aggregateId payload causationId processId correlationId : Event<'TEvent> = 
    { aggregateId = aggregateId
      payload = payload
      eventId = Guid.NewGuid()
      processId = processId
      causationId = causationId
      correlationId = correlationId
      eventNumber = None }
    
let processEvents handler (events : Event<'TEvent> list) = 
    let handle (e:Event<'TEvent>) = handler e.payload
    List.map handle events
    
let readAndHandleEvents loadEvents handler lastEventNumber = 
    let events = loadEvents lastEventNumber
    let result = processEvents handler events
        
    let rec eventProcessed = 
        function 
        | head :: [] -> (head:Event<'TEvent>).eventNumber
        | head :: tail -> eventProcessed tail
        | [] -> Some(lastEventNumber)
        
    let lastProcessed = 
        let res = eventProcessed events
        match res with
        | Some r -> r
        | _ -> lastEventNumber
        
    (result, lastProcessed)