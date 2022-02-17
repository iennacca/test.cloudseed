import type { AuthenticationSession } from '$lib/services/Infrastructure/authentication/AuthenticationSession'
import { getAppBaseUrl } from '$lib/services/Infrastructure/Configuration'

import type { CheckoutProduct } from '$lib/services/Application/Checkouts/Checkout'
import type { THttpResponse } from '$lib/services/Infrastructure/http_requests/THttpResponse'
import { HttpPostRequestAsync } from '$lib/services/Infrastructure/http_requests/HttpHandler'

export type CreateCheckoutRequest = {
    products: CheckoutProduct[],
    session: AuthenticationSession
}

export type CreateCheckoutResponse = {
    checkoutUrl: string
}

type CreateCheckoutAppRequest = {
    products: CheckoutProduct[]
}

export async function sendCreateCheckoutCommandAsync(
    session: AuthenticationSession,
    products: CheckoutProduct[]): Promise<THttpResponse<CreateCheckoutResponse>> {

    if(session === null) {
        return {
            errorResponse: {
                title: "User must be logged in!",
                status: 500
            }
        }
    }

    const response = await HttpPostRequestAsync<CreateCheckoutAppRequest, CreateCheckoutResponse>(
        getAppBaseUrl() + '/checkout/create',
        {
            products: products
        },
        session.accessToken)

    return response
}