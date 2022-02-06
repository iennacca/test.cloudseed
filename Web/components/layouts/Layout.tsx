import React, { ReactNode } from 'react'
import Head from 'next/head'

import Navigation, { NavigationLink } from './Navigation';
import { getDeployEnvironment } from '../../services/Configuration';
import { DeployEnvironments } from '../../services/environment/DeployEnvironments';
import { useSession } from '../../services/Authentication'

type Props = {
  children?: ReactNode
  title?: string
}

const Layout = ({ children, title = 'CloudSeed' }: Props) => {
  const deployEnvironment = getDeployEnvironment()
  const sessionResponse = useSession()

  const getAllNavigationLinks = (
    userId: string | null,
    deployEnvironment: DeployEnvironments
  ): NavigationLink[] => {
    if(deployEnvironment !== DeployEnvironments.Development) {
      return []
    }

    let navigationLinks: NavigationLink[] = [
      {
        name: 'Pricing',
        href: '/pricing'
      }
    ]

    if(userId !== null) {
      navigationLinks.push(
        {
          name: "Account",
          href: "/account"
        }
      )
    } else {
      navigationLinks.push(
        {
          name: "Register",
          href: "/register"
        }
      )
    }

    return navigationLinks
  }
  
  return (
    <div>
      <Head>
        <title>{title}</title>
        <meta charSet="utf-8" />
        <meta name="viewport" content="initial-scale=1.0, width=device-width" />
      </Head>
      <header>
        <Navigation 
          homeLink={
            {
              name: "CloudSeed",
              href: '/'
            }
          }
          navigationLinks={ 
            getAllNavigationLinks(
              sessionResponse.session?.userId ?? null,
              deployEnvironment
            )
          } />
      </header>
      <div >
        {children}
      </div>
      <div className="py-2 text-center">
        <a href="https://cloudseed.xyz"
          className="text-hamred">
          Built with CloudSeed
        </a>
      </div>
    </div>
  )
}

export default Layout
