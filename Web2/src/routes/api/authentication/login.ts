import type { RequestEvent, RequestHandler, EndpointOutput } from "@sveltejs/kit";
import { HttpPostRequestAsync } from "$lib/services/Infrastructure/http_requests/HttpHandler";
import { getWebBaseUrl, getAppBaseUrl } from "$lib/services/Infrastructure/Configuration";
import type { THttpResponse } from '$lib/services/Infrastructure/http_requests/THttpResponse'
import type { ErrorHttpResponse } from '$lib/services/Infrastructure/http_requests/ErrorHttpResponse'

export type LoginUserCommandRequest = {
    emailAddress: string
}

export const post:RequestHandler = async (event: RequestEvent): Promise<EndpointOutput<null | ErrorHttpResponse>> => {
    const request: LoginUserCommandRequest = await event.request.json()

    const response = await loginUserAppCommandAsync(
        {
            email_address: request.emailAddress
        }
    )

    if(response.errorResponse) {
        return {
            status: 500,
            body: response.errorResponse
        }
    }

	return {
		status: 200
	};
}

export const loginUserCommandAsync = async (userData: LoginUserCommandRequest): Promise<THttpResponse<null>> => {
    const response = await HttpPostRequestAsync<LoginUserCommandRequest, null>(
        getWebBaseUrl() + '/api/authentication/login',
        userData
    );
    
    return response
}

interface LoginUserAppCommandRequest {
    email_address: string
}

const loginUserAppCommandAsync = async (userData: LoginUserAppCommandRequest): Promise<THttpResponse<null>> => {
    const response = await HttpPostRequestAsync<LoginUserAppCommandRequest, null>(
        getAppBaseUrl() + '/login',
        userData
    );
    
    return response
}