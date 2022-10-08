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
	import Navigation from '$lib/UI/header/layouts/Navigation.svelte';
	import type { NavigationLink } from '$lib/UI/header/layouts/Navigation.svelte'
	import '../app.css';
	import type { AuthenticationSession } from '$lib/services/Infrastructure/authentication/AuthenticationSession'
	import { tryGetGoogleAnalyticsId } from '../lib/services/Infrastructure/Configuration'

	export let session: App.Session

	const getNavigationLinks = (session: AuthenticationSession | null) => {
		let navigationLinks: NavigationLink[] = [
		{
			name: 'About',
			href: 'https://labs.hamy.xyz/projects/smashthebutton'
		}
		]

		return navigationLinks
	}

	try {
		if(typeof window !== 'undefined' 
        && window) {
			const googleAnalyticsId = tryGetGoogleAnalyticsId()
			// @ts-ignore
			window.dataLayer = window.dataLayer || [];
			// @ts-ignore
			function gtag(){window.dataLayer.push(arguments);}
			// @ts-ignore
			gtag('js', new Date());
			// @ts-ignore
			gtag('config', googleAnalyticsId);
		}
	} catch(error) {
		console.error("Failed to initialize Analytics")
	}
</script>

<svelte:head>
	<script async src="https://www.googletagmanager.com/gtag/js"></script>
</svelte:head>

<Navigation 
	homeLink={
		{
			name: "SMASH_THE_BUTTON",
			href: "/"
		}
	}
	allLinks={getNavigationLinks(session.session)} />

<main>
	<slot />
</main>

<footer>
	<div class="py-2 text-center">
        <a href="https://cloudseed.xyz"
          class="text-hamred">
          Built with CloudSeed
        </a>
	</div>
</footer>

<style>
</style>
