import Link from 'next/link'
import React, { FunctionComponent, useState } from 'react'

import ErrorDetails from '../components/status/ErrorDetails'
import Layout from '../components/layouts/Layout'
import { tryRegisterUser } from '../services/UserService'

const RegisterPage: FunctionComponent = () => {
  const [errors, setErrors] = useState<string[]>([])
  const [isRegistered, setIsRegistered] = useState<boolean>(false)

  // @ts-ignore
  const registerUser = event => {
    setErrors([])
    let localErrors = []

    event.preventDefault()

    const emailAddress: string = event.target.email.value;

    if(emailAddress == null 
      || emailAddress.length == 0
      || emailAddress.indexOf('@') < 0) {
      localErrors.push("Email address is malformed")
    }

    if(localErrors.length > 0) {
      setErrors(localErrors)
      return
    }

    tryRegisterUser(
      {
        email_address: emailAddress
      }
    ).then(response => {
      if(response.errorResponse) {
        setErrors(
          [
            response.errorResponse.title ?? "unknown error"
          ]
        )
        return;
      }
      setIsRegistered(true);
    })
    .catch(unexpectedError => {
      console.error(unexpectedError)
      setErrors(
        [
          unexpectedError
        ]
      )
    })
  }

  const getRegistrationTsx = () => (
    <>
      <div className="sm:mx-auto sm:w-full sm:max-w-md">
        <h2 className="mt-6 text-center text-3xl font-extrabold text-gray-900">
          Register
        </h2>
        <p className="mt-2 text-center text-sm text-gray-600">
          Already have an account? {'     '}
          <Link href="/login">
            <a className="font-medium">Login</a>
          </Link>
        </p>
      </div>
      <div className="mt-8 sm:mx-auto sm:w-full sm:max-w-md">
        <div className="bg-white py-8 px-4 shadow sm:rounded-lg sm:px-10">
          <form className="space-y-6" onSubmit={registerUser}>
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
              {
                errors.map( error => {
                  <div className="block text-sm font-medium text-red-700">
                    { error }
                  </div>
                })
              }
            </div>

            <div>
              <button
                type="submit"
                className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
              >
                Register
              </button>
            </div>
          </form>
        </div>
      </div>
    </>
  )

  const getRegistrationSucceededTsx = () => (
    <div className="sm:mx-auto sm:w-full sm:max-w-md">
        <h2 className="mt-6 text-center text-3xl font-extrabold text-gray-900">
          Registration Successful
        </h2>
        <p className="mt-2 text-center text-sm text-gray-600">
          <Link href="/login">
            <a className="font-medium">Login</a>
          </Link>
          {'     '} to your account to get started!
        </p>
      </div>
  )

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
        {
          isRegistered
            ? getRegistrationSucceededTsx()
            : getRegistrationTsx()
        }
      </div>
    </Layout>
  )
}

export default RegisterPage
