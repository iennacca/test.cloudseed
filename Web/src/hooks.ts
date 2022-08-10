import cookie from 'cookie';
import { v4 as uuid } from '@lukeed/uuid';
import type { Handle } from '@sveltejs/kit';
import { clearSessionCookiesOnResponse, getSessionFromCookiesAsync, setSessionCookiesOnResponse } from '$lib/services/Infrastructure/authentication/AuthenticationService';

export const handle: Handle = async ({ event, resolve }) => {
	const cookies = cookie.parse(event.request.headers.get('cookie') || '');
	let session = await getSessionFromCookiesAsync(cookies)

	console.log('hamy - session')
	console.log(session)

	event.locals.session = session

	const response = await resolve(event);

	// if (session) {
	// 	setSessionCookiesOnResponse(
	// 		response,
	// 		session
	// 	)
	// } else {
	// 	console.log('hamy - clear cookie')
	// 	clearSessionCookiesOnResponse(
	// 		response,
	// 	)
	// }

	return response;
};

export function getSession({locals}) {
	return {
		session: locals.session
	}
}
