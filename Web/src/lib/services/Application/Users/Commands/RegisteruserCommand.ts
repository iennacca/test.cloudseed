import { getAppBaseUrl } from '$lib/services/Infrastructure/Configuration'
import { HttpPostRequestAsync } from '$lib/services/Infrastructure/http_requests/HttpHandler'
import type { THttpResponse } from '$lib/services/Infrastructure/http_requests/THttpResponse'

type RegisterUserCommand = {
    email_address: string
}

export const registerUserCommandAsync = async (userData: RegisterUserCommand): Promise<THttpResponse<null>> => {
    const url = getAppBaseUrl() + '/users/register'
    const response = await HttpPostRequestAsync<RegisterUserCommand, null>(
        url,
        userData
    );
    
    return response
}