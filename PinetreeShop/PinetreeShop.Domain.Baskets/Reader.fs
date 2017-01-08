module PinetreeShop.Domain.Baskets.Reader

module Basket = PinetreeShop.Domain.Baskets.BasketAggregate
module Persistence = PinetreeCQRS.Persistence.SqlServer
open Chessie.ErrorHandling
open PinetreeCQRS.Infrastructure.Types
    

let getBasket basketId =
    let events = Seq.map (fun (e : EventEnvelope<Basket.Event>) -> e.Payload) <!> Persistence.Events.loadAggregateEvents Basket.basketCategory 0 basketId
    let state =  Basket.loadBasket <!> events
    state