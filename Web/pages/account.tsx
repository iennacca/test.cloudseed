import { useCallback, useEffect, useState } from 'react'
import useSWR from 'swr'

import { useSession } from '../services/Authentication'
import Layout from '../components/layouts/Layout'
import ErrorDetails from '../components/status/ErrorDetails'
import {
  Subscription,
} from '../services/SubscriptionService'
import {
  getAccountUrl,
  getAccountApiFetcherAsync
} from './api/views/account'
import { getSubscriptionsProducts } from './pricing'
import { tryUpdateUserInfo } from '../services/UserService'

const AccountPage = () => {
  const sessionResponse = useSession("/login")

  const [errors, setErrors] = useState<string[]>([])

  const shouldFetchData = sessionResponse.session
  const { data, error } = useSWR(
    () => shouldFetchData
      ? getAccountUrl
      : null,
    () => {
      return shouldFetchData
      ? getAccountApiFetcherAsync({
          accessToken: sessionResponse!.session!.accessToken
        })
      : null
    }
  )

  const [email, setEmail] = useState<string>("")
  const [gitHubUsername, setGitHubUserName] = useState<string>("")
  const [orderManagementUrl, setOrderManagementUrl] = useState<string | null>(null)
  const [subscriptions, setSubscriptions] = useState<Subscription[]>([])

  useEffect(() => {
    // const dataError = data?.errorResponse
    const dataResponse = data?.responseBody

    if(error) {
      setErrors([error])
    }
    if(dataResponse?.userInfo) {
      setEmail(dataResponse.userInfo?.emailAddress ?? "")
      setGitHubUserName(dataResponse.userInfo?.gitHubUsername ?? "")
    }
    if(dataResponse?.subscriptions) {
      setSubscriptions(dataResponse.subscriptions)
    }
    if(dataResponse?.manageSubscriptionsUrl) {
      setOrderManagementUrl(dataResponse.manageSubscriptionsUrl)
    }
  }, [data, errors])

  const saveUserInfo = useCallback(() => {
    if(sessionResponse.session === null) {
      return
    }

    tryUpdateUserInfo(
      sessionResponse.session.accessToken,
      sessionResponse.session.userId,
      {
        emailAddress: email,
        gitHubUsername: gitHubUsername
      }
    ).then((data) => {
      if(data.errorResponse) {
        setErrors([data.errorResponse.title ?? 'Failed saving data'])
      }
    }).catch((error) => {
      setErrors([error ?? ' Failed saving data'])
    })
  }, [sessionResponse, email, gitHubUsername])
  
  const createFormMarkup = () => {
    return (
      <form className="space-y-8 divide-y max-w-7xl mx-auto py-16 px-8 sm:px-6 lg:py-24 lg:px-8">
        <div className="space-y-8 divide-y sm:space-y-5">
          <h1 className="text-2xl leading-6 font-medium text-gray-900">Account</h1>
          <div className="mt-6 grid grid-cols-1 gap-y-6 gap-x-4 sm:grid-cols-6">
            <div className="sm:col-span-4">
              <label htmlFor="email" className="block text-sm font-medium text-gray-700">
                Email
              </label>
              <div className="mt-1 flex rounded-md">
                <input
                  type="email"
                  name="email"
                  id="email"
                  className="flex-1 block w-full min-w-0 rounded-none rounded-r-md sm:text-sm border-black border p-1"
                  placeholder="you@example.com"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                />
              </div>
            </div>
            <div className="sm:col-span-6">
              <div className="sm:col-span-4">
                <label htmlFor="gitlab" className="block text-sm font-medium text-gray-700">
                  GitHub Username
                </label>
                <p className="sm:col-span-4 mt-1 text-sm text-gray-500 mb-2">
                  You will get access to the CloudSeed git repo via GitHub.
                </p>
                <div className="mt-1 flex rounded-md">
                  <input
                    name="gitlab"
                    id="gitlab"
                    className="flex-1 block w-full min-w-0 rounded-none rounded-r-md sm:text-sm border-black border p-1"
                    placeholder="your_user_name"
                    value={gitHubUsername}
                    onChange={(e) => setGitHubUserName(e.target.value)}
                  />
                </div>
              </div>
              <div className="flex flex-1 py-2">
                  <a
                    className="flex items-center justify-center px-5 py-2 border border-transparent text-base font-medium text-white bg-black hover:bg-gray-900"
                    href=''
                    onClick={(_) => saveUserInfo()}
                  >
                    Save
                  </a>
              </div>
            </div>
            <div className="sm:col-span-6">
              <h2 className="sm:col-span-4 text-xl font-medium text-gray-900">Active Subscriptions</h2>
              <div>
                  {
                    orderManagementUrl
                      ? <a href={orderManagementUrl} className="text-hamred">Manage Subscriptions</a>
                      : null
                  }
              </div>
              {
                  createSubscriptionsListMarkup(subscriptions)
              }
            </div>
          </div>
        </div>
      </form>
    )
  }

  const createSubscriptionsListMarkup = (subscriptions: Subscription[]) => {
    if(subscriptions.length == 0) {
      return (
        <div>
          No active subscriptions. Subscribe from the Pricing page.
        </div>
      )
    }

    let allSubscriptionProducts = getSubscriptionsProducts()

    return (
      <div>
        <ul className="divide-y divide-gray-200">
          {
            subscriptions.map(s => {
              const product = allSubscriptionProducts
                .find(element => element.id === s.productId)
              const date = new Date(s.expirationTimestamp)
              const humanReadableDate = date.getFullYear() + '.' + (date.getMonth() + 1) + '.' + date.getDate()
              return (
                <li key={s.productId} className="flex py-4 justify-between">
                  <div>
                    {
                      product
                        ? product.name
                        : s.productId
                    }
                  </div>
                  <div>
                    {
                      'Expires: ' + humanReadableDate
                    }
                  </div>
                </li>
              )
            })
          }
        </ul>
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
      {
        sessionResponse.session
          ? createFormMarkup()
          : <h1>You must be logged in.</h1>
      }
    </Layout>
  )
}

export default AccountPage