import React, { FunctionComponent } from 'react'

import { EmailListSignup } from '../components/marketing/EmailListSignup'
import FeatureList from '../components/marketing/FeatureList'
import Layout from '../components/layouts/Layout'
import TechStack from '../components/marketing/TechStack'

const IndexPage: FunctionComponent = () => (
  <Layout title="CloudSeed">
    <HeroSection />
    <FeatureList />
    <TechStack />
    <EmailListSignup />
  </Layout>
)

const HeroSection: FunctionComponent = () => (
  <div className="relative overflow-hidden">
      <div className="relative pt-6 pb-16 sm:pb-24">
        <main className="mt-16 mx-auto max-w-7xl px-4 sm:mt-24">
          <div className="text-center">
            <h1 className="text-4xl tracking-tight font-extrabold text-gray-900 sm:text-5xl md:text-6xl">
              <span className="block xl:inline">Launch your next business in minutes,  </span>{'      '}
              <span className="block text-indigo-600 xl:inline">not months</span>
            </h1>
            <p className="mt-3 max-w-md mx-auto text-base text-gray-500 sm:text-lg md:mt-5 md:text-xl md:max-w-3xl">
              CloudSeed is a simple, powerful project base that takes care of common business scaffolding so you can <strong>focus 
              on building your business</strong> - not reinventing the wheel.
            </p>
            <div className="mt-5 max-w-md mx-auto sm:flex sm:justify-center md:mt-8">
              <div className="rounded-md shadow">
                <a
                  href="#email_list_signup"
                  className="w-full flex items-center justify-center px-8 py-3 border border-transparent text-base font-medium rounded-md hover:bg-indigo-700 md:py-4 md:text-lg md:px-10"
                >
                  Request Access
                </a>
              </div>
              <div className="mt-3 rounded-md shadow sm:mt-0 sm:ml-3">
                <a
                  href="#feature_list"
                  className="w-full flex items-center justify-center px-8 py-3 border border-transparent text-base font-medium rounded-md text-indigo-600 bg-white hover:bg-gray-50 md:py-4 md:text-lg md:px-10"
                >
                  Features
                </a>
              </div>
            </div>
          </div>
        </main>
      </div>
    </div>
)

export default IndexPage
