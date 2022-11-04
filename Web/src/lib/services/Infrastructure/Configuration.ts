import { DeployEnvironments, parseDeployEnvironmentFromString } from '$lib/services/Infrastructure/environment/DeployEnvironments'

export const getAccessTokenCookieName = (): string => {
    const tokenName = assertIsString(
        'VITE_PUBLIC_ACCESS_TOKEN_COOKIE_NAME',
        import.meta.env.VITE_PUBLIC_ACCESS_TOKEN_COOKIE_NAME,
        getDeployEnvironment() === DeployEnvironments.Development
    )

    return tokenName.length > 0
        ? tokenName
        : 'CLOUDSEED_BUSINESS_ACCESS_TOKEN'
}

export const getAppBaseUrl = (): string => {
    const appBaseUrl = assertIsString(
            'VITE_PUBLIC_APP_BASE_URL',
            import.meta.env.VITE_PUBLIC_APP_BASE_URL,
            getDeployEnvironment() === DeployEnvironments.Development
        ) 

    return appBaseUrl.length > 0
        ? appBaseUrl
        : 'PLACEHOLDER_ENV_VARIABLE'
}

export const getDeployEnvironment = (): DeployEnvironments => {
    const envString = process.env.NODE_ENV
    const deployEnvironment = parseDeployEnvironmentFromString(
        envString
    )
    return deployEnvironment
}

export const getWebBaseUrl = (): string => {
    const webBaseUrl = assertIsString(
        'VITE_PUBLIC_WEB_BASE_URL',
        import.meta.env.VITE_PUBLIC_WEB_BASE_URL,
        getDeployEnvironment() === DeployEnvironments.Development
    )

    return webBaseUrl.length > 0
        ? webBaseUrl
        : 'PLACEHOLDER_ENV_VARIABLE'
}

export const tryGetGoogleAnalyticsId = (): string => {
    const googleId = assertIsString(
        'VITE_PUBLIC_GOOGLE_ANALYTICS_ID',
        import.meta.env.VITE_PUBLIC_GOOGLE_ANALYTICS_ID,
        getDeployEnvironment() === DeployEnvironments.Development
    )

    return googleId.length > 0
        ? googleId
        : 'PLACEHOLDER_ENV_VARIABLE'
}

const assertIsString = (key: string, value: any, shouldThrow: boolean = true): string => {
    if(typeof value !== 'string') {
        const message = 'Environment variable is not a string! variable: ' + key
        if(shouldThrow) {
            throw new Error(
                message
            )
        } else {
            console.error(message)
            return ''
        }
    } else {
        return value
    }
}