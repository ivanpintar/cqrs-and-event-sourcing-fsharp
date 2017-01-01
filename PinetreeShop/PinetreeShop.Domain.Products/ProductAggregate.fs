module PinetreeShop.Domain.Products.ProductAggregate

open PinetreeCQRS.Infrastructure.Types
open PinetreeCQRS.Infrastructure.Commands
open PinetreeCQRS.Infrastructure.Events
open System
open Chessie.ErrorHandling
open PinetreeCQRS.Infrastructure.Validation

type ProductError =
    | ValidationError of string
    interface IError
    override e.ToString() = sprintf "%A" e

type Command = 
    | Create of string * decimal
    | AddToStock of int
    | RemoveFromStock of int
    | Reserve of int
    | CancelReservation of int
    | PurchaseReserved of int

type Event = 
    | ProductCreated of string * decimal
    | ProductQuantityChanged of int
    | ProductReserved of int
    | ProductReservationCanceled of int
    | ProductReservationFailed of int * IError list
    | ProductPurchased of int
    interface IEvent

module private Handlers = 
    type State = 
        { Created : bool
          Quantity : int
          Reserved : int }
        static member Zero = 
            { Created = false
              Quantity = 0
              Reserved = 0 }


    let applyEvent state event = 
        match event with
        | ProductCreated(name, price) -> { state with Created = true }
        | ProductQuantityChanged diff -> { state with Quantity = state.Quantity + diff }
        | ProductReserved qty -> { state with Reserved = state.Reserved + qty }
        | ProductReservationCanceled qty -> { state with Reserved = state.Reserved - qty }
        | ProductReservationFailed (qty, reasons) -> state
        | ProductPurchased qty -> 
            { state with Reserved = state.Reserved - qty
                         Quantity = state.Quantity - qty }

    module private Validate =
        let inCase predicate error value = 
            let result = predicate value
            match result with
            | false -> ok value
            | true -> Bad [ ValidationError error :> IError ]

        module private Helpers =
            let available state = state.Quantity - state.Reserved
            let notCreated = inCase (fun s' -> s'.Created) "Product already created"
            let positiveQuantity = inCase (fun q' -> q' <= 0) "Quantity must be a positive number"
            let positivePrice = inCase (fun p' -> p' < 0m) "Price must be a positive number"
            let canChangeQuantity = inCase (fun (s', d') -> not (d' < 0 && available s' >= -d' || d' >= 0)) "Not enough available items"
            let enoughReservedItems = inCase (fun (s', q') -> s'.Reserved < q') "Not enough reserved items"
            let created = inCase (fun s' -> not s'.Created) "Product must be created" 

        let canCreate (s, p) = Helpers.notCreated s <* Helpers.positivePrice p
        let createdAndPositiveQuantity (s, q) = Helpers.created s <* Helpers.positiveQuantity q 
        let createdAndEnoughReservedItems (s, q) = Helpers.created s <* Helpers.positiveQuantity q <* Helpers.enoughReservedItems (s, q)
        let createdAndCanRemoveItems (s, q) = Helpers.created s <* Helpers.positiveQuantity q <* Helpers.canChangeQuantity (s, -q)


    let executeCommand (state:State) command = 
        match command with
        | Create(name, price) -> Validate.canCreate (state, price) <?> [ ProductCreated(name, price) ]
        | AddToStock qty -> Validate.createdAndPositiveQuantity (state, qty) <?> [ ProductQuantityChanged(qty) ]
        | RemoveFromStock qty -> Validate.createdAndCanRemoveItems (state, qty) <?> [ ProductQuantityChanged(-qty) ]
        | CancelReservation qty -> Validate.createdAndEnoughReservedItems (state, qty) <?> [ ProductReservationCanceled(qty) ]
        | PurchaseReserved qty ->  Validate.createdAndEnoughReservedItems (state, qty) <?> [ ProductPurchased(qty) ]
        | Reserve qty -> 
            let r =  Validate.createdAndCanRemoveItems (state, qty) 
            match r with
            | Ok (s, _) -> ok [ProductReserved(qty)] 
            | Bad f -> ok [ProductReservationFailed(qty, f)] 


let makeProductCommandHandler = 
    makeCommandHandler { Zero = Handlers.State.Zero
                         ApplyEvent = Handlers.applyEvent
                         ExecuteCommand = Handlers.executeCommand } 