<script lang="ts">
	import Navigation from '$lib/UI/header/layouts/Navigation.svelte';
	import type { NavigationLink } from '$lib/UI/header/layouts/Navigation.svelte'
	import '../app.css';

	const getNavigationLinks = () => {
		let navigationLinks: NavigationLink[] = [
		{
			name: 'Get Started',
			href: 'https://cloudseed.xyz/documentation'
		}
		]

		return navigationLinks
	}
</script>

<Navigation 
	homeLink={
		{
			name: "CloudSeed Base",
			href: "/"
		}
	}
	allLinks={getNavigationLinks()} />

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
