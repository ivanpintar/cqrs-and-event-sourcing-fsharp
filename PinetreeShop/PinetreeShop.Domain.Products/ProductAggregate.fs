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
    | ChangeQuantity of int
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

type State = 
    { created : bool
      quantity : int
      reserved : int }
    static member Zero = 
        { created = false
          quantity = 0
          reserved = 0 }

let available state = state.quantity - state.reserved

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
    let notCreated s = validator (fun s' -> s'.created = false) "Product already created" s
    let created s = validator (fun s' -> s'.created) "Product must be created first" s
    let positiveQuantity (s, q) cmd = created s cmd <* validator (fun q' -> q' > 0) "Quantity must be a positive number" q cmd
    let canCreate (s, p) cmd = notCreated s cmd <* validator (fun p' -> p' >= 0m) "Price must be a positive number" p cmd
    let canChangeQuantity (s, diff) cmd = created s cmd <* validator (fun (s', diff') -> (diff' < 0 && available s' >= -diff') || diff >= 0) "Not enough available items" (s, diff) cmd
    let enoughReservedItems (s, q) cmd = created s cmd <* positiveQuantity (s, q) cmd <* validator (fun (s', q) -> s'.reserved >= q) "Not enough resreved items" (s, q) cmd


let executeCommand state command = 
    match command with
    | Create(name, price) -> command |> Validate.canCreate (state, price) <?> [ ProductCreated(name, price) ]
    | AddToStock qty -> [ ProductQuantityChanged(qty) ] |> Success
    | RemoveFromStock qty -> command |> Validate.canChangeQuantity (state, -qty) <?> [ ProductQuantityChanged(-qty) ]
    | ChangeQuantity diff -> command |> Validate.canChangeQuantity (state, diff) <?> [ ProductQuantityChanged(diff) ]
    | CancelReservation qty -> command |> Validate.enoughReservedItems (state, qty) <?> [ ProductReservationCanceled(qty) ]
    | PurchaseReserved qty -> command |> Validate.enoughReservedItems (state, qty) <?> [ ProductPurchased(qty) ]
    | Reserve qty -> 
        let r = command |> Validate.canChangeQuantity (state, -qty) 
        match r with
        | Success s -> [ProductReserved(qty)] |> Success
        | Failure (c, f) -> [ProductReservationFailed(qty, f)] |> Success


let commandHandler = 
    makeHandler { zero = State.Zero
                  applyEvent = applyEvent
                  executeCommand = executeCommand } 