module PinetreeCQRS.Infrastructure.Tests.CommandTests

open PinetreeCQRS.Infrastructure.Types
open System
open Xunit
open FSharpx.Validation
open PinetreeCQRS.Infrastructure.Commands
open PinetreeCQRS.Infrastructure.Events
open PinetreeCQRS.Infrastructure.Tests.Helpers

module TestAggregate = 
    type Command = 
        | CommandOne of string
        | CommandTwo of string
    
    type Event = 
        | EventOne of string
        | EventTwo of string
        interface IEvent
    
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
    
    let executeCommand state command = 
        match command with
        | CommandOne c -> [ EventOne(c) ] |> Success
        | CommandTwo _ -> Failure([ FailureReason "Failed" ])

module TestAggregateTests = 
    let handleCommand initialEvents cmd = 
        let lastEventNumber = Seq.fold (fun acc e -> e.eventNumber) 0 initialEvents
        let load id = initialEvents
        let commit e = Seq.map (fun e' -> { e' with eventNumber = lastEventNumber + 1 }) e
        let onFailure e = e
        
        let handler = 
            makeHandler { zero = TestAggregate.State.Zero
                          applyEvent = TestAggregate.applyEvent
                          executeCommand = TestAggregate.executeCommand } load commit onFailure
        handler cmd
    
    [<Fact>]
    let ``When Command One is dispatched EventOne is created``() = 
        let aggregateId = Guid.NewGuid() |> AggregateId
        let command = TestAggregate.CommandOne("one") |> createCommand aggregateId (Expected(0), None, None, None)
        let expected = TestAggregate.EventOne("one") |> createExpectedEvent (aggregateId, command, 1)
        let res = handleCommand [] command
        match res with
        | Success e -> 
            let actual = Seq.head e |> comparableEvent
            Assert.Equal(expected, actual)
        | Failure f -> failwith (f.reasons.ToString())
    
    [<Fact>]
    let ``When Command One is dispatched again EventOne is created with a new version``() = 
        let aggregateId = Guid.NewGuid() |> AggregateId
        let command = TestAggregate.CommandOne("one 2") |> createCommand aggregateId (Expected(1), None, None, None)
        let initialEvent = TestAggregate.EventOne("one 1") |> createExpectedEvent (aggregateId, command, 1)
        let expected = TestAggregate.EventOne("one 2") |> createExpectedEvent (aggregateId, command, 2)
        let res = handleCommand [ initialEvent ] command
        match res with
        | Success e -> 
            let actual = Seq.head e |> comparableEvent
            Assert.Equal(expected, actual)
        | Failure f -> failwith (f.reasons.ToString())
    
    [<Fact>]
    let ``When Command One is dispatched again with the same version Fail with version mismatch``() = 
        let aggregateId = Guid.NewGuid() |> AggregateId
        let command = TestAggregate.CommandOne("one 2") |> createCommand aggregateId (Expected(0), None, None, None)
        let initialEvent1 = TestAggregate.EventOne("one 1") |> createExpectedEvent (aggregateId, command, 1)
        let initialEvent2 = TestAggregate.EventOne("one 2") |> createExpectedEvent (aggregateId, command, 2)
        let expected = [ "Version mismatch" ] |> createExpectedFailure (aggregateId, command)
        let res = handleCommand [ initialEvent1; initialEvent2 ] command
        match res with
        | Success e -> failwith "Did not fail"
        | Failure f -> 
            let actual = comparableFailure f
            Assert.Equal(expected, actual)
    
    [<Fact>]
    let ``When Command One is dispatched again with an irrelevant version produce event``() = 
        let aggregateId = Guid.NewGuid() |> AggregateId
        let command = TestAggregate.CommandOne("one 3") |> createCommand aggregateId (Irrelevant, None, None, None)
        let initialEvent1 = TestAggregate.EventOne("one 1") |> createExpectedEvent (aggregateId, command, 1)
        let initialEvent2 = TestAggregate.EventOne("one 2") |> createExpectedEvent (aggregateId, command, 2)
        let expected = TestAggregate.EventOne("one 3") |> createExpectedEvent (aggregateId, command, 3)
        let res = handleCommand [ initialEvent1; initialEvent2 ] command
        match res with
        | Success e -> 
            let actual = Seq.head e |> comparableEvent
            Assert.Equal(expected, actual)
        | Failure f -> failwith (f.reasons.ToString())
    
    [<Fact>]
    let ``When Command Two is dispatched Failed command is created``() = 
        let aggregateId = Guid.NewGuid() |> AggregateId
        let command = TestAggregate.CommandTwo("two") |> createCommand aggregateId (Irrelevant, None, None, None)
        let expected = [ "Failed" ] |> createExpectedFailure (aggregateId, command)
        let res = handleCommand [] command
        match res with
        | Success e -> failwith "Did not fail"
        | Failure f -> 
            let actual = comparableFailure f
            Assert.Equal(expected, actual)
