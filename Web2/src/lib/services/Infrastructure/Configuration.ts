// import type { DeployEnvironments } from '$lib/services/Infrastructure/environment/DeployEnvironments' 
import { DeployEnvironments, parseDeployEnvironmentFromString } from '$lib/services/Infrastructure/environment/DeployEnvironments'

export const getAccessTokenCookieName = (): string => {
    const tokenName = assertIsString(
        'VITE_PUBLIC_ACCESS_TOKEN_COOKIE_NAME',
        import.meta.env.VITE_PUBLIC_ACCESS_TOKEN_COOKIE_NAME,
        getDeployEnvironment() === DeployEnvironments.Development
    )

    // ham: We set defaults like this because in prod build it takes in vars at build time
    // this means we may not have access to env vars in container in CloudRun - set sane defaults
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
        : 'https://app.cloudseed.xyz'
}

export const getDeployEnvironment = (): DeployEnvironments => {
    const envString = process.env.NODE_ENV
    const deployEnvironment = parseDeployEnvironmentFromString(
        envString
    )
    console.log(deployEnvironment)
    return deployEnvironment
}

export const getWebBaseUrl = (): string => {
    const webBaseUrl = assertIsString(
        'VITE_PUBLIC_WEB_BASE_URL',
        import.meta.env.VITE_PUBLIC_WEB_BASE_URL,
        getDeployEnvironment() === DeployEnvironments.Development
    )
    console.log('hamy, webbaseurl: ' + webBaseUrl)

    return webBaseUrl.length > 0
        ? webBaseUrl
        : 'https://cloudseed.xyz'
}

export const tryGetGoogleAnalyticsId = (): string => {
    const googleId = assertIsString(
        'VITE_PUBLIC_GOOGLE_ANALYTICS_ID',
        import.meta.env.VITE_PUBLIC_GOOGLE_ANALYTICS_ID,
        getDeployEnvironment() === DeployEnvironments.Development
    )

    return googleId.length > 0
        ? googleId
        : 'G-YVPKETS06B'
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