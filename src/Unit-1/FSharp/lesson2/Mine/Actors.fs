module Actors

open System
open Akka.Actor
open Akka.FSharp
open Messages
open Utility

[<Literal>]
let ExitCommand = "exit"

let consoleReaderActor (consoleWriter: ActorRef) (mailbox: Actor<_>) (message:obj) = 
    let self = mailbox.Self

    let isValid s = String.length s % 2 = 0

    let getAndValidateInput () = 
        let line = Console.ReadLine()
        match line with 
        |ExitCommand -> mailbox.Context.System.Shutdown ()
        |s when s |> String.IsNullOrEmpty -> self <! InputFailure(InputNull, "Input was null or empty")
        |_ -> 
            if line |> isValid  
            then self <! InputSuccess("Thanks for your input")
            else self <! InputFailure(ValidationFailed, "Input did not have an even number of characters")
            
    match message with 
    | :? StartCommand -> 
        do printInstructions() |> ignore
        self <! ContinueProcessing
    | :? InputResult -> 
        consoleWriter <! message
        self <! ContinueProcessing
    |_ -> getAndValidateInput()
        
        

let consoleWriterActor (message) = 
    
    let printInColor color message =
        Console.ForegroundColor <- color
        Console.WriteLine (message.ToString ())
        Console.WriteLine()
        Console.ResetColor ()

    match message with
    | InputFailure(InputNull, reason) -> printInColor ConsoleColor.DarkYellow reason
    | InputFailure(ValidationFailed, reason) -> printInColor ConsoleColor.Red reason
    | InputSuccess(reason) -> printInColor ConsoleColor.Green reason