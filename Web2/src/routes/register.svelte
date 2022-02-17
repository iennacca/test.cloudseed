<script context="module">
	// hamytodo - do some preload stuff to see if session is here or not
</script>

<script lang="ts">
	import { registerUserCommandAsync } from '$lib/services/Application/Users/Commands/RegisteruserCommand'
	import ErrorDetails from '$lib/UI/status/ErrorDetails.svelte'

	let emailAddress: string = ''

	let form_isSubmitting: boolean = false
	let isRegistered: boolean = false
	let errors: string[] = []

	const registerUser = () => {
		form_isSubmitting = true

		registerUserCommandAsync(
			{
				email_address: emailAddress
			}
		).then(response => {
			if(response.errorResponse) {
				errors = [response.errorResponse.title ?? 'UnknownError']
			} else {
				errors = []
				isRegistered = true
			}
			form_isSubmitting = false
		}).catch(unexpectedError => {
			console.error(unexpectedError)
			errors = [unexpectedError]
			form_isSubmitting = false
    	})
	}
</script>

<svelte:head>
	<title>Register</title>
</svelte:head>

{#if isRegistered}
	You're registered!
{:else}
	{#if errors.length > 0}
		<ErrorDetails 
			props={{
				title: "An error occurred",
				errorItems: errors
			}}
			/>
	{/if}
	<div class="sm:mx-auto sm:w-full sm:max-w-md">
		<h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900">
		Register
		</h2>
		<p class="mt-2 text-center text-sm text-gray-600">
		Already have an account? {'     '}
			<a href="/login"
				class="font-medium">Login</a>
		</p>
	</div>
	<div class="mt-8 sm:mx-auto sm:w-full sm:max-w-md">
		<div class="bg-white py-8 px-4 border border-black sm:px-10">
		<div class="space-y-6">
			<div>
				<label for="email" class="block text-sm font-medium text-gray-700">
					Email address
				</label>
				<div class="mt-1">
					<input
					id="email"
					name="email"
					type="email"
					autoComplete="email"
					required
					bind:value={emailAddress}
					class="appearance-none block w-full px-3 py-2 border border-black placeholder-gray-400 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
					/>
				</div>
				<!-- {
					errors.map( error => {
					<div class="block text-sm font-medium text-red-700">
						{ error }
					</div>
					})
				} -->
				</div>

				<div>
				<button
					type="submit"
					class="w-full flex justify-center py-2 px-4 border border-black text-sm font-medium bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
					on:click|preventDefault={
						form_isSubmitting
							? () => null
							: registerUser
					}
					>
					Register
				</button>
				</div>
			</div>
		</div>
	</div>
{/if}