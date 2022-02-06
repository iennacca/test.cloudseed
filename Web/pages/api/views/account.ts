import { NextApiRequest, NextApiResponse } from 'next'

import { getSessionFromAccessToken } from '../../../services/Authentication'
import { IGetUserPayload, tryGetUserInfo } from '../../../services/UserService'
import { Subscription, tryGetManageSubscriptionsUrlForUser, tryGetSubscriptionsForUser } from '../../../services/SubscriptionService'
import { getWebBaseUrl } from '../../../services/Configuration'

import { HttpGetRequestAsync } from '../../../services/http_requests/HttpHandler'

export type GetAccountResponse = {
    errors: string[],
    manageSubscriptionsUrl: string | null
    subscriptions: Subscription[],
    userInfo: IGetUserPayload | null
}

type GetAccountRequest = {
    accessToken: string
}

export const getAccountUrl = getWebBaseUrl() + '/api/views/account'

export const getAccountApiFetcherAsync = async (requestPayload: GetAccountRequest) => {
    const response = await HttpGetRequestAsync<GetAccountResponse>(
        getAccountUrl,
        requestPayload
    )
    return response;
}

const getAccountApiRoute = async (req: NextApiRequest, res: NextApiResponse<GetAccountResponse>): Promise<void> => {    
    const { accessToken } = req.query
    if(!accessToken) {
        return res.json( 
            {
              errors: ["Not logged in"],
              manageSubscriptionsUrl: null,
              subscriptions: [],
              userInfo: null
            }
        )
    }

    // @ts-ignore
    const session = getSessionFromAccessToken(accessToken)

    if(!session) {
        return res.json( 
            {
              errors: ["Not logged in"],
              manageSubscriptionsUrl: null,
              subscriptions: [],
              userInfo: null
            }
        )
    }
    
      const userInfoResponse = await tryGetUserInfo(
        session.userId,
        session.accessToken
      )
      const userSubscriptionResponse = await tryGetSubscriptionsForUser(
        session.userId,
        session.accessToken
      )
    
      const errors = []
      if(userInfoResponse.errorResponse
        && userInfoResponse.errorResponse.title) {
        errors.push(userInfoResponse.errorResponse.title)
      }
      if(userSubscriptionResponse.errorResponse
        && userSubscriptionResponse.errorResponse.title) {
        errors.push(userSubscriptionResponse.errorResponse.title)
      }
    
      let manageSubscriptionsUrl: string | null = null
      if(userSubscriptionResponse.responseBody) {
        const manageSubscriptionsUrlResponse = await tryGetManageSubscriptionsUrlForUser(
          session.userId,
          session.accessToken
        );
        manageSubscriptionsUrl = manageSubscriptionsUrlResponse.responseBody?.url ?? null
        if(manageSubscriptionsUrlResponse.errorResponse
            && manageSubscriptionsUrlResponse.errorResponse.title) {
          errors.push(manageSubscriptionsUrlResponse.errorResponse.title)
        }
      }

      return res.json( 
        {
          errors: errors,
          manageSubscriptionsUrl: manageSubscriptionsUrl,
          subscriptions: userSubscriptionResponse.responseBody ?? [],
          userInfo: userInfoResponse.responseBody ?? null
        }
      )
}

export default getAccountApiRoute