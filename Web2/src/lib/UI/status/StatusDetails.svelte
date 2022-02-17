<script context="module" lang="ts">
    export type StatusDetailsProps = {
        title: string,
        items: string[],
        itemClasses: string[],
        backgroundClasses: string[]
    }
</script>

<script lang="ts">
    export let statusDetailsProps: StatusDetailsProps
</script>

<div class={`rounded-md p-4 ${statusDetailsProps.backgroundClasses.join(' ')}`}>
    <div class="ml-3">
    <h3 class={`text-sm font-medium`}>{statusDetailsProps.title}</h3>
    <div class={`mt-2 text-sm ${statusDetailsProps.itemClasses.join(' ')}`}>
        <ul class="list-disc pl-5 space-y-1">   
            {#each statusDetailsProps.items as item}
                <li >{item}</li>
            {/each}
        </ul>
    </div>
    </div>
</div>




<!-- import React from 'react'

interface IStatusDetailsProps {
    title: string,
    items: string[],
    itemclasss: string[],
    backgroundclasss: string[]
}

const StatusDetails = (props: IStatusDetailsProps) => {
    const getItemTsx = (items: string[], itemclasss: string[]) => {
        return items.length === 0
            ? <></>
            : <div class={`mt-2 text-sm ${itemclasss.join(' ')}`}>
                <ul role="list" class="list-disc pl-5 space-y-1">   
                    { 
                        items.map( (item, index) => {
                            return <li key={index}>{item}</li>
                        })
                    }
                </ul>
            </div>
    }

    return (
        <div class={`rounded-md p-4 ${props.backgroundclasss.join(' ')}`}>
            <div class="ml-3">
            <h3 class={`text-sm font-medium`}>{props.title}</h3>
            {
                getItemTsx(props.items, props.itemclasss)
            }
            </div>
        </div>
    )
}

export default StatusDetails -->