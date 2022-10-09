<script>
    export let title
    export let date
    export let content
</script>

<article class="p-6 prose">
    <h1>{title}</h1>

    <p>Date: {date}</p>

    <svelte:component this={content} />
    <!-- <slot /> -->
</article>