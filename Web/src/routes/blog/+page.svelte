<script context="module" lang="ts">
</script>

<script lang="ts">
	import type { BlogPostListItem } from "./BlogTypes"

	export let data : {
		allPosts: Array<BlogPostListItem>
	}
</script>

<section>
	<h1 class="text-2xl font-semibold">Blog</h1>
	<ul>
		{#each data.allPosts as post}
			<li class="list-disc">
				<a href={post.path}
					class="text-lg font-semibold">
					{post.title}
				</a>
				| {post.date}
			</li>
		{/each}
	</ul>
</section>

<style>
</style>
