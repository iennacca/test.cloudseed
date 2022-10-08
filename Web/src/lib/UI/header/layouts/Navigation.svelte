<script context="module" lang="ts">
    export type NavigationLink = {
        name: string,
        href: string
    }
</script>

<script lang="ts">
    export let homeLink: NavigationLink
    export let allLinks: NavigationLink[]
</script>

<header>
    <nav class="flex items-center justify-between flex-wrap p-6 text-black">
        <div class="flex flex-shrink-0 text-black mr-6">
            <a href={homeLink.href}
                class="font-semibold text-xl tracking-tight hover:text-hamred">
                { homeLink.name }
            </a>
        </div>
        <div class='flex items-right'>
            {#each allLinks as link}
                <a href={link.href}
                    class="block mt-4 lg:inline-block lg:mt-0 hover:text-hamred mr-6 lg:flex">
                    {link.name}
                </a>
            {/each}
        </div>
      </nav>
</header>

<style>
</style>
