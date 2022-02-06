/*
    * A wrapper HttpResponse
*/
import { ErrorHttpResponse } from "./ErrorHttpResponse";

export interface THttpResponse<T> {
    responseBody?: T,
    errorResponse?: ErrorHttpResponse
}