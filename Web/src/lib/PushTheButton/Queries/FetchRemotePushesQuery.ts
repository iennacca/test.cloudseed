import { getAppBaseUrl } from "$lib/services/Infrastructure/Configuration"
import { HttpGetRequestAsync } from "$lib/services/Infrastructure/http_requests/HttpHandler"
import type { THttpResponse } from "$lib/services/Infrastructure/http_requests/THttpResponse"

export type FetchRemotePushesQueryResponse = number

const getRemotePushesQueryUrl = () : string => {
    return  getAppBaseUrl() + '/push-the-button/push/totals'
}

export const getRemotePushesQueryAsync = async (): Promise<THttpResponse<FetchRemotePushesQueryResponse>> => {
    const response = await HttpGetRequestAsync<FetchRemotePushesQueryResponse>(
        getRemotePushesQueryUrl(),
       null,
       ""
    );
    
    return response
}