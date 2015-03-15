module Messages

type StartCommand = |StartCommand
type ContinueProcessing = |ContinueProcessing

type InputError =
    |InputNull
    |ValidationFailed

type InputResult = 
    |InputFailure of InputError * reason:string
    |InputSuccess of reason:string

