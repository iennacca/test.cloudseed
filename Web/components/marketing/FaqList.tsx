const faqs = [
  {
    question: 'Can I change the code?',
    answer: <>
      <p>
        Yep, CloudSeed was built to provide a solid starting point for your app and to be minimally invasive so you can easily add / modify / delete code, libraries, and technologies as needed.
      </p>
      <br />
      <p>
        This is your app - build it how you want.
      </p>
    </>,
  },
  {
    question: 'Where can I get help?',
    answer: 'The latest documentation will be available on https://cloudseed.xyz',
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
              Put your FAQ here!
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