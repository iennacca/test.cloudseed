<script context="module" lang="ts">
    export type SuccessDetailsProps = {
        title: string,
        successItems: string[]
    }
</script>

<script lang="ts">
    import StatusDetails from './StatusDetails.svelte'

    export let props: SuccessDetailsProps
</script>

<StatusDetails
    statusDetailsProps={{
        title: props.title,
        items: props.successItems,
        itemClasses: [],
        backgroundClasses: ['bg-green-50']
    }} />
