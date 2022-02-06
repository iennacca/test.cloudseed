import Link from "next/link"

const customerSupportLink = "mailto:hamy+cloudseed@hamy.xyz"

const faqs = [
  {
    question: 'Do I get access to the full codebase?',
    answer: <>
      <p>
        Yes, once you subscribe to CloudSeed you will be added to the private GitHub repository and get access to the full source code!
      </p>
    </>,
  },
  {
    question: 'Can I change the code?',
    answer: <>
      <p>
        Yes, CloudSeed was built to provide a solid starting point for your app and to be minimally invasive so you can easily add / modify / delete code, libraries, and technologies as needed.
      </p>
      <br />
      <p>
        Want to use a different frontend library (Angular / Vue / vanilla), testing library (NUnit), database (Mongo / Postgres), etc? Just add it! 
      </p>
    </>,
  },
  {
    question: 'What license do I use CloudSeed under?',
    answer: <>
       Read the {'   '}
       <Link href="/terms">
          <a className="text-hamred">
          CloudSeed Terms of Service
          </a>
        </Link>
    </>,
  },
  {
    question: 'How many projects can I use CloudSeed for?',
    answer: 'Each license allows you to build one application with CloudSeed. We also have an unlimited license which allows you to build unlimited applications.',
  },
  {
    question: 'How long can I use CloudSeed for?',
    answer: 'You can use CloudSeed for the entire lifetime of your app - even if your subscription expires. However you must have an active subscription to get the latest code.',
  },
  {
    question: 'Do I get access to updates?',
    answer: <>
        <p>
        We continually update CloudSeed with code improvements and package updates so it's always the simple, powerful, and modern app foundation you've come to expect.
        </p>
        <br />
        <p>
          Note: Applying updates to an existing application may be tricky depending on how much you've changed the original CloudSeed code.
        </p>
      </>,
  },
  {
    question: 'Can I get support?',
    answer: <>
        <p>
          We offer support via email for questions around CloudSeed, implementation, and integration.
        </p>
      </>,
  },
  {
    question: 'What is your return policy?',
    answer: <>
        <p>
          We do not offer refunds as there's no way to 'return' a digital product once downloaded.
        </p>
        <br />
        <p>
          Contact us with any questions you have before purchasing.
        </p>
      </>,
  },
]

export default function FaqList() {
  return (
    <div className="bg-white">
      <div className="max-w-7xl mx-auto py-16 px-4 sm:px-6 lg:py-20 lg:px-8">
        <div>
          <div className="mb-8">
            <h2 className="text-4xl font-extrabold text-gray-900 text-center">Frequently Asked Questions</h2>
            <p className="mt-4 text-lg text-gray-500 text-center">
              Can’t find the answer you’re looking for? {' '}
              <a href={customerSupportLink} className="font-medium text-indigo-600 hover:text-indigo-500">
                Contact us
              </a>
              .
            </p>
          </div>
          <div className="mt-12 lg:mt-0 lg:col-span-2">
            <dl className="space-y-12">
              {faqs.map((faq) => (
                <div key={faq.question}>
                  <dt className="text-lg leading-6 font-medium text-gray-900">{faq.question}</dt>
                  <dd className="mt-2 text-base text-gray-500">{faq.answer}</dd>
                </div>
              ))}
            </dl>
          </div>
        </div>
      </div>
    </div>
  )
}