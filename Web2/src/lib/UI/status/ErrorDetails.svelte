<script context="module" lang="ts">
    export type ErrorDetailsProps = {
        title: string,
        errorItems: string[]
    }
</script>

<script lang="ts">
    import StatusDetails from './StatusDetails.svelte'

    export let props: ErrorDetailsProps
</script>

<StatusDetails
    statusDetailsProps={{
        title: props.title,
        items: props.errorItems,
        itemClasses: [],
        backgroundClasses: ['bg-red-50']
    }} />
