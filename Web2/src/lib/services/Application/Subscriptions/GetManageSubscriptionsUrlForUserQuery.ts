import { getAppBaseUrl } from "$lib/services/Infrastructure/Configuration";
import { HttpGetRequestAsync } from "$lib/services/Infrastructure/http_requests/HttpHandler";
import type { THttpResponse } from '$lib/services/Infrastructure/http_requests/THttpResponse'

export type GetManageSubscriptionsUrlQueryResponsePayload = {
    url: string
}

export const sendGetManageSubscriptionsUrlForUserQueryAsync = async (userId: string, accessToken: string): Promise<THttpResponse<GetManageSubscriptionsUrlQueryResponsePayload>> => {
    const response = await HttpGetRequestAsync<GetManageSubscriptionsUrlQueryResponsePayload>(
        // /users/{userId}/subscriptions/manage
        getAppBaseUrl() + '/users/' + userId + '/subscriptions/manage',
       null,
       accessToken=accessToken
    );
    
    return response
}