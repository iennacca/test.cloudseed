namespace CloudSeedApp

module Events =

    type EventHandler<'TIn, 'TOut> = 'TIn -> 'TOut
