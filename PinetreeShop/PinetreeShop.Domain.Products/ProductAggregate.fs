module PinetreeShop.Domain.Products.ProductAggregate

open PinetreeCQRS.Infrastructure.Types
open PinetreeCQRS.Infrastructure.Commands
open PinetreeCQRS.Infrastructure.Events
open FSharpx.Validation
open PinetreeCQRS.Infrastructure.Validation
open System

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
    | ProductReservationFailed of int * FailureReason list
    | ProductPurchased of int
    interface IEvent

module private Handlers = 
    type State = 
        { created : bool
          quantity : int
          reserved : int }
        static member Zero = 
            { created = false
              quantity = 0
              reserved = 0 }


    let applyEvent state event = 
        match event with
        | ProductCreated(name, price) -> { state with created = true }
        | ProductQuantityChanged diff -> { state with quantity = state.quantity + diff }
        | ProductReserved qty -> { state with reserved = state.reserved + qty }
        | ProductReservationCanceled qty -> { state with reserved = state.reserved - qty }
        | ProductReservationFailed (qty, reasons) -> state
        | ProductPurchased qty -> 
            { state with reserved = state.reserved - qty
                         quantity = state.quantity - qty }

    module private Validate =
        module private Helpers =
            let available state = state.quantity - state.reserved
            let notCreated s = validator (fun s' -> s'.created = false) "Product already created" s
            let positiveQuantity q = validator (fun q' -> q' > 0) "Quantity must be a positive number" q
            let positivePrice p = validator (fun p' -> p' >= 0m) "Price must be a positive number" p
            let canChangeQuantity (s, d) = validator (fun (s', d') -> (d' < 0 && available s' >= -d') || d' >= 0) "Not enough available items" (s, d)
            let enoughReservedItems (s, q) = validator (fun (s', q') -> s'.reserved >= q') "Not enough reserved items" (s, q)
            let created s = validator (fun s' -> s'.created) "Product must be created" s

        let canCreate (s, p) cmd = Helpers.notCreated s cmd <* Helpers.positivePrice p cmd
        let createdAndPositiveQuantity (s, q) cmd = Helpers.created s cmd <* Helpers.positiveQuantity q cmd
        let createdAndEnoughReservedItems (s, q) cmd = Helpers.created s cmd <* Helpers.positiveQuantity q cmd <* Helpers.enoughReservedItems (s, q) cmd
        let createdAndCanRemoveItems (s, q) cmd = Helpers.created s cmd <* Helpers.positiveQuantity q cmd <* Helpers.canChangeQuantity (s, -q) cmd


    let executeCommand state command = 
        match command with
        | Create(name, price) -> command |> Validate.canCreate (state, price) <?> [ ProductCreated(name, price) ]
        | AddToStock qty -> command |> Validate.createdAndPositiveQuantity (state, qty) <?> [ ProductQuantityChanged(qty) ]
        | RemoveFromStock qty -> command |> Validate.createdAndCanRemoveItems (state, qty) <?> [ ProductQuantityChanged(-qty) ]
        | CancelReservation qty -> command |> Validate.createdAndEnoughReservedItems (state, qty) <?> [ ProductReservationCanceled(qty) ]
        | PurchaseReserved qty -> command |> Validate.createdAndEnoughReservedItems (state, qty) <?> [ ProductPurchased(qty) ]
        | Reserve qty -> 
            let r = command |> Validate.createdAndCanRemoveItems (state, qty) 
            match r with
            | Success s -> [ProductReserved(qty)] |> Success
            | Failure f -> [ProductReservationFailed(qty, f)] |> Success


let commandHandler = 
    makeHandler { zero = Handlers.State.Zero
                  applyEvent = Handlers.applyEvent
                  executeCommand = Handlers.executeCommand } 