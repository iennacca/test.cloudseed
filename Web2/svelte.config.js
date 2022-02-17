// import adapter from '@sveltejs/adapter-auto';
import adapter from '@sveltejs/adapter-node';
import preprocess from 'svelte-preprocess';
import { mdsvex } from 'mdsvex'

/** @type {import('@sveltejs/kit').Config} */
const config = {
	extensions: ['.svelte', '.md', '.svx'],
	// Consult https://github.com/sveltejs/svelte-preprocess
	// for more information about preprocessors
	preprocess: [
		mdsvex({
			extensions: ['.md', '.svx']
		}),
		preprocess()
	],

	kit: {
		adapter: adapter(),

		// Override http methods in the Todo forms
		methodOverride: {
			allowed: ['PATCH', 'DELETE']
		},
		vite: {
			server: {
				watch: {
					usePolling: true
				},
				hmr: {
					// protocol: 'ws',
					// host: 'localhost',
					port: 5000
				}
			}
		}
	}
};

export default config;
