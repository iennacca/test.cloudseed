import { sveltekit } from '@sveltejs/kit/vite';
 
/** @type {import('vite').UserConfig} */
const config = {
    plugins: [sveltekit()],
    server: {
        port: 8080,
        // watch: {
        //     usePolling: true
        // },
        // hmr: {
        //     // protocol: 'ws',
        //     // host: 'localhost',
        //     port: 5000
        // }
    },
    preview: {
        port: 8080
    }
};

export default config;