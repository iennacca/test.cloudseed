
import type { RequestEvent, RequestHandler, EndpointOutput } from "@sveltejs/kit";
import type { ErrorHttpResponse } from '$lib/services/Infrastructure/http_requests/ErrorHttpResponse'
import { clearSessionCookiesOnResponse } from "$lib/services/Infrastructure/authentication/AuthenticationService";

export type CreateSessionResponse = null

export const get:RequestHandler = async (event: RequestEvent): Promise<EndpointOutput<CreateSessionResponse | ErrorHttpResponse>> => {   

    // setSessionCookiesOnResponse()
    const apiResponse = {
        status: 200,
        headers: { Location: '/' },
        body: null
    }
    clearSessionCookiesOnResponse(apiResponse)

    return apiResponse
}