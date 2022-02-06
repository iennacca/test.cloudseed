import { NextApiRequest } from "next";

export interface TypedBodyNextApiRequest<T> extends NextApiRequest {
    body: T
}