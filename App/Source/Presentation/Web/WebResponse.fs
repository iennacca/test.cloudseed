namespace CloudSeedApp 

module WebResponse =

    type WebResponseError<'TError> = {
        ErrorCode: 'TError
        // ErrorDescription: string
    }

    type WebResponseSuccess<'TSuccess> = 'TSuccess

    type WebResponse<'TSuccess, 'TError> = 
        | WebError of WebResponseError<'TError>
        | WebSuccess of WebResponseSuccess<'TSuccess>
        