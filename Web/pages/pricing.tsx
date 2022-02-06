import { useState } from 'react'

import { useSession } from '../services/Authentication'
import { Session } from "../state/AppContext"
import Layout from '../components/layouts/Layout'
import { CheckIcon, MinusIcon } from '@heroicons/react/outline'
import { useRouter } from 'next/router'
import { tryCreateCheckoutAsync } from '../services/CheckoutService'
import ErrorDetails from '../components/status/ErrorDetails'
import { getDeployEnvironment } from '../services/Configuration'
import { DeployEnvironments } from '../services/environment/DeployEnvironments'
import FaqList from '../components/marketing/FaqList'


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
    id: 'myfakeid',
    name: 'Single Application',
    description: 'Build one app with CloudSeed',
    priceYearly: 50,
    features: [
      ...coreFeatures,
      {
        name: 'Build one app',
        included: false
      }
    ]
  },
  {
    id: 'myfakeid',
    name: 'Unlimited Applications',
    description: 'Build unlimited apps with CloudSeed',
    priceYearly: 150,
    features: [
      ...coreFeatures,
      {
        name: 'Build unlimited apps',
        included: true
      }
    ]
  },
]

const testSubscriptions: Subscription[] = [
  {
    id: 'TEST_SUBSCRIPTION_ONE',
    name: 'Test Product',
    description: 'Build unlimited apps with CloudSeed',
    priceYearly: 50,
    features: [
      ...coreFeatures,
      {
        name: 'Build unlimited apps',
        included: true
      }
    ]
  }
]

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

const PricingPage = () => {
  const [errors, setErrors] = useState<string[]>([])
  const sessionResponse = useSession()
  const router = useRouter()

  /*
    * onclickhandler
      * if not signed in, use the regular router.push thing
      * else send the create checout thing
  */
  const getStartedButtonForProductId = (productId: string, session: Session | null) => {
    const onClickHandler = async (
      e: any, 
      session: Session | null,
      productId: string) => {
      e.preventDefault()

      if(session === null) {
        router.push('/register')
        return
      }

      const checkoutResponse = await tryCreateCheckoutAsync(
        session,
        [
          {
            itemId: productId,
            quantity: 1
          }
        ]
      )

      if(checkoutResponse.none) {
        setErrors([checkoutResponse.error!])
        return
      }

      const checkoutUrl = checkoutResponse.value!.checkoutUrl
      window.location.assign(checkoutUrl)
    }

    return (
      <div className="">
          <a
            className="flex items-center justify-center px-5 py-3 border border-transparent text-base font-medium text-white bg-black hover:bg-gray-900"
            aria-describedby="tier-standard"
            href=''
            onClick={(e) => onClickHandler(e, session, productId)}
          >
            Get started
          </a>
      </div>
    )
  }

  return (
    <Layout title="CloudSeed">
      {
        errors.length > 0
          ? <ErrorDetails
              title='An error occurred'
              errorItems={errors} />
          : null
      }
      <div className="py-12 bg-white" id="tech_stack">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="sm:flex sm:flex-col sm:align-center mb-6">
            <h1 className="text-5xl font-extrabold text-gray-900 sm:text-center">Pricing Plans</h1>
            <p className="mt-5 text-xl text-gray-500 sm:text-center">
              Simple pricing so you can launch today.
            </p>
          </div>
          <div className="relative">
            <div className="absolute inset-0 h-3/4" />
            <div className="relative z-10 max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
              <div className="max-w-md mx-auto space-y-4 lg:max-w-5xl lg:grid lg:grid-cols-2 lg:gap-5 lg:space-y-0">
                {getSubscriptionsProducts().map((subscription) => (
                  <div key={subscription.id} className="flex flex-col border-2 border-black overflow-hidden divide-y-2 divide-black">
                    <div className="px-6 py-8 bg-white sm:p-10 sm:pb-6">
                      <div>
                        <h3
                          className="inline-flex px-4 py-1 text-sm font-semibold tracking-wide uppercase bg-indigo-100 text-indigo-600"
                          id="tier-standard"
                        >
                          {subscription.name}
                        </h3>
                      </div>
                      <div className="mt-4 flex items-baseline text-6xl font-extrabold">
                        ${subscription.priceYearly}
                        <span className="ml-1 text-2xl font-medium text-gray-500">/year</span>
                      </div>
                      <p className="mt-5 text-lg text-gray-500">{subscription.description}</p>
                    </div>
                    <div className="flex-1 flex flex-col justify-between px-6 pt-6 pb-8 space-y-6 sm:p-10 sm:pt-6">
                      <ul role="list" className="space-y-4">
                        {subscription.features.map((feature) => (
                          <li key={feature.name} className="flex items-start">
                            <div className="flex-shrink-0">
                              {
                                feature.included 
                                  ? <CheckIcon className="h-6 w-6 text-green-500" aria-hidden="true" />
                                  : <MinusIcon className="h-6 w-6 text-red-500" aria-hidden="true" />
                              }
                            </div>
                            <p className="ml-3 text-base text-gray-800">{feature.name}</p>
                          </li>
                        ))}
                      </ul>
                      {
                        getStartedButtonForProductId(
                          subscription.id,
                          sessionResponse.session
                        )
                      }
                    </div>
                  </div>
                ))}
              </div>
            </div>
          </div>
        </div>
      </div>
      <FaqList />
    </Layout>
  )
}

export default PricingPage
