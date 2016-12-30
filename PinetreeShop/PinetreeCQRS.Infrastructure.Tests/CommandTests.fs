module PinetreeCQRS.Infrastructure.Tests.CommandTests

open PinetreeCQRS.Infrastructure.Types
open System
open Xunit
open FSharpx.Validation
open PinetreeCQRS.Infrastructure.Commands
open PinetreeCQRS.Infrastructure.Events

module TestAggregate = 
    type Commands = 
        | CommandOne of string
        | CommandTwo of string
    
    type Events = 
        | EventOne of string
        | EventTwo of string
    
    type State = 
        { one : string
          two : string }
        static member Zero = 
            { one = ""
              two = "" }
    
    let applyEvent state event = 
        match event with
        | EventOne e -> { state with one = e }
        | EventTwo e -> { state with two = e }
    
    let executeCommand state (c : Commands) = 
        match c with
        | CommandOne c' -> [ EventOne(c') ] |> Success
        | CommandTwo _ -> Failure(c, [ "Failed" ])

module TestAggregateTests = 
    let load t id = Seq.empty
    let commit e = Seq.map (fun e' -> e') e
    let onFailure e = e
    
    let handler = 
        makeHandler { zero = TestAggregate.State.Zero
                      applyEvent = TestAggregate.applyEvent
                      executeCommand = TestAggregate.executeCommand } load commit onFailure
    
    let comparableEvent e : Event<'TEvent> = 
        { e with eventId = Guid.Empty
                 eventNumber = None }

    let comparableFailure f : Failed<'TCommand> =
        { f with eventId = Guid.Empty
                 eventNumber = None }
        
    [<Fact>]
    let ``When Command One is dispatched EventOne is created``() = 
        let aggregateId = Guid.NewGuid()
        let commandData = TestAggregate.CommandOne("one")
        let command = createCommand aggregateId commandData None None None
        
        let expected : Event<'TEvent> = 
            { aggregateId = aggregateId
              eventId = Guid.Empty
              processId = None
              causationId = command.commandId
              correlationId = command.commandId
              eventNumber = None
              payload = TestAggregate.EventOne("one") }
        
        let res = handler command

        match res with
        | Success e -> 
            let actual = Seq.head e |> comparableEvent
            Assert.Equal(expected, actual)                 
        | Failure f -> 
            failwith (f.reason.ToString())

        
    
    [<Fact>]
    let ``When Command Two is dispatched Failed command is created``() = 
        let aggregateId = Guid.NewGuid()
        let commandData = TestAggregate.CommandTwo("two")
        let command = createCommand aggregateId commandData None None None
        
        let expected : Failed<'TEvent> = 
            { aggregateId = aggregateId
              eventId = Guid.Empty
              reason = [ "Failed" ]
              processId = None
              causationId = command.commandId
              correlationId = command.commandId
              eventNumber = None
              payload = TestAggregate.CommandTwo("two") }
        
        let res = handler command
        
        match res with
        | Success e -> 
            failwith "Did not fail"
        | Failure f -> 
            let actual = comparableFailure f
            Assert.Equal(expected, actual)                 
