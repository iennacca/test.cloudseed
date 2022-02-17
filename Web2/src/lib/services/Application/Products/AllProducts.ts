import { getDeployEnvironment } from '$lib/services/Infrastructure/Configuration'
import { DeployEnvironments } from '$lib/services/Infrastructure/environment/DeployEnvironments'

type Feature = {
    name: string,
    included: boolean
}

type Subscription = {
    id: string,
    name: string,
    description: string,
    priceYearly: number,
    features: Feature[]
}

const coreFeatures: Feature[] = [
{
    name: 'Full Source Code',
    included: true
},
{
    name: 'Authentication',
    included: true
},
{
    name: 'Payments',
    included: true
},
{
    name: 'Subscriptions',
    included: true
},
{
    name: 'Fully-containerized',
    included: true
},
]

// Note: These products need to be kept in sync with 
// App's products
const productionSubscriptions: Subscription[] = [
    {
        id: 'REAL_SUBSCRIPTION_ONE',
        name: 'Real Product',
        description: 'A product sold with CloudSeed!',
        priceYearly: 50,
        features: [
          ...coreFeatures,
          {
            name: 'A real product',
            included: true
          }
        ]
      }
]

const testSubscriptions: Subscription[] = [
    {
        id: 'TEST_SUBSCRIPTION_ONE',
        name: 'Test Product',
        description: 'A test product sold with CloudSeed!',
        priceYearly: 50,
        features: [
          ...coreFeatures,
          {
            name: 'A fake product',
            included: false
          }
        ]
      }
]

export const getSubscriptionProductDetails = (productId: string): Subscription | null => {
    const product = getSubscriptionsProducts()
        .find(element => element.id === productId)
    
    if(!product) {
        return null
    }

    return product
}

export const getSubscriptionsProducts = (): Subscription[] => {
    let subscriptions = productionSubscriptions;

    if(getDeployEnvironment() === DeployEnvironments.Development) {
        subscriptions = [
        ...subscriptions,
        ...testSubscriptions
        ]
    }

    return subscriptions
}