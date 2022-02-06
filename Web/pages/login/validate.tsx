import { GetServerSideProps } from 'next'
import React, { FunctionComponent } from 'react'
import Layout from '../../components/layouts/Layout'
import ErrorDetails from '../../components/status/ErrorDetails'
import { constructUrlWithQueryParameters } from '../../services/http_requests/HttpHandler'
import { useAppContext } from '../../state/AppContext'
import { getWebBaseUrl } from '../../services/Configuration'

type ValidationProps = {
  errors: string[] | null
}


export const getServerSideProps: GetServerSideProps = async (context) => {


  const { token } = context.query 

  if(!token) {
    return {
      props: {
        errors: [
          'No token found! Parameter "token" is required!'
        ]
      }
    }
  }

  return {
    redirect: {
      permanent: false,
      destination: constructUrlWithQueryParameters(
        getWebBaseUrl() + '/api/session/create',
        {
          token: token
        }
      )
    }
  }

}

const LoginValidatePage: FunctionComponent<ValidationProps> = (props: ValidationProps) => {
  const appContext = useAppContext()

  const getErrorDetails = (errors: string[]) => {
    return <ErrorDetails title="Failed to Login"
      errorItems={ errors } />
  }

  return (
    <Layout title="CloudSeed">
      {
        (props.errors)
          ? getErrorDetails(props.errors)
          : <div className="min-h-full flex flex-col justify-center py-12 sm:px-6 lg:px-8">
              { 
                appContext.session
                  ? "Redirecting..."
                  : "Loading..."
              }
          </div>
      }
    </Layout>
  )
}

export default LoginValidatePage
