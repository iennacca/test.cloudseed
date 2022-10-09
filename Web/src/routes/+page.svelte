<script context="module" lang="ts">
</script>

<script lang="ts">
	import PushTheButton from "$lib/PushTheButton/PushTheButton.svelte";
	import HeroSection from "$lib/UI/marketing/HeroSection.svelte";
</script>

<svelte:head>
	<title>CloudSeed Base</title>
</svelte:head>

<section>
	<HeroSection
		title="Welcome to CloudSeed!"
		subtitle="The simple project starterkit." />
	<PushTheButton />
</section>

<style>
</style>
