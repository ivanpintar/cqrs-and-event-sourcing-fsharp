module PinetreeShop.Domain.Baskets.BasketAggregate

open PinetreeCQRS.Infrastructure.Types
open PinetreeCQRS.Infrastructure.Commands
open PinetreeCQRS.Infrastructure.Events
open System
open Chessie.ErrorHandling
open PinetreeCQRS.Infrastructure.Validation

type BasketError =
    | ValidationError of string
    interface IError
    override e.ToString() = sprintf "%A" e
    
type BasketState =
    | NotCreated
    | Pending
    | Cancelled
    | CheckedOut

type Command = 
    | Create
    | AddItem of BasketItem 
    | RemoveItem of ProductId * int
    | Cancel
    | CheckOut of ShippingAddress
    interface ICommand

type Event = 
    | BasketCreated
    | BasketItemAdded of BasketItem
    | BasketItemRemoved of ProductId * int
    | BasketCancelled
    | BasketCheckedOut of ShippingAddress * BasketItem list
    interface IEvent

type State = 
    { BasketState : BasketState
      Items : Map<ProductId, BasketItem> }
    static member Zero = 
        { BasketState = NotCreated
          Items = Map.empty }

module private Handlers = 

    let addItem currentItems (item:BasketItem) : Map<ProductId, BasketItem>= 
        let currentItem = Map.tryFind item.ProductId currentItems 
        let newItem = 
            match currentItem with
            | None -> item
            | Some i -> { i with Quantity = i.Quantity + item.Quantity }
        Map.add newItem.ProductId newItem currentItems              

    let removeItem currentItems productId quantity : Map<ProductId, BasketItem> =     
        let currentItem = Map.tryFind productId currentItems 
        match currentItem with
        | None -> currentItems
        | Some i ->
            if (i.Quantity - quantity) <= 0 then Map.remove productId currentItems
            else Map.add productId { i with Quantity = i.Quantity - quantity } currentItems


    let applyEvent state event = 
        match event with
        | BasketCreated -> { state with BasketState = Pending }
        | BasketItemAdded item -> { state with Items = addItem state.Items item }
        | BasketItemRemoved (productId, qty) -> { state with Items = removeItem state.Items productId qty }
        | BasketCancelled -> { state with BasketState = Cancelled }
        | BasketCheckedOut _ -> { state with BasketState = CheckedOut }


    module private Validate =
        let inCase predicate error value = 
            let result = predicate value
            match result with
            | false -> ok value
            | true -> Bad [ ValidationError error :> IError ]

        module private Helpers =
            let notEmptyShippingAddress (ShippingAddress sa) = 
                inCase (fun sa -> String.IsNullOrWhiteSpace(sa)) "Shipping Address cannot be empty" sa
            let isBasketState s os = 
                inCase (fun s -> s.BasketState <> os) (sprintf "Wrong Basket state %A" s.BasketState) s
            let notBasketState s os = 
                inCase (fun s -> s.BasketState = os) (sprintf "Wrong Basket state %A" s.BasketState) s
            let hasItems ol = inCase (fun ol -> ol = Map.empty) "No items" ol
            let canCreate s = isBasketState s NotCreated
            let created s = notBasketState s NotCreated
        
        let canCreate s = Helpers.canCreate s
        let canCheckOut s sa = Helpers.isBasketState s Pending <* Helpers.hasItems s.Items <* Helpers.notEmptyShippingAddress sa
        let canCancel s = Helpers.isBasketState s Pending 
        let canAddItem s = Helpers.isBasketState s Pending
        let canRemoveItem s = Helpers.isBasketState s Pending


    let executeCommand (state:State) command = 
        match command with
        | Create -> Validate.canCreate state <?> [ BasketCreated ]
        | AddItem item -> Validate.canAddItem state <?> [ BasketItemAdded item ]
        | RemoveItem (productId, qty) -> 
            let removeItems =
                let item = Map.tryFind productId state.Items
                match item with
                | Some i ->
                    if(i.Quantity <= qty) then [BasketItemRemoved(productId, i.Quantity)]
                    else [ BasketItemRemoved(productId, qty)]
                | None -> []
            Validate.canRemoveItem state <?> removeItems
        | Cancel -> Validate.canCancel state <?> [ BasketCancelled ]
        | CheckOut sa -> 
            let items = Map.toList state.Items |>  List.map (fun (k, v) -> v)
            Validate.canCheckOut state sa <?> [ BasketCheckedOut(sa, items) ]


let makeBasketCommandHandler = 
    makeCommandHandler { Zero = State.Zero
                         ApplyEvent = Handlers.applyEvent
                         ExecuteCommand = Handlers.executeCommand } 

let loadBasket events = Seq.fold Handlers.applyEvent State.Zero events