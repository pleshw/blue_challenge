import { createApp } from 'vue'
import { createPinia } from 'pinia'
import PrimeVue from 'primevue/config'
import Aura from '@primeuix/themes/aura'
import { definePreset } from '@primeuix/themes'
import ConfirmationService from 'primevue/confirmationservice'
import ToastService from 'primevue/toastservice'
import Tooltip from 'primevue/tooltip'

import App from './App.vue'
import router from './router'

import 'primeicons/primeicons.css'

// Custom blue theme based on the provided color scheme
const BlueChallenge = definePreset(Aura, {
  semantic: {
    primary: {
      50: '#e6f0ff',
      100: '#b3d1ff',
      200: '#80b3ff',
      300: '#4d94ff',
      400: '#1a75ff',
      500: '#0066ff',
      600: '#0052cc',
      700: '#003d99',
      800: '#002966',
      900: '#001433',
      950: '#000a1a'
    },
    colorScheme: {
      light: {
        primary: {
          color: '#0066ff',
          inverseColor: '#ffffff',
          hoverColor: '#0052cc',
          activeColor: '#003d99'
        },
        highlight: {
          background: '#e6f0ff',
          focusBackground: '#b3d1ff',
          color: '#003d99',
          focusColor: '#002966'
        }
      },
      dark: {
        primary: {
          color: '#4d94ff',
          inverseColor: '#001433',
          hoverColor: '#1a75ff',
          activeColor: '#0066ff'
        },
        highlight: {
          background: 'rgba(77, 148, 255, 0.16)',
          focusBackground: 'rgba(77, 148, 255, 0.24)',
          color: 'rgba(255,255,255,.87)',
          focusColor: 'rgba(255,255,255,.87)'
        }
      }
    }
  }
})

const app = createApp(App)

app.use(createPinia())
app.use(router)
app.use(PrimeVue, {
  theme: {
    preset: BlueChallenge,
  },
})
app.use(ConfirmationService)
app.use(ToastService)
app.directive('tooltip', Tooltip)

app.mount('#app')
