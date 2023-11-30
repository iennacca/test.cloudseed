/** @type {import('tailwindcss').Config} */
module.exports = {
  // hamy: all css in .fs
  content: ["./Source/**/*.{fs,html,cshtml}"],
  theme: {
    extend: {},
  },
  plugins: [
    require('@tailwindcss/typography'),
    require("daisyui")
  ],
}

