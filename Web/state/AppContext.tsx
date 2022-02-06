import { AppContext } from 'next/app'
import React, { createContext, FunctionComponent, useContext, useState, Dispatch, SetStateAction } from 'react'

interface IAppContext {
    session: Session | null,
    setSession: Dispatch<SetStateAction<Session | null>>
}

export type Session = {
    userId: string,
    accessToken: string
}

const AppContext = createContext({} as IAppContext)

export const AppContextWrapper: FunctionComponent = ({ children }) => {
    const [session, setSession] = useState<Session | null>(null)

    let appContextState: IAppContext = {
        session: session,
        setSession: setSession
    }

    return (
        <AppContext.Provider value={appContextState}>
            { children }
        </AppContext.Provider>
    )
}

export const useAppContext = () => {
    return useContext(AppContext)
}