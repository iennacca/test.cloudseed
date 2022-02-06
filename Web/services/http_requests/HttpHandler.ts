import { ErrorHttpResponse } from "./ErrorHttpResponse"
import { THttpResponse } from "./THttpResponse"

export async function HttpPostRequestAsync<P,T>(
    fullUrl: string,
    payload: P,
    accessToken?: string): Promise<THttpResponse<T>> {
    var body = JSON.stringify(payload)

    let urlOptions = {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: body,
    } as RequestInit

    if(accessToken) {
        urlOptions.headers = {
            ...urlOptions.headers,
            'Authorization': `Bearer ${accessToken}`
        }
    }

    const response = await HandleHttpJsonResponseAsync<T>(
        fetch(
            fullUrl,
            urlOptions
        ))
    return response
}

export async function HttpGetRequestAsync<T>(
    fullUrl: string,
    payload?: any,
    accessToken?: string
): Promise<THttpResponse<T>> {
    const constructedUrl = constructUrlWithQueryParameters(
        fullUrl,
        payload
    )

    let urlOptions = {
        method: 'GET',
    } as RequestInit
    if(accessToken) {
        urlOptions.headers = {
            ...urlOptions.headers,
            'Authorization': `Bearer ${accessToken}`
        }
    }

    const response = await HandleHttpJsonResponseAsync<T>(
        fetch(
            constructedUrl,
            urlOptions
        )
    )
    return response
}

export async function HandleHttpJsonResponseAsync<T>(httpResponsePromise: Promise<Response>): Promise<THttpResponse<T>> {
    const response = await httpResponsePromise;

    if(!response.ok) {
        let rawBody = await response.text()

        console.log('hamy - response')
        console.log(rawBody)

        let body = null
        try {
            body = JSON.parse(rawBody)
        } catch (_) {
            body = {
                title: "Could not parse error response",
                status: 0
            } as ErrorHttpResponse
        }
        
        return {
            errorResponse: body
        }
    } else {
        // We cannot always rely on ContentLength header to be set correctly
        // so we just see if anything is in the body directly
        let rawBody = await response.text()
        let contentLength = rawBody.length

        let body = contentLength > 0
            ? JSON.parse(rawBody)
            : null;

        let typedBody = body as T
        return {
            responseBody: typedBody
        } as THttpResponse<T>
    }
}

export const constructUrlWithQueryParameters = (
    baseUrl: string,
    payload?: any
): string => {
    return baseUrl
        + (payload ? '?' : '')
        + (payload ? new URLSearchParams(payload) : '')
}