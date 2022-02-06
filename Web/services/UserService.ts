import { getAppBaseUrl } from './Configuration'
import { HttpGetRequestAsync, HttpPostRequestAsync } from './http_requests/HttpHandler'
import { THttpResponse } from './http_requests/THttpResponse'

/* Region: UserInfo */
export interface IGetUserPayload {
    userId: string,
    emailAddress: string,
    gitHubUsername: string
}

export const getUserInfoUrl = (userId: string): string => {
    return  getAppBaseUrl() + '/users/' + userId
}

export const tryGetUserInfo = async (userId: string, accessToken: string): Promise<THttpResponse<IGetUserPayload>> => {
    const response = await HttpGetRequestAsync<IGetUserPayload>(
       getUserInfoUrl(userId),
       null,
       accessToken=accessToken
    );
    
    return response
}

interface IUpdateUserPayload {
    emailAddress: string | null,
    gitHubUsername: string | null
}

export const tryUpdateUserInfo = async (accessToken: string, userId: string, updateUserPayload: IUpdateUserPayload): Promise<THttpResponse<null>> => {
    const response = await HttpPostRequestAsync<IUpdateUserPayload, null>(
        getAppBaseUrl() + '/users/' + userId,
        updateUserPayload,
        accessToken
    );
    
    return response
}
/* EndRegion: UserInfo */

/* Region: UserAuthentication */
interface IRegisterUserPayload {
    email_address: string
}

export const tryRegisterUser = async (userData: IRegisterUserPayload): Promise<THttpResponse<null>> => {
    const response = await HttpPostRequestAsync<IRegisterUserPayload, null>(
        getAppBaseUrl() + '/users/register',
        userData
    );
    
    return response
}

interface ILoginUserPayload {
    email_address: string
}

export const tryLoginUser = async (userData: ILoginUserPayload): Promise<THttpResponse<null>> => {
    const response = await HttpPostRequestAsync<ILoginUserPayload, null>(
        getAppBaseUrl() + '/login',
        userData
    );
    
    return response
}
/* EndRegion: UserAuthentication */