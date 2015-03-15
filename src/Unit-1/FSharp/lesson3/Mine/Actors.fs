module Actors

open System
open Akka.Actor
open Akka.FSharp
open Messages
open Utility

[<Literal>]
let ExitCommand = "exit"

let validationActor (consoleWriter: ActorRef) (mailbox: Actor<_>) (message:obj) = 
    let self = mailbox.Self
    let isValid s = String.length s % 2 = 0

    let validateInput line = 
        match line with 
        |s when s |> String.IsNullOrEmpty -> 
            consoleWriter <! InputFailure(InputNull, "Input was null or empty")
        |_ ->
            if line |> isValid  
            then consoleWriter <! InputSuccess("Thanks for your input")
            else consoleWriter <! InputFailure(ValidationFailed, "Input did not have an even number of characters")

    match message with 
    | :? string as input -> validateInput (input)
    |_ -> ()

    mailbox.Sender() <! ContinueProcessing

let consoleReaderActor (consoleWriter: ActorRef) (validationActor: ActorRef) (mailbox: Actor<_>) (message:obj) = 
    let self = mailbox.Self

    let getLine () = Console.ReadLine()

    let processInput line = 
        match line with 
        |ExitCommand -> mailbox.Context.System.Shutdown ()
        |_ -> validationActor <! line

    match message with 
    | :? StartCommand -> 
        do printInstructions() |> ignore
        self <! ContinueProcessing
    |_ -> (getLine >> processInput) ()
        

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