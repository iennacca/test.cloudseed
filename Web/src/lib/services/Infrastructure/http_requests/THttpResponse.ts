/*
    * A wrapper HttpResponse
*/
import type { ErrorHttpResponse } from "$lib/services/Infrastructure/http_requests/ErrorHttpResponse";

export interface THttpResponse<T> {
    payload?: T,
    errorResponse?: ErrorHttpResponse
}