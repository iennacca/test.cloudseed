<script context="module" lang="ts">
    // import { session } from '$app/stores';

    export const load = async({ session }) => {
        if(!session.session) {
            return {
              status: 302,
              redirect: '/login'
            }
        }

        const errors: string[] = []

        const getUserInfoQueryResponse = await getUserInfoQueryAsync(
            session.session.userId,
            session.session.accessToken
        )
        if(getUserInfoQueryResponse.errorResponse) {
          errors.push(getUserInfoQueryResponse.errorResponse.title)
        }

        const getUserSubscriptionsQueryResponse = await sendGetSubscriptionsForUserQueryAsync(
          session.session.userId,
          session.session.accessToken
        )
        if(getUserSubscriptionsQueryResponse.errorResponse) {
          errors.push(getUserSubscriptionsQueryResponse.errorResponse.title)
        }

        const getManageSubscriptionsForUserQueryResponse = await sendGetManageSubscriptionsUrlForUserQueryAsync(
          session.session.userId,
          session.session.accessToken
        )
        if(getManageSubscriptionsForUserQueryResponse.errorResponse) {
          errors.push(getManageSubscriptionsForUserQueryResponse.errorResponse.title)
        }

        console.log(getUserSubscriptionsQueryResponse)


        return {
            props: {
                session: session,
                manageSubscriptionsUrl: getManageSubscriptionsForUserQueryResponse.responseBody?.url ?? null,
                subscriptions: getUserSubscriptionsQueryResponse.responseBody ?? [],
                userInfo: getUserInfoQueryResponse.responseBody ?? null,
                errors: errors
            } as AccountPageProps
        }
    }

    type AccountPageProps = {
        session: App.Session,
        errors: string[],
        manageSubscriptionsUrl: string | null
        subscriptions: Subscription[],
        userInfo: GetUserInfoQueryResult | null
    }
</script>

<script lang="ts">
    import type { Subscription } from '$lib/services/Application/Subscriptions/Subscription'
    import type { GetUserInfoQueryResult } from '$lib/services/Application/Users/Queries/GetUserInfoQuery'
    import { getUserInfoQueryAsync } from '$lib/services/Application/Users/Queries/GetUserInfoQuery'
    import { sendSaveUserInfoCommandAsync } from '$lib/services/Application/Users/Commands/SaveUserInfoCommand'
    import ErrorDetails from '$lib/UI/status/ErrorDetails.svelte'
    import SuccessDetails from '$lib/UI/status/SuccessStatusDetails.svelte'
    import SuccessStatusDetails from '$lib/UI/status/SuccessStatusDetails.svelte';
    import { sendGetSubscriptionsForUserQueryAsync } from '$lib/services/Application/Subscriptions/GetSubscriptionsForUserQuery'
import { subscribe } from 'svelte/internal'
  import { getSubscriptionProductDetails } from '$lib/services/Application/Products/AllProducts'
  import { sendGetManageSubscriptionsUrlForUserQueryAsync } from '$lib/services/Application/Subscriptions/GetManageSubscriptionsUrlForUserQuery'

    // export let session: App.Session
    export let session: App.Session = null
    export let subscriptions: Subscription[] = []
    export let manageSubscriptionsUrl: string | null = null
    export let errors: string[] = []
    export let userInfo: GetUserInfoQueryResult | null = null

    const subscriptionsList: {subscriptionName: string, subscriptionExpirationString: string}[] = subscriptions.map(s => {
      const product = getSubscriptionProductDetails(s.productId)

      const date = new Date(s.expirationTimestamp)
      const humanReadableDate = date.getFullYear() + '.' + (date.getMonth() + 1) + '.' + date.getDate()

      return {
        subscriptionName: product.name,
        subscriptionExpirationString: humanReadableDate
      }
    })

    let successItems: string[] = []

    const saveUserInfo = () => {
        sendSaveUserInfoCommandAsync(
            session.session.accessToken,
            userInfo
        ).then(response => {
            if(response.errorResponse) {
                errors = [response.errorResponse.title ?? 'Failed to save userInfo']
            } else {
                successItems = ['Saved user successfully']
            }
        }).catch(error => {
            errors = [error]
        })
    }
</script>

{#if errors.length > 0}
    <ErrorDetails
        props={{
            title: "An error occurred",
            errorItems: errors
        }}
        />
{/if}
{#if successItems.length > 0}
    <SuccessStatusDetails
        props={{
            title: "Update succeeded",
            successItems: successItems
        }}
        />
{/if}
<form class="space-y-8 divide-y max-w-7xl mx-auto py-16 px-8 sm:px-6 lg:py-24 lg:px-8">
    <div class="space-y-8 divide-y sm:space-y-5">
      <h1 class="text-2xl leading-6 font-medium text-gray-900">Account</h1>
      <div class="mt-6 grid grid-cols-1 gap-y-6 gap-x-4 sm:grid-cols-6">
        <div class="sm:col-span-4">
          <label for="email" class="block text-sm font-medium text-gray-700">
            Email
          </label>
          <div class="mt-1 flex rounded-md">
            <input
              type="email"
              name="email"
              id="email"
              class="flex-1 block w-full min-w-0 rounded-none rounded-r-md sm:text-sm border-black border p-1"
              placeholder="you@example.com"
              bind:value={userInfo.emailAddress}
            />
          </div>
        </div>
        <div class="sm:col-span-6">
          <div class="sm:col-span-4">
            <label for="gitlab" class="block text-sm font-medium text-gray-700">
              GitHub Username
            </label>
            <p class="sm:col-span-4 mt-1 text-sm text-gray-500 mb-2">
              You will get access to the CloudSeed git repo via GitHub.
            </p>
            <div class="mt-1 flex rounded-md">
              <input
                name="gitlab"
                id="gitlab"
                class="flex-1 block w-full min-w-0 rounded-none rounded-r-md sm:text-sm border-black border p-1"
                placeholder="your_user_name"
                bind:value={userInfo.gitHubUsername}
              />
            </div>
          </div>
          <div class="flex flex-1 py-2">
              <a
                class="flex items-center justify-center px-5 py-2 border border-transparent text-base font-medium text-white bg-black hover:bg-gray-900"
                href=''
                onClick={(_) => saveUserInfo()}
              >
                Save
              </a>
          </div>
        </div>
        <div class="sm:col-span-6">
          <h2 class="sm:col-span-4 text-xl font-medium text-gray-900">Active Subscriptions</h2>
          <div>
            {#if manageSubscriptionsUrl}
              <a href={manageSubscriptionsUrl} class="text-hamred">
                Manage Subscriptions
              </a>
            {/if}
          </div>
          {#if subscriptionsList.length === 0}
            <div>
              No active subscriptions. Subscribe from the Pricing page.
            </div>
          {:else}
            <div>
              <ul class="divide-y divide-gray-200">
                {#each subscriptionsList as subscription}
                  <li class="flex py-4 justify-between">
                    <div>
                      { subscription.subscriptionName }
                    </div>
                    <div>
                      Expires: {subscription.subscriptionExpirationString}
                    </div>
                  </li>

                {/each}
              </ul>
            </div>
          {/if}
        </div>
      </div>
    </div>
  </form>