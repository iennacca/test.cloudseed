<script lang="ts">
    export let buttonText: string 
    export let onClick: () => void
</script>

<button
    on:click={onClick}
    type="button"
    class="text-white bg-black py-2 px-2">
    {buttonText}
</button>