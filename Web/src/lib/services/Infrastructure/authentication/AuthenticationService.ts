import { serialize, CookieSerializeOptions } from 'cookie'
import { getAccessTokenCookieName } from "../Configuration"
import type { AuthenticationSession } from "./AuthenticationSession"
import type { AccessTokenPayload } from "./AccessToken"
import type { RequestEvent, EndpointOutput } from '@sveltejs/kit'
import { HttpGetRequestAsync } from '../http_requests/HttpHandler'
import { getAppBaseUrl } from '../Configuration'

const CLIENT_ACCESS_TOKEN_EARLY_INVALIDATE_SECONDS: number = 5 * 60

export const getSessionFromCookiesAsync = async (cookies: {[key: string]: string}): Promise<AuthenticationSession | null> => {
    let session: AuthenticationSession | null = null

   const accessTokenCookieName = getAccessTokenCookieName()
   if(accessTokenCookieName in cookies) {
       let rawAccessToken = cookies[accessTokenCookieName]

       session = await getSessionFromAccessTokenAsync(rawAccessToken)
   }

    return session
}

export const getSessionFromAccessTokenAsync = async (rawAccessToken: string): Promise<AuthenticationSession | null> => {

    let accessTokenPayload: AccessTokenPayload = parseJwtBase64(rawAccessToken)
    const isTokenValidLocal = checkIsAccessTokenValidLocal(accessTokenPayload)
    if(!isTokenValidLocal) {
        return null
    }

    const isTokenValidApp = await validateAccessTokenWithServerAsync(rawAccessToken)
    if(!isTokenValidApp) {
        return null
    }

    return {
        userId: accessTokenPayload.sub,
        accessToken: rawAccessToken
    }
}

export const setSessionCookiesOnResponse = (
    res: EndpointOutput,
    session: AuthenticationSession
) => {
    let accessTokenName = getAccessTokenCookieName()
    setCookie(
        res,
        accessTokenName,
        session.accessToken,
        {
            httpOnly: true,
            path: '/'
        }
    )
}

export const clearSessionCookiesOnResponse = (
    res: EndpointOutput,
) => {
    let accessTokenName = getAccessTokenCookieName()
    setCookie(
        res,
        accessTokenName,
        'NOT_A_TOKEN',
        {
            maxAge: 0
        }
    )
}

const parseJwtBase64 = (base64Token: string) => {
        let base64Payload = base64Token.split('.')[1]
        let decodedPayload = Buffer.from(base64Payload, 'base64')
        return JSON.parse(decodedPayload.toString());
}

const checkIsAccessTokenValidLocal = (accessToken: AccessTokenPayload): boolean => {
    // Check expiration
    // * Early expire if close to expiration to prevent discrepancies with backend AUTH
    const secondsTilExpiration = getSecondsTilAccessTokenExpires(accessToken)
    if(!secondsTilExpiration || secondsTilExpiration < CLIENT_ACCESS_TOKEN_EARLY_INVALIDATE_SECONDS) {
        return false
    }

    return true
}

const validateAccessTokenWithServerAsync = async (accessToken: string): Promise<boolean> => {
    var response = await HttpGetRequestAsync<any>(
        getAppBaseUrl() + '/login/session/validate',
        { token: accessToken },
        accessToken
    );

    return response.errorResponse
        ? false
        : true
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
    res: EndpointOutput,
    name: string,
    value: string,
    options: CookieSerializeOptions = {}
) => {
    res.headers = {
        ...res.headers,
        'set-cookie': serialize(
            name,
            value,
            options
        )
    }
}

// Pulled from - https://futurestud.io/tutorials/get-number-of-seconds-since-epoch-in-javascript
const nowSecondsSinceEpoch = () => {
    return Math.round(Date.now() / 1000)
}