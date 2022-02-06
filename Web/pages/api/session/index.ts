import { NextApiRequest, NextApiResponse } from 'next'

import { Session } from '../../../state/AppContext'
import { clearSessionCookiesOnResponse, getSessionFromApiRequest } from '../../../services/Authentication'
import { HttpGetRequestAsync } from '../../../services/http_requests/HttpHandler'
import { getAppBaseUrl } from '../../../services/Configuration'

export type GetSessionResponse = {
    session: Session | null
}

const sessionRoute = async (req: NextApiRequest, res: NextApiResponse<GetSessionResponse>) => {
    const session = getSessionFromApiRequest(req)

    if(session === null) {
        clearSessionCookiesOnResponse(res)
        return res.json(
            {
                session: null
            }
        )
    }

    const isTokenValid = await validateAccessTokenWithServerAsync(session.accessToken)
    if(!isTokenValid) {
        clearSessionCookiesOnResponse(res)
        return res.json(
            {
                session: null
            }
        )
    }

    return res.json(
        {
            session: session
        }
    )
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

export default sessionRoute