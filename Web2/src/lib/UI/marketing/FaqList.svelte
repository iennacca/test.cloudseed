<script context="module">
	export const prerender = true;
</script>

<script lang="ts">
    const customerSupportLink = "mailto:hamy+cloudseed@hamy.xyz"

    const faqs = [
        {
    question: 'Can I change the code?',
    answer: `
      <p>
        Yep, CloudSeed was built to provide a solid starting point for your app and to be minimally invasive so you can easily add / modify / delete code, libraries, and technologies as needed.
      </p>
      <br />
      <p>
        This is your app - build it how you want.
      </p>
    `,
  },
  {
    question: 'Where can I get help?',
    answer: 'The latest documentation will be available on https://cloudseed.xyz',
  },

    ]
</script>

<div class="bg-white">
    <div class="max-w-7xl mx-auto py-16 px-4 sm:px-6 lg:py-20 lg:px-8">
      <div>
        <div class="mb-8">
          <h2 class="text-4xl font-extrabold text-gray-900 text-center">Frequently Asked Questions</h2>
          <p class="mt-4 text-lg text-gray-500 text-center">
            Can’t find the answer you’re looking for? {' '}
            <a href={customerSupportLink} class="font-medium text-indigo-600 hover:text-indigo-500">
              Contact us
            </a>
            .
          </p>
        </div>
        <div class="mt-12 lg:mt-0 lg:col-span-2">
          <dl class="space-y-12">
            {#each faqs as faq}
              <div>
                <dt class="text-lg leading-6 font-medium text-gray-900">{faq.question}</dt>
                <dd class="mt-2 text-base text-gray-500">{@html faq.answer}</dd>
              </div>
            {/each}
          </dl>
        </div>
      </div>
    </div>
  </div>