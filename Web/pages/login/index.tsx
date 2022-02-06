import Link from 'next/link'
import React, { FunctionComponent, useCallback, useState } from 'react'

import { useSession } from '../../services/Authentication'
import ErrorDetails from '../../components/status/ErrorDetails'
import Layout from '../../components/layouts/Layout'
import SuccessStatusDetails from '../../components/status/SuccessStatusDetails'
import { tryLoginUser } from '../../services/UserService'

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
      tryLoginUser(
        {
          email_address: emailAddress
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

export default RegisterPage
