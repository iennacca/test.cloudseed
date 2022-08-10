
import { HttpGetRequestAsync } from "$lib/services/Infrastructure/http_requests/HttpHandler";
import { getAppBaseUrl } from "$lib/services/Infrastructure/Configuration";
import type { Subscription } from './Subscription'
import type { THttpResponse } from '$lib/services/Infrastructure/http_requests/THttpResponse'

export const sendGetSubscriptionsForUserQueryAsync = async (userId: string, accessToken: string): Promise<THttpResponse<Subscription[]>> => {
    const response = await HttpGetRequestAsync<Subscription[]>(
        _getSubscriptionsForUserUrl(userId),
       null,
       accessToken=accessToken
    );

    console.log(response)
    
    return response
}

const _getSubscriptionsForUserUrl = (userId: string): string => {
    return  getAppBaseUrl() + '/users/' + userId + '/subscriptions'
}