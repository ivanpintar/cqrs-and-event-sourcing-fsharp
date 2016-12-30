﻿module PinetreeShop.Domain.Products.Tests.Base

open PinetreeShop.Domain.Tests.TestBase
open PinetreeCQRS.Infrastructure.Commands
open PinetreeCQRS.Infrastructure.Types

module Product = PinetreeShop.Domain.Products.ProductAggregate

let handleCommand initialEvents cmd = 
    let lastEventNumber = Seq.fold (fun acc e -> e.eventNumber) 0 initialEvents
    let load t id = initialEvents
    let commit e = Seq.map (fun e' -> { e' with eventNumber = lastEventNumber + 1 }) e
    let onFailure e = e
    let handler = Product.commandHandler load commit onFailure
    handler cmd