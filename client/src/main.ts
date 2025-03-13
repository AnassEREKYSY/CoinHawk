import { bootstrapApplication } from '@angular/platform-browser';
import { importProvidersFrom, APP_INITIALIZER } from '@angular/core';
import { AppComponent } from './app/app.component';
import { provideRouter } from '@angular/router';
import { routes } from './app/app.routes';
import { HttpClientModule } from '@angular/common/http';
import { KeycloakAngularModule, KeycloakService } from 'keycloak-angular';
import { initializeKeycloak } from '../keycloak-init';

bootstrapApplication(AppComponent, {
  providers: [
    importProvidersFrom(HttpClientModule, KeycloakAngularModule),
    provideRouter(routes),
    {
      provide: APP_INITIALIZER,
      useFactory: initializeKeycloak,
      deps: [KeycloakService],
      multi: true,
    },
  ],
}).catch(err => console.error(err));
