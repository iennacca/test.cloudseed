// import useSWR from 'swr'
import { serialize, CookieSerializeOptions } from 'cookie'
import { GetServerSidePropsResult, NextApiRequest, NextApiResponse, Redirect } from 'next'
import { NextApiRequestCookies } from 'next/dist/server/api-utils'
import { useRouter } from 'next/router'
import { useEffect } from 'react'
import useSWR from 'swr'

import { GetSessionResponse } from '../pages/api/session'
import { Session, useAppContext } from '../state/AppContext'
import { getAccessTokenCookieName } from './Configuration'
import { HttpGetRequestAsync } from './http_requests/HttpHandler'
import { THttpResponse } from './http_requests/THttpResponse'
import { getWebBaseUrl } from './Configuration'

type AccessTokenPayload = {
    sub: string, // userId
    iat: number, // creationTime secondsFromEpoch
    exp: number, // expirationTime secondsFromEpoch
}

const CLIENT_ACCESS_TOKEN_EARLY_INVALIDATE_SECONDS: number = 5 * 60

export const getSessionFromApiRequest = (req: NextApiRequest): Session | null => {
    return getSessionFromCookies(req.cookies)
}

export const getSessionFromCookies = (cookies: NextApiRequestCookies): Session | null => {
    let session: Session | null = null

   const accessTokenCookieName = getAccessTokenCookieName()
   if(accessTokenCookieName in cookies) {
       let rawAccessToken = cookies[accessTokenCookieName]

       session = getSessionFromAccessToken(rawAccessToken)
   }

    return session
}

export const getSessionFromAccessToken = (rawAccessToken: string): Session | null => {

    let accessTokenPayload: AccessTokenPayload = parseJwtBase64(rawAccessToken)
    const isTokenValid = checkIsAccessTokenValid(accessTokenPayload)
    if(isTokenValid) {
        return {
            userId: accessTokenPayload.sub,
            accessToken: rawAccessToken
        }
    }

    return null
}

export const setSessionCookiesOnResponse = (
    res: NextApiResponse,
    session: Session
) => {
    let accessTokenName = getAccessTokenCookieName()
    setCookie(
        res,
        accessTokenName,
        session.accessToken
    )
}

export const clearSessionCookiesOnResponse = (
    res: NextApiResponse,
) => {
    let accessTokenName = getAccessTokenCookieName()
    setCookie(
        res,
        accessTokenName,
        '',
        {
            maxAge: 0
        }
    )
}

type ISessionResponse = {
    session: Session | null,
    loading: boolean
}

export function useSession(redirectIfNoSessionTo: string = ''): ISessionResponse {
    const appContext = useAppContext()

    const { data, error } = useSWR<THttpResponse<GetSessionResponse>>(
        '/api/session',
        (url) => HttpGetRequestAsync<GetSessionResponse>(url))
    
    const loading = !data && !error
    const session = data?.responseBody?.session ?? null
    
    const router = useRouter()
    useEffect(() => {
        if(!loading) {
            appContext.setSession(session)
            if(session === null && redirectIfNoSessionTo) {
                router.push(redirectIfNoSessionTo)
            }
        }
    }, [data])

    return {
        session: session,
        loading: loading
    }
}

export const getServerSidePropsNeedsAuthenticationRedirect = (): GetServerSidePropsResult<any> => {
    return {
        // props: {},
        redirect: {
          permanent: false,
          destination: getWebBaseUrl() + '/login',
        } as Redirect,
      }
}

// Pulled from: https://stackoverflow.com/a/46188039/2857326
// Potentially problematic if we use urls / special chars
const parseJwtBase64 = (base64Token: string) => {
    // try {
        let base64Payload = base64Token.split('.')[1]
        let decodedPayload = Buffer.from(base64Payload, 'base64')
        return JSON.parse(decodedPayload.toString());
    //   } catch (e) {
    //     return null;
    //   }
}

const checkIsAccessTokenValid = (accessToken: AccessTokenPayload): boolean => {
    // Check expiration
    // * Early expire if close to expiration to prevent discrepancies with backend AUTH
    const secondsTilExpiration = getSecondsTilAccessTokenExpires(accessToken)
    if(!secondsTilExpiration || secondsTilExpiration < CLIENT_ACCESS_TOKEN_EARLY_INVALIDATE_SECONDS) {
        return false
    }

    return true
}

const getSecondsTilAccessTokenExpires = (accessTokenPayload: AccessTokenPayload): number | null => {
    const expirationTimeSeconds = accessTokenPayload.exp 
    if(!expirationTimeSeconds) {
        return null
    }

    const nowSecondsEpoch = nowSecondsSinceEpoch()
    return expirationTimeSeconds - nowSecondsEpoch
}

const setCookie = (
    res: NextApiResponse,
    name: string,
    value: string,
    options: CookieSerializeOptions = {}
) => {
    res.setHeader(
        'Set-Cookie',
        serialize(
            name,
            value,
            options
        )
    )
}

// Pulled from - https://futurestud.io/tutorials/get-number-of-seconds-since-epoch-in-javascript
const nowSecondsSinceEpoch = () => {
    return Math.round(Date.now() / 1000)
}