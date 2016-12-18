# cqrs and event sourcing

Learning CQRS and Event Sourcing.
This is a small, very contrived shopping app with three aggregates: Basket, Product and Order.
Each aggregate has it's own set of command and events, a read model. Each is managed independently using it's own API and JS web app.
An OrderProcessManager ties these aggregates together by reacting to events and dispatching commands to other aggregates.

Underneath is a very simple CQRS-ES "framework" with a SQLServer database for command queuing and event storage.
The read models are backed by SQLServer db and populated by simple console apps which run event handlers.
