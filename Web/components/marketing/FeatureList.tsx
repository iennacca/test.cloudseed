import { CheckIcon } from '@heroicons/react/outline'

const features: {
  name: string,
  description: string,
  icon: (props: React.ComponentProps<'svg'>) => JSX.Element
}[] = [
  {
    name: 'Users + Authentication',
    description: 'Registration + Login pages, email-based authentication, authentication',
    icon: CheckIcon
  },
  {
    name: 'Payments + Subscriptions',
    description: 'Checkout, order, and invoice integrations with Stripe. Provision and manage subscriptions.',
    icon: CheckIcon
  },
  {
    name: 'Scalable Architecture',
    description: 'Multi-tier architecture leveraging industry-leading technologies to scale to your first 100k+ users and beyond.',
    icon: CheckIcon
  },
  {
    name: 'Data Integration + Upkeep',
    description: 'Fully integrated datamodel using EntityFramework Core and Postgres. Built-in database upgrade mechanisms to keep your data up-to-date.',
    icon: CheckIcon
  },
  {
    name: 'Run-Anywhere',
    description: 'Fully containerized via Docker so you can develop and deploy anywhere.',
    icon: CheckIcon
  },
  {
    name: 'Customizable for your Business',
    description: 'Flexible configuration and decoupled architecture allows you to customize and grow your system along with your business.',
    icon: CheckIcon
  },
  {
    name: 'Build with Confidence',
    description: 'Comprehensive testing environment for unit and integration tests against a live DB.',
    icon: CheckIcon
  },
]

export default function FeatureList() {
  return (
    <div className="bg-white" id="feature_list">
      <div className="max-w-7xl mx-auto py-16 px-4 sm:px-6 lg:py-24 lg:px-8 lg:grid lg:grid-cols-3 lg:gap-x-8">
        <div>
          <h2 className="text-base font-semibold text-indigo-600 uppercase tracking-wide">Batteries included</h2>
          <p className="mt-2 text-3xl font-extrabold text-gray-900">Launch in minutes</p>
          <p className="mt-4 text-lg text-gray-500">
            Start launching business features now. Don't spend cycles rerolling authentication, payments, integrations etc.
          </p>
        </div>
        <div className="mt-12 lg:mt-0 lg:col-span-2">
          <dl className="space-y-10 sm:space-y-0 sm:grid sm:grid-cols-2 sm:grid-rows-4 sm:grid-flow-col sm:gap-x-6 sm:gap-y-10 lg:gap-x-8">
            {features.map((feature) => (
              <div key={feature.name} className="relative">
                <dt>
                    {
                        <feature.icon 
                            className="absolute h-6 w-6 text-green-500" aria-hidden="true" />
                    }
                  <p className="ml-9 text-lg leading-6 font-medium text-gray-900">{feature.name}</p>
                </dt>
                <dd className="mt-2 ml-9 text-base text-gray-500">{feature.description}</dd>
              </div>
            ))}
          </dl>
        </div>
      </div>
    </div>
  )
}