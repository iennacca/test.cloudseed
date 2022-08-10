import type { THttpResponse } from '$lib/services/Infrastructure/http_requests/THttpResponse'
import { getAppBaseUrl } from '$lib/services/Infrastructure/Configuration';
import { HttpPostRequestAsync } from '$lib/services/Infrastructure/http_requests/HttpHandler'

interface SaveUserInfoCommand {
    userId: string,
    emailAddress: string | null,
    gitHubUsername: string | null
}

export const sendSaveUserInfoCommandAsync = async (accessToken: string, saveUserInfoCommand: SaveUserInfoCommand): Promise<THttpResponse<null>> => {
    const response = await HttpPostRequestAsync<SaveUserInfoCommand, null>(
        getAppBaseUrl() + '/users/' + saveUserInfoCommand.userId,
        saveUserInfoCommand,
        accessToken
    );
    
    return response
}
