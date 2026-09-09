import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { providePrimeNG } from 'primeng/config';
import Aura from '@primeuix/themes/aura';
import { definePreset } from '@primeuix/themes';
import { routes } from './app.routes';
import { authInterceptor } from './core/interceptors/auth.interceptor';

const ConstruControlPreset = definePreset(Aura, {
  semantic: {
    primary: {
      50: '{amber.50}',
      100: '{amber.100}',
      200: '{amber.200}',
      300: '{amber.300}',
      400: '{amber.400}',
      500: '{amber.500}',
      600: '{amber.600}',
      700: '{amber.700}',
      800: '{amber.800}',
      900: '{amber.900}',
      950: '{amber.950}'
    },
    colorScheme: {
      dark: {
        surface: {
          0: '#ffffff',
          50: '#F4F5F7',
          100: '#E5E7EB',
          200: '#374151',
          300: '#2A3140',
          400: '#232937',
          500: '#1A1F2E',
          600: '#161B26',
          700: '#12161F',
          800: '#0F1419',
          900: '#0B0E13',
          950: '#07090C'
        }
      }
    }
  }
});

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor])),
    providePrimeNG({
      theme: {
        preset: ConstruControlPreset,
        options: {
          darkModeSelector: '.app-dark',
          cssLayer: false
        }
      },
      ripple: true
    })
  ]
};
