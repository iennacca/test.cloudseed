/*
    * The error response returned from the server
*/

export type ErrorHttpResponse = {
    title?: string,
    status?: number
}

export const createErrorHttpResponse = (
    title: string,
    status: number
) => {
    return {
        title: title,
        status: status
    }
}