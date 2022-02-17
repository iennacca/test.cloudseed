
import type { RequestEvent, RequestHandler, EndpointOutput } from "@sveltejs/kit";
import { HttpGetRequestAsync } from "$lib/services/Infrastructure/http_requests/HttpHandler";
import { getWebBaseUrl, getAppBaseUrl } from "$lib/services/Infrastructure/Configuration";
import type { THttpResponse } from '$lib/services/Infrastructure/http_requests/THttpResponse'
import type { ErrorHttpResponse } from '$lib/services/Infrastructure/http_requests/ErrorHttpResponse'
import type { AuthenticationSession } from '$lib/services/Infrastructure/authentication/AuthenticationSession'
import { setSessionCookiesOnResponse } from "$lib/services/Infrastructure/authentication/AuthenticationService";

export type CreateSessionResponse = null

type AppLoginValidationResponse = {
    user_id: string,
    access_token: string
}

export const get:RequestHandler = async (event: RequestEvent): Promise<EndpointOutput<CreateSessionResponse | ErrorHttpResponse>> => {   
    const token = event.url.searchParams.get("token")

    if(token === null) {
        return {
            status: 500,
            body: {
                status: 500,
                title: "No token provided"
            }
        }
    }

    const response = await HttpGetRequestAsync<AppLoginValidationResponse>(
        getAppBaseUrl() + '/login/session/create',
        {
            token: token
        }
    )

    if(response.errorResponse || !response.responseBody) {
        return {
            status: 500,
            body: response.errorResponse
        }
    }

    const session: AuthenticationSession = {
        userId: response.responseBody.user_id,
        accessToken: response.responseBody.access_token
    }

    // setSessionCookiesOnResponse()
    const apiResponse = {
        status: 302,
        headers: { Location: '/' },
        body: null
    }
    setSessionCookiesOnResponse(apiResponse, session)

    return apiResponse
}

// export type CreateSessionResponse = null

// type AppLoginValidationResponse = {
//     user_id: string,
//     access_token: string
// }

// const sessionRoute = async (req: NextApiRequest, res: NextApiResponse<CreateSessionResponse | ErrorHttpResponse>) => {
//     let { token, redirect } = req.query
//     if(token === null) {
//         return res
//             .status(500)
//             .json(
//             {
//                 title: "No access token found! Parameter 'token' must be provided.",
//                 status: 500
//             }
//         )
//     }

//     const response = await HttpGetRequestAsync<AppLoginValidationResponse>(
//         getAppBaseUrl() + '/login/session/create',
//         {
//             token: token
//         }
//     )

//     if(response.errorResponse
//         || !response.responseBody) {
//         return res
//             .status(500)
//             .json(
//                 response.errorResponse ?? {
//                     title: 'Failed to login!'
//             }
//         )
//     }

//     const session = {
//         userId: response.responseBody.user_id,
//         accessToken: response.responseBody.access_token
//     } as Session
//     setSessionCookiesOnResponse(
//         res,
//         session
//     )

//     let redirectString = 
//         redirect 
//             ? typeof(redirect) === 'string'
//                 ? redirect 
//                 : redirect[0]
//             : null
//     res.redirect(
//         redirectString
//         ? redirectString 
//         : '/')
// }

// export default sessionRoute