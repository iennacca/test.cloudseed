import React, { FunctionComponent } from 'react'

import Layout from '../components/layouts/Layout'

const IndexPage: FunctionComponent = () => (
  <Layout title="CloudSeed">
    <HeroSection />
  </Layout>
)

const HeroSection: FunctionComponent = () => (
  <div className="relative overflow-hidden">
      <div className="relative pt-6 pb-16 sm:pb-24">
        <main className="mt-16 mx-auto max-w-7xl px-4 sm:mt-24">
          <div className="text-center">
            <h1 className="text-4xl tracking-tight font-extrabold sm:text-5xl md:text-6xl">
              <span className="block xl:inline">Build your App with CloudSeed</span>{'      '}
            </h1>
            <p className="mt-3 max-w-md mx-auto text-base text-gray-500 sm:text-lg md:mt-5 md:text-xl md:max-w-3xl">
              A simple, powerful project base for your business.
            </p>
          </div>
        </main>
      </div>
    </div>
)

export default IndexPage
