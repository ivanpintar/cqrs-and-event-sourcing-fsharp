module PinetreeCQRS.Infrastructure.Tests.CommandTests

open PinetreeCQRS.Infrastructure.Types
open System
open Xunit
open PinetreeCQRS.Infrastructure.Commands
open PinetreeCQRS.Infrastructure.Events
open PinetreeCQRS.Infrastructure.Tests.Helpers
open Chessie.ErrorHandling

module TestAggregate = 
    type Command = 
        | CommandOne of string
        | CommandTwo of string
        | CommandThree of string
        interface ICommand
    
    type Event = 
        | EventOne of string
        | EventTwo of string
        interface IEvent

    type Failure =
        | FailureOne of string
        | FailureTwo of string
        interface IError
    
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
        | CommandOne c -> ok [ EventOne(c) ]
        | CommandTwo c-> Bad [ FailureOne c :> IError ]
        | CommandThree c -> Bad [ FailureTwo c :> IError ]

module TestAggregateTests = 
    let checkSuccess expected res = 
        match res with
        | Ok(e, _) -> 
            let actual = List.head e |> comparableEvent
            Assert.Equal(expected, actual)
        | Bad f -> failwith "aaa"
    
    let checkFailure expected res = 
        match res with
        | Ok _ -> failwith "Did not fail"
        | Bad f -> Assert.Equal<IError>(expected, f)
    
    let handleCommand initialEvents cmd = 
        let lastEventNumber = Seq.fold (fun acc e -> e.EventNumber) 0 initialEvents
        let load id = ok initialEvents
        let commit = List.map (fun e' -> { e' with EventNumber = lastEventNumber + 1 }) >> ok
        
        let handler = 
            makeCommandHandler { Zero = TestAggregate.State.Zero
                                 ApplyEvent = TestAggregate.applyEvent
                                 ExecuteCommand = TestAggregate.executeCommand } load commit
        handler cmd
    
    [<Fact>]
    let ``When Command One is dispatched EventOne is created``() = 
        let aggregateId = Guid.NewGuid() |> AggregateId
        let command = TestAggregate.CommandOne("one") |> createCommand aggregateId (Expected(0), None, None, None)
        let expected = TestAggregate.EventOne("one") |> createExpectedEvent (aggregateId, command, 1)
        handleCommand [] command |> checkSuccess expected
    
    [<Fact>]
    let ``When Command One is dispatched again EventOne is created with a new version``() = 
        let aggregateId = Guid.NewGuid() |> AggregateId
        let command = TestAggregate.CommandOne("one 2") |> createCommand aggregateId (Expected(1), None, None, None)
        let initialEvent = TestAggregate.EventOne("one 1") |> createExpectedEvent (aggregateId, command, 1)
        let expected = TestAggregate.EventOne("one 2") |> createExpectedEvent (aggregateId, command, 2)
        handleCommand [ initialEvent ] command |> checkSuccess expected
    
    [<Fact>]
    let ``When Command One is dispatched again with the same version Fail with version mismatch``() = 
        let aggregateId = Guid.NewGuid() |> AggregateId
        let command = TestAggregate.CommandOne("one 2") |> createCommand aggregateId (Expected(0), None, None, None)
        let initialEvent1 = TestAggregate.EventOne("one 1") |> createExpectedEvent (aggregateId, command, 1)
        let initialEvent2 = TestAggregate.EventOne("one 2") |> createExpectedEvent (aggregateId, command, 2)
        handleCommand [ initialEvent1; initialEvent2 ] command |> checkFailure [ Error "Version mismatch" ]
    
    [<Fact>]
    let ``When Command One is dispatched again with an irrelevant version produce event``() = 
        let aggregateId = Guid.NewGuid() |> AggregateId
        let command = TestAggregate.CommandOne("one 3") |> createCommand aggregateId (Irrelevant, None, None, None)
        let initialEvent1 = TestAggregate.EventOne("one 1") |> createExpectedEvent (aggregateId, command, 1)
        let initialEvent2 = TestAggregate.EventOne("one 2") |> createExpectedEvent (aggregateId, command, 2)
        let expected = TestAggregate.EventOne("one 3") |> createExpectedEvent (aggregateId, command, 3)
        handleCommand [ initialEvent1; initialEvent2 ] command |> checkSuccess expected
    
    [<Fact>]
    let ``When Command Two is dispatched FailureOne is created``() = 
        let aggregateId = Guid.NewGuid() |> AggregateId
        let command = TestAggregate.CommandTwo("two") |> createCommand aggregateId (Irrelevant, None, None, None)
        handleCommand [] command |> checkFailure [ TestAggregate.FailureOne "two" ]

    [<Fact>]
    let ``When Command Three is dispatched FailureTwo is created``() = 
        let aggregateId = Guid.NewGuid() |> AggregateId
        let command = TestAggregate.CommandThree("three") |> createCommand aggregateId (Irrelevant, None, None, None)
        handleCommand [] command |> checkFailure [ TestAggregate.FailureTwo "three" ]
