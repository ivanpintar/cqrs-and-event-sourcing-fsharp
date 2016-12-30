module PinetreeCQRS.Infrastructure.Validation

open Types
open FSharpx.Validation

let validator pred (error:string) value command = 
    let result = pred value
    match result with
    | true -> Success value
    | false -> Failure(command, [ error ])

/// Given a value, creates a choice 1. (Applicative functor)
let puree = Success

/// Given a function in a choice and a choice value x, applies the function to the value if available, 
/// otherwise propagates the second choice. (Applicative functor)
let apply f x = 
    match f, x with
    | Success f, Success x -> Success(f x)
    | Failure(c, reason), Success x -> Failure(c, reason)
    | Success f, Failure(c, reason) -> Failure(c, reason)
    | Failure(c, reason), Failure(c2, reason2) -> Failure(c, reason @ reason2)

let (<*>) = apply

/// Applies the function to the choice 1 value and returns the result as a choice 1, if matched, 
/// otherwise returns the original choice 2 value. (Functor)
let map f o = 
    match o with
    | Success x -> f x |> puree
    | Failure x -> Failure x

let inline (<!>) f x = map f x
let inline lift2 f a b = f <!> a <*> b

/// Composes two choice types, passing the case-1 type of the right value.
let inline ( *> ) a b = lift2 (fun _ z -> z) a b

/// Composes two choice types, passing the case-2 type of the left value.
let inline (<*) a b = lift2 (fun z _ -> z) a b

/// Composes a choice type with a non choice type.
let inline (<?>) a b = lift2 (fun _ z -> z) a (Success b)

/// Composes a non-choice type with a choice type.
let inline (|?>) a b = lift2 (fun z _ -> z) (Failure a) b
