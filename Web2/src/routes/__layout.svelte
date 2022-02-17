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

	export let session: App.Session

	const getNavigationLinks = (session: AuthenticationSession | null) => {
		let navigationLinks: NavigationLink[] = [
		{
			name: 'Documentation',
			href: '/documentation'
		},
		{
			name: 'Pricing',
			href: '/pricing'
		}
		]

		if(session !== null) {
			navigationLinks.push(
				{
				name: "Account",
				href: "/account"
				}
			)
		} else {
			navigationLinks.push(
				{
				name: "Login",
				href: "/login"
				}
			)
		}

		return navigationLinks
	}
</script>

<Navigation 
	homeLink={
		{
			name: "CloudSeed",
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
