const colors = require('tailwindcss/colors')

module.exports = {
  content: ['./src/**/*.{html,js,svelte,ts}'],
  darkMode: 'media', // or 'media' or 'class'
  theme: {
    colors: {
      black: '#000000',
      white: '#ffffff',
      hamred: '#be403b',
      gray: colors.gray,
      red: colors.red,
      yellow: colors.amber,
      green: colors.emerald
    },
  },
  variants: {
    extend: {},
  },
  plugins: [
    require('@tailwindcss/typography')
  ],

}
