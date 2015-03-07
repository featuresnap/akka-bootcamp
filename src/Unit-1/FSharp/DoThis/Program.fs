open System
open Akka.FSharp
open Akka.FSharp.Spawn
open Akka.Actor
open Actors

let printInstructions () =
    Console.WriteLine "Write whatever you want into the console!"
    Console.Write "Some lines will appear as"
    Console.ForegroundColor <- ConsoleColor.DarkRed
    Console.Write " red "
    Console.ResetColor ()
    Console.Write " and others will appear as"
    Console.ForegroundColor <- ConsoleColor.Green
    Console.Write " green! "
    Console.ResetColor ()
    Console.WriteLine ()
    Console.WriteLine ()
    Console.WriteLine "Type 'exit' to quit this application at any time.\n"

[<EntryPoint>]
let main argv = 
    // initialize an actor system
    let myActorSystem = ActorSystem.Create "MyActorSystem" 
    
    printInstructions ()
    
    // make your first actors using the 'spawn' function
    let writerRef = spawn myActorSystem "writer" (actorOf consoleWriterActor)
    let readerRef = spawn myActorSystem "reader" (actorOf2 (consoleReaderActor writerRef))
    
    // tell the consoleReader actor to begin
    readerRef <! "start"

    myActorSystem.AwaitTermination ()
    0
