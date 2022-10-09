<script context="module" lang="ts">
</script>

<script lang="ts">
	import Post from "./Post.svelte"
	import type { PostData } from "./PostData"

	export let data : {
		postData: PostData
	}

	// console.log("Page Post Data: ", data)
</script>

<section>
	<Post 
		title={data.postData.title}
		date={data.postData.date}
		content={data.postData.content}>
	</Post>
</section>

<style>
</style>
