module PinetreeCQRS.Infrastructure.Validation

open Types
open Chessie.ErrorHandling

/// Composes two choice types, passing the case-1 type of the right value.
let inline ( *> ) a b = lift2 (fun _ z -> z) a b

/// Composes two choice types, passing the case-2 type of the left value.
let inline (<*) a b = lift2 (fun z _ -> z) a b

/// Composes a choice type with a non choice type.
let inline (<?>) a b = lift2 (fun _ z -> z) a (ok b)

/// Composes a non-choice type with a choice type.
let inline (|?>) a b = lift2 (fun z _ -> z) (Bad a) b
