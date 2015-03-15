open System
open Akka.FSharp
open Akka.FSharp.Spawn
open Akka.Actor
open Utility

[<EntryPoint>]
let main argv = 
    let myActorSystem = System.create "MyActorSystem" (Configuration.load ())
    let consoleWriterActor = spawn myActorSystem "consoleWriterActor" (actorOf Actors.consoleWriterActor)
    let validationActor = spawn myActorSystem "validationActor" (actorOf2 (Actors.validationActor consoleWriterActor))
    let consoleReaderActor = spawn myActorSystem "consoleReaderActor" (actorOf2 (Actors.consoleReaderActor consoleWriterActor validationActor))
    consoleReaderActor <! Messages.StartCommand
    myActorSystem.AwaitTermination ()
    0