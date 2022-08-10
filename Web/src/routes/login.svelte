<script context="module">
    export const load = async({ session }) => {

        return {
            props: {
                session: session
            }
        }
    }
</script>

<script lang="ts">
	import { loginUserCommandAsync } from './api/authentication/login';
	import ErrorDetails from '$lib/UI/status/ErrorDetails.svelte'
    import SuccessStatusDetails from '$lib/UI/status/SuccessStatusDetails.svelte'

    export let session: App.Session

	let emailAddress: string = ''

	let form_isSubmitting: boolean = false
	let isLoggedIn: boolean = session.session !== null
    let isLogInRequestSuccessful: boolean = false
	let errors: string[] = []

	const loginUser = () => {
		form_isSubmitting = true

		loginUserCommandAsync(
			{
				emailAddress: emailAddress
			}
		).then(response => {
			if(response.errorResponse) {
				errors = [response.errorResponse.title ?? 'UnknownError']
			} else {
				errors = []
                isLogInRequestSuccessful = true
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
	<title>Login</title>
</svelte:head>

{#if isLoggedIn}
	You're already logged in!
{:else}
	{#if errors.length > 0}
		<ErrorDetails 
			props={{
				title: "An error occurred",
				errorItems: errors
			}}
			/>
	{/if}
    {#if isLogInRequestSuccessful}
        <SuccessStatusDetails 
            props={{
                title: 'Login Request Successful',
                successItems: [
                    "We've sent an email with a link for you to login",
                    "If you don't see it, please check your spam folder."
                    ]
            }} />
    {:else}
        <div class="sm:mx-auto sm:w-full sm:max-w-md">
            <h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900">
                Login
            </h2>
            <p class="mt-2 text-center text-sm text-gray-600">
                We'll send an email with a link to access your profile. No passwords required.
                <br />
                No account? {'     '}
                <a href="/register"
                    class="font-medium">Register</a>
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
                    </div>

                    <div>
                        <button
                            type="submit"
                            class="w-full flex justify-center py-2 px-4 border border-black text-sm font-medium bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                            on:click|preventDefault={
                                form_isSubmitting
                                    ? () => null
                                    : loginUser
                            }
                            >
                            Login
                        </button>
                    </div>
                </div>
            </div>
        </div>
    {/if}
{/if}

<!-- import Link from 'next/link'
import React, { FunctionComponent, useCallback, useState } from 'react'

import { useSession } from '../../services/Authentication'
import ErrorDetails from '../../components/status/ErrorDetails'
import Layout from '../../components/layouts/Layout'
import SuccessStatusDetails from '../../components/status/SuccessStatusDetails'
import { loginUserCommandAsync } from '../api/authentication/commands/login-user'

const RegisterPage: FunctionComponent = () => {
  const session = useSession()
  const [errors, setErrors] = useState<string[]>([])
  const [wasLoginAttempted, setWasLoginAttempted] = useState<boolean>(false)

  // @ts-ignore
  const loginUser = useCallback ((event) => {
    let localErrors: string[] = []

    event.preventDefault()

    const emailAddress: string = event.target.email.value;

    if(emailAddress == null 
      || emailAddress.length == 0
      || emailAddress.indexOf('@') < 0) {
      localErrors.push("Email address is malformed")
    }

    if(localErrors.length == 0) {
      loginUserCommandAsync(
        {
          emailAddress: emailAddress
        }
      ).then(response => {
        if(response.errorResponse) {
          localErrors.push(
              response.errorResponse.title ?? "unknown error"
          )
        }
        setWasLoginAttempted(true);
      })
      .catch(unexpectedError => {
        console.error(unexpectedError)
        localErrors.push(
            unexpectedError
        )
      })
    }
    setErrors(localErrors)
  }, [setErrors, setWasLoginAttempted])

  const getLoginFormMarkup = () => {
    return (
      <div className="mt-8 sm:mx-auto sm:w-full sm:max-w-md">
        <div className="bg-white py-8 px-4 shadow sm:rounded-lg sm:px-10">
          <form className="space-y-6" onSubmit={loginUser}>
            <div>
              <label htmlFor="email" className="block text-sm font-medium text-gray-700">
                Email address
              </label>
              <div className="mt-1">
                <input
                  id="email"
                  name="email"
                  type="email"
                  autoComplete="email"
                  required
                  className="appearance-none block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm placeholder-gray-400 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                />
              </div>
            </div>

            <div>
              <button
                type="submit"
                className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
              >
                Login
              </button>
            </div>
          </form>
        </div>
      </div>
    )
  }

  if(session.session != null) {
    return(
      <Layout title="CloudSeed">
        It looks like you're already logged in!
      </Layout>
    )
  }

  return (
    <Layout title="CloudSeed">
      {
        errors.length === 0
          ? <></>
          : <ErrorDetails
              title="An error occurred"
              errorItems={errors}
            />
      }
      <div className="min-h-full flex flex-col justify-center py-12 sm:px-6 lg:px-8">
          <div className="sm:mx-auto sm:w-full sm:max-w-md">
            <h2 className="mt-6 text-center text-3xl font-extrabold text-gray-900">
              Login
            </h2>
            <p className="mt-2 text-center text-sm text-gray-600">
              We'll send an email with a link to access your profile. No passwords required.
            </p>
            <p className="mt-2 text-center text-sm text-gray-600">
              No account? {'     '}
              <Link href="/register">
                <a className="font-medium">Register</a>
              </Link>
            </p>
          </div>

          {
            wasLoginAttempted && errors.length === 0
              ? <LoginSuccessMessage />
              : getLoginFormMarkup()
          }
        </div>
    </Layout>
  )
}

const LoginSuccessMessage = () => (
  <SuccessStatusDetails 
    title='Login Request Successful'
    successItems={[
      "We've sent an email with a link for you to login",
      "If you don't see it, please check your spam folder."
    ]} />
)

export default RegisterPage -->