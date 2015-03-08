module Messages

type StartCommand = |StartCommand
type ContinueCommand = |ContinueCommand

type InputError =
    |InputNull
    |ValidationFailed

type InputResult = 
    |InputSuccess of reason:string
    |InputFailure of InputError * reason:string

