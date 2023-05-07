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
  daisyui: {
    themes: [
      {
        hamylofi: {
          "color-scheme": "light",
          "primary": "#000000",
          "primary-content": "#ffffff",
          "secondary": "#4b5563",
          "secondary-content": "#ffffff",
          "accent": "#A71930",
          "accent-content": "#ffffff",
          "neutral": "#000000",
          "neutral-content": "#ffffff",
          "base-100": "#ffffff",
          "base-200": "#F2F2F2",
          "base-300": "#E6E5E5",
          "base-content": "#000000",
          "info": "#0070F3",
          "info-content": "#ffffff",
          "success": "#198754",
          "success-content": "#ffffff",
          "warning": "#FBBD23",
          "warning-content": "#000000",
          "error": "#CC0000",
          "error-content": "#ffffff",
          "--rounded-box": "0.25rem",
          "--rounded-btn": "0.125rem",
          "--rounded-badge": "0.125rem",
          "--animation-btn": "0",
          "--animation-input": "0",
          "--btn-focus-scale": "1",
          "--tab-radius": "0",
        }
      }
    ]
  },
  variants: {
    extend: {},
  },
  plugins: [
    require('@tailwindcss/typography'),
    require("daisyui")
  ],

}
