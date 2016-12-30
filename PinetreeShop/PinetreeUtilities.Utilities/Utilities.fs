module PinetreeUtilities.Utilities

let incrementInteger i = 
    match i with
    | Some i -> Some(i + 1)
    | None -> Some(0)
