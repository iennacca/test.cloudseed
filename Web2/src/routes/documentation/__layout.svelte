<script context="module">
	export const prerender = true
</script>

<script lang="ts">
	import '../../app.css';

	type DocumentationLinkSection = {
	name: string,
	links: DocumentationLink[]
	}

	type DocumentationLink = {
	name: string,
	url: string
	}

	const documentationLinkSections: DocumentationLinkSection[] = [
	{
		name: "Get Started",
		links: [
		{
			name: "Quickstart",
			url: "/documentation"
		}
		]
	}
	]
</script>

<main class="prose p-6">
	<div class="flex">
        <div class="shrink p-6">
          <h2 class="text-2xl">Documentation</h2>
          <ul>
			{#each documentationLinkSections as section}
				<h3 class="text-lg mt-4">{section.name}</h3>
				{#each section.links as link}
					<a href={link.url}
						class="hover:text-hamred">
						<li class="mt-1">{link.name}</li>
					</a>
				{/each}
			{/each}
          </ul>
        </div>
        <div class="grow p-6">
          <div class="prose prose-md">
            <slot />
          </div>
        </div>
      </div>
</main>
