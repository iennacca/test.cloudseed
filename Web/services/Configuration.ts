import { DeployEnvironments, parseDeployEnvironmentFromString } from "./environment/DeployEnvironments"

export const getAccessTokenCookieName = (): string => {
    return assertIsString(
        'ACCESS_TOKEN_COOKIE_NAME',
        process.env.ACCESS_TOKEN_COOKIE_NAME
    )
}

export const getAppBaseUrl = (): string => {
    return assertIsString(
        'NEXT_PUBLIC_APP_BASE_URL',
        process.env.NEXT_PUBLIC_APP_BASE_URL
    )
}

export const getDeployEnvironment = (): DeployEnvironments => {
    const envString = process.env.NODE_ENV
    const deployEnvironment = parseDeployEnvironmentFromString(
        envString
    )
    return deployEnvironment
}

export const getWebBaseUrl = (): string => {
    return assertIsString(
        'NEXT_PUBLIC_WEB_BASE_URL',
        process.env.NEXT_PUBLIC_WEB_BASE_URL
    )
}

export const tryGetGoogleAnalyticsId = (): string | null => {
    return assertIsString(
        'NEXT_PUBLIC_GOOGLE_ANALYTICS_ID',
        process.env.NEXT_PUBLIC_GOOGLE_ANALYTICS_ID,
        false
    )
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