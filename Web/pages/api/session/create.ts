import { NextApiRequest, NextApiResponse } from 'next'

import { Session } from '../../../state/AppContext'
import { getAppBaseUrl } from '../../../services/Configuration'
import { setSessionCookiesOnResponse } from '../../../services/Authentication'
import { HttpGetRequestAsync } from '../../../services/http_requests/HttpHandler'
import { ErrorHttpResponse } from '../../../services/http_requests/ErrorHttpResponse'

export type CreateSessionResponse = null

type AppLoginValidationResponse = {
    user_id: string,
    access_token: string
}

const sessionRoute = async (req: NextApiRequest, res: NextApiResponse<CreateSessionResponse | ErrorHttpResponse>) => {
    let { token, redirect } = req.query
    if(token === null) {
        return res
            .status(500)
            .json(
            {
                title: "No access token found! Parameter 'token' must be provided.",
                status: 500
            }
        )
    }

    const response = await HttpGetRequestAsync<AppLoginValidationResponse>(
        getAppBaseUrl() + '/login/session/create',
        {
            token: token
        }
    )

    if(response.errorResponse
        || !response.responseBody) {
        return res
            .status(500)
            .json(
                response.errorResponse ?? {
                    title: 'Failed to login!'
            }
        )
    }

    const session = {
        userId: response.responseBody.user_id,
        accessToken: response.responseBody.access_token
    } as Session
    setSessionCookiesOnResponse(
        res,
        session
    )

    let redirectString = 
        redirect 
            ? typeof(redirect) === 'string'
                ? redirect 
                : redirect[0]
            : null
    res.redirect(
        redirectString
        ? redirectString 
        : '/')
}

export default sessionRoute