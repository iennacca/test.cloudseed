<script context="module">
  export const load = async({ session }) => {

      return {
          props: {
              session: session
          }
      }
  }
</script>

<script lang="ts">
    import { goto } from '$app/navigation';
    import { getDeployEnvironment } from '$lib/services/Infrastructure/Configuration'
    import { DeployEnvironments } from '$lib/services/Infrastructure/environment/DeployEnvironments'
    import { subscribe } from 'svelte/internal'
    import { CheckIcon, MinusIcon } from '$lib/UI/ui-utils/icons'
    import { sendCreateCheckoutCommandAsync } from '$lib/services/Application/Checkouts/Commands/CreateCheckoutCommand'
    import type { AuthenticationSession } from '$lib/services/Infrastructure/authentication/AuthenticationSession'
    import FaqList from '$lib/UI/marketing/FaqList.svelte'
    import { getSubscriptionsProducts } from '$lib/services/Application/Products/AllProducts'

    export let session: App.Session
    let errors: string[] = []

    const onClickHandler = async (
      session: AuthenticationSession | null,
      productId: string) => {
        // hamytodo - figure out how to get session

      if(session === null) {
        goto('/register')
        return
      }

      const checkoutResponse = await sendCreateCheckoutCommandAsync(
        session,
        [
          {
            itemId: productId,
            quantity: 1
          }
        ]
      )

      if(checkoutResponse.errorResponse) {
        errors = [ checkoutResponse.errorResponse?.title ?? "an error occurred"]
        return
      }

      const checkoutUrl = checkoutResponse.responseBody!.checkoutUrl
      window.location.assign(checkoutUrl)
    }
</script>

<div class="py-12 bg-white" id="tech_stack">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="sm:flex sm:flex-col sm:align-center mb-6">
        <h1 class="text-5xl font-extrabold text-gray-900 sm:text-center">Pricing Plans</h1>
        <p class="mt-5 text-xl text-gray-500 sm:text-center">
          Simple pricing so you can launch today.
        </p>
      </div>
      <div class="relative">
        <div class="absolute inset-0 h-3/4" />
        <div class="relative z-10 max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div class="max-w-md mx-auto space-y-4 lg:max-w-5xl lg:grid lg:grid-cols-2 lg:gap-5 lg:space-y-0">
            {#each getSubscriptionsProducts() as subscription}
              <div class="flex flex-col border-2 border-black overflow-hidden divide-y-2 divide-black">
                <div class="px-6 py-8 bg-white sm:p-10 sm:pb-6">
                  <div>
                    <h3
                      class="inline-flex px-4 py-1 text-sm font-semibold tracking-wide uppercase bg-indigo-100 text-indigo-600"
                      id="tier-standard"
                    >
                      {subscription.name}
                    </h3>
                  </div>
                  <div class="mt-4 flex items-baseline text-6xl font-extrabold">
                    ${subscription.priceYearly}
                    <span class="ml-1 text-2xl font-medium text-gray-500">/year</span>
                  </div>
                  <p class="mt-5 text-lg text-gray-500">{subscription.description}</p>
                </div>
                <div class="flex-1 flex flex-col justify-between px-6 pt-6 pb-8 space-y-6 sm:p-10 sm:pt-6">
                  <ul role="list" class="space-y-4">
                    {#each subscription.features as feature}
                      <li class="flex items-start">
                        <div class="flex-shrink-0 mr-4">
                            {#if feature.included }
                                {@html CheckIcon}
                            {:else}
                                {@html MinusIcon}
                            {/if}
                        </div>
                        <p class="ml-3 text-base text-gray-800">{feature.name}</p>
                      </li>
                    {/each}
                  </ul>
                  <div class="">
                        <a
                        class="flex items-center justify-center px-5 py-3 border border-transparent text-base font-medium text-white bg-black hover:bg-gray-900"
                        aria-describedby="tier-standard"
                        href=''
                        on:click={() => onClickHandler(session.session, subscription.id)}
                        >
                        Get started
                        </a>
                    </div>
                </div>
              </div>
            {/each}
          </div>
        </div>
      </div>
      <FaqList />
    </div>
  </div>