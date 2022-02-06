const colors = require('tailwindcss/colors')

module.exports = {
  purge: [
    './components/**/*.tsx',
    './pages/**/*.tsx'
  ],
  darkMode: false, // or 'media' or 'class'
  theme: {
    colors: {
      black: '#000000',
      white: '#ffffff',
      hamred: '#be403b',
      gray: colors.coolGray,
      red: colors.red,
      yellow: colors.amber,
      green: colors.emerald
    },
    typography: {
      default: {
        css: {
          color: '#000000',
        },
      },
      sm: {
        css: {
          fontSize: '1rem'
        }
      }
    },
  },
  variants: {
    extend: {},
  },
  plugins: [],
}
