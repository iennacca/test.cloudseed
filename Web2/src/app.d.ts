/// <reference types="@sveltejs/kit" />

// See https://kit.svelte.dev/docs/typescript
// for information about these interfaces\

import type { Session as AuthenticationSession } from '$lib/services/Infrastructure/authentication/AuthenticationSessionicationSession'

declare global {
	declare namespace App {
		interface Locals {
			session: AuthenticationSession
		}
	
		interface Platform {}
	
		interface Session {
			session: AuthenticationSession
		}
	
		interface Stuff {}
	}
}
