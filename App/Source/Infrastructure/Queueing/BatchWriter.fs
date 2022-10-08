namespace CloudSeedApp

open System
open System.Collections.Generic
open System.Linq

module BatchWriter =

    let createBatchWriter<'TMessage> (batchSize : int) (writeFnAsync : (list<'TMessage>) -> Async<unit>) = 
        let agent = MailboxProcessor.Start(fun inbox -> 
            let rec loop (messageList : list<'TMessage>) = 
                async {
                    let! message = inbox.Receive()
                    let newMessageList = message::messageList 

                    // printfn "newMessageList count: %A" newMessageList.Length
                    match newMessageList with
                    | newMessageList when (newMessageList.Length >= batchSize 
                        || inbox.CurrentQueueLength = 0) ->
                        do! writeFnAsync newMessageList 
                        return! loop []
                    | _ -> return! loop newMessageList
                }
            // printfn "Starting mailbox"
            loop [])

        fun (message : 'TMessage) ->
            agent.Post message