import { getAppBaseUrl } from "$lib/services/Infrastructure/Configuration"
import { HttpGetRequestAsync } from "$lib/services/Infrastructure/http_requests/HttpHandler"
import type { THttpResponse } from "$lib/services/Infrastructure/http_requests/THttpResponse"

export type GetUserInfoQueryResult = {
    userId: string,
    emailAddress: string,
    gitHubUsername: string
}

const getUserInfoUrl = (userId: string): string => {
    return  getAppBaseUrl() + '/users/' + userId
}

export const getUserInfoQueryAsync = async (userId: string, accessToken: string): Promise<THttpResponse<GetUserInfoQueryResult>> => {
    const response = await HttpGetRequestAsync<GetUserInfoQueryResult>(
       getUserInfoUrl(userId),
       null,
       accessToken=accessToken
    );
    
    return response
}