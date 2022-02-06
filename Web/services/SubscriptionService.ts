import { getAppBaseUrl } from './Configuration'
import { HttpGetRequestAsync } from './http_requests/HttpHandler'
import { THttpResponse } from './http_requests/THttpResponse'

// export interface IGetSubscriptionsForUserPayload {
//     subscriptions: Subscription[]
// }

export type Subscription = {
    productId: string,
    expirationTimestamp: string
}

export const getSubscriptionsForUserUrl = (userId: string): string => {
    return  getAppBaseUrl() + '/users/' + userId + '/subscriptions'
}

export const tryGetSubscriptionsForUser = async (userId: string, accessToken: string): Promise<THttpResponse<Subscription[]>> => {
    const response = await HttpGetRequestAsync<Subscription[]>(
        getSubscriptionsForUserUrl(userId),
       null,
       accessToken=accessToken
    );
    
    return response
}

export type GetManageSubscriptionsUrlResponsePayload = {
    url: string
}

export const tryGetManageSubscriptionsUrlForUser = async (userId: string, accessToken: string): Promise<THttpResponse<GetManageSubscriptionsUrlResponsePayload>> => {
    const response = await HttpGetRequestAsync<GetManageSubscriptionsUrlResponsePayload>(
        // /users/{userId}/subscriptions/manage
        getAppBaseUrl() + '/users/' + userId + '/subscriptions/manage',
       null,
       accessToken=accessToken
    );
    
    return response
}