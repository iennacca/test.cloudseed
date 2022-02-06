
export enum DeployEnvironments {
    Development = 'development',
    Production = 'production'
}

export const parseDeployEnvironmentFromString = (deployEnvironmentString: string): DeployEnvironments => {
    switch(deployEnvironmentString) {
        case DeployEnvironments.Development:
            return DeployEnvironments.Development
        case DeployEnvironments.Production:
            return DeployEnvironments.Production
        default: 
            throw 'Unknown deploy environment!'
    }
}