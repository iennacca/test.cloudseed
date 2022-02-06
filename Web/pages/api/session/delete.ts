import { NextApiRequest, NextApiResponse } from 'next'

import { clearSessionCookiesOnResponse } from '../../../services/Authentication'
import { ErrorHttpResponse } from '../../../services/http_requests/ErrorHttpResponse'

export type DeleteSessionResponse = null

const deleteSessionRoute = async (req: NextApiRequest, res: NextApiResponse<DeleteSessionResponse | ErrorHttpResponse>) => {
    let { redirect } = req.query
    clearSessionCookiesOnResponse(res)

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

export default deleteSessionRoute