
export type AccessTokenPayload = {
    sub: string, // userId
    iat: number, // creationTime secondsFromEpoch
    exp: number, // expirationTime secondsFromEpoch
}