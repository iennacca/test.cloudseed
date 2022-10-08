<script lang="ts">
    import { onMount } from "svelte/internal";
	import { sendButtonPushesCommandAsync } from "./Commands/SendButtonPushesCommand";
    import { getRemotePushesQueryAsync } from "./Queries/FetchRemotePushesQuery"

    let remoteCount = 0
    let localCount = 0
    let unsavedLocalCount = 0
    $: totalCount = remoteCount + localCount

    let buttonText = "smash"

    let getNextButtonText = () => {
        if(Math.random() < 0.95) {
            return "smash"
        }

        let allTexts = [
            "activate",
            "attack",
            "bash",
            "boop",
            "bop",
            "click",
            "chop",
            "engage",
            "flick",
            "hit",
            "kick",
            "pinch",
            "ping",
            "poke",
            "pull",
            "push",
            "roundhouse",
            "smoosh",
            "snap",
            "spin",
            "tap",
            "touch",
            "trigger",
            "twist",
            "whack",
            "whallop",
            "wreck"
        ]
        let chosenIndex = Math.floor(Math.random() * allTexts.length)
        return allTexts[chosenIndex]
    }
    let updateButtonText = () => {
        let newButtonText = getNextButtonText()
        buttonText = newButtonText
    }

    let fetchRemotePushes = () => {
        getRemotePushesQueryAsync()
        .then(totalRemotePushes => {
            remoteCount = totalRemotePushes.payload ?? 0
        })
    }
    onMount(async () => fetchRemotePushes())

    let saveButtonPushes = () => {
        if(unsavedLocalCount === 0) {
            return
        }

        sendButtonPushesCommandAsync({
            hits: unsavedLocalCount
        })
        unsavedLocalCount = 0
    }

    let saveButtonPushesInterval = setInterval(
        saveButtonPushes,
        2000
    )
    let fetchTotalPushesInterval = setInterval(
        fetchRemotePushes,
        5000
    )
</script>

<div class="push-the-button text-center pt-6 pb-16">
    <div class="my-5">
        <div class="text-4xl py-4 font-bold">
            {totalCount}
        </div>
        <div class="text-lg pb-4">
            Total 
        </div>
    </div>
    <button
        on:click={() => {
            localCount += 1 
            unsavedLocalCount += 1
            updateButtonText()}}
        type="button"
        class="text-white bg-black py-2 px-4 uppercase font-bold"
        style="touch-action:manipulation;">
        {buttonText}
    </button>
    <div class="mt-5 max-w-md mx-auto sm:flex sm:justify-center md:mt-8" />
</div>