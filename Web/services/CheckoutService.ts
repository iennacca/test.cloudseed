import TryOptionResult from "./result_types/TryOptionResult"
import { HttpPostRequestAsync } from "./http_requests/HttpHandler"
import { getAppBaseUrl } from "./Configuration"
import { Session } from "../state/AppContext"

export type CheckoutProduct =
    {
        itemId: string,
        quantity: number
    }

type AppCreateCheckoutRequest = {
    products: CheckoutProduct[]
}

type AppCreateCheckoutResponse = {
    checkoutUrl: string
}

export async function tryCreateCheckoutAsync(
    session: Session,
    products: CheckoutProduct[]): Promise<TryOptionResult<AppCreateCheckoutResponse>> {

    if(session === null) {
        return TryOptionResult.createNone<AppCreateCheckoutResponse>(
            "User must be logged in!"
        )
    }

    const response = await HttpPostRequestAsync<AppCreateCheckoutRequest, AppCreateCheckoutResponse>(
        getAppBaseUrl() + '/checkout/create',
        {
            products: products
        },
        session.accessToken)

    if(response.errorResponse) {
        return TryOptionResult.createNone<AppCreateCheckoutResponse>(
            response.errorResponse.title ?? 'Unknown error'
        )
    }

    return TryOptionResult.createSome<AppCreateCheckoutResponse>(
        response.responseBody!
    )
}