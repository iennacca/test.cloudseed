export const placeHolder = "iamaplaceholder"

// import type { RequestEvent, RequestHandler, EndpointOutput } from "@sveltejs/kit";

// import type { Session } from '$lib/services/Infrastructure/authentication/Session'
// import type { ErrorHttpResponse } from '$lib/services/Infrastructure/http_requests/ErrorHttpResponse'
// import { setSessionCookiesOnResponse, clearSessionCookiesOnResponse } from "$lib/services/Infrastructure/authentication/AuthenticationService";

// export const get:RequestHandler = async (event: RequestEvent): Promise<EndpointOutput<Session | ErrorHttpResponse>> => {   
//     // const token = event.url.searchParams.get("token")

//     const response = {
//         status: 200,
//         body: null
//     }
    
//     if(!event.locals.session) {
//         const response = {
//             status: 200,
//             body: null
//         }
//         clearSessionCookiesOnResponse(response)
//     }

// }


// import { NextApiRequest, NextApiResponse } from 'next'

// import { Session } from '../../../state/AppContext'
// import { clearSessionCookiesOnResponse, getSessionFromApiRequest } from '../../../services/Authentication'
// import { HttpGetRequestAsync } from '../../../services/http_requests/HttpHandler'
// import { getAppBaseUrl } from '../../../services/Configuration'

// export type GetSessionResponse = {
//     session: Session | null
// }

// const sessionRoute = async (req: NextApiRequest, res: NextApiResponse<GetSessionResponse>) => {
//     const session = getSessionFromApiRequest(req)

//     if(session === null) {
//         clearSessionCookiesOnResponse(res)
//         return res.json(
//             {
//                 session: null
//             }
//         )
//     }

//     const isTokenValid = await validateAccessTokenWithServerAsync(session.accessToken)
//     if(!isTokenValid) {
//         clearSessionCookiesOnResponse(res)
//         return res.json(
//             {
//                 session: null
//             }
//         )
//     }

//     return res.json(
//         {
//             session: session
//         }
//     )
// }

// const validateAccessTokenWithServerAsync = async (accessToken: string): Promise<boolean> => {
//     var response = await HttpGetRequestAsync<any>(
//         getAppBaseUrl() + '/login/session/validate',
//         { token: accessToken },
//         accessToken
//     );

//     return response.errorResponse
//         ? false
//         : true
// }

// export default sessionRoute