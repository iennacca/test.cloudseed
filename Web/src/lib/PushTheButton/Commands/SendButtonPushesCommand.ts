import { getAppBaseUrl } from "$lib/services/Infrastructure/Configuration"
import { HttpPostRequestAsync } from "$lib/services/Infrastructure/http_requests/HttpHandler"
import type { THttpResponse } from "$lib/services/Infrastructure/http_requests/THttpResponse"

export type SendButtonPushesCommand = {
    hits: number
}

export type SendButtonPushesCommandResponse = {}

const sendButtonPushesCommandUrl = () : string => {
    return  getAppBaseUrl() + '/push-the-button/push'
}

export const sendButtonPushesCommandAsync = async (payload : SendButtonPushesCommand): Promise<THttpResponse<SendButtonPushesCommandResponse>> => {
    const response = await HttpPostRequestAsync<SendButtonPushesCommand, SendButtonPushesCommandResponse>(
        sendButtonPushesCommandUrl(),
       payload,
       ""
    );
    
    return response
}