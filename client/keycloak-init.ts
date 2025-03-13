import { KeycloakService } from "keycloak-angular";

export function initializeKeycloak(keycloak: KeycloakService) {
    return () =>
      keycloak.init({
        config: {
          url: 'http://localhost:8082',
          realm: 'CoinHawkRealm',
          clientId: 'coin-hawk-app',
          
        },
        initOptions: {
          onLoad: 'login-required',
          checkLoginIframe: false,
          redirectUri: 'http://localhost:4200/dashboard'
        },
      })
      
      // .then(() => {
      //   const el = document.getElementById('app-wrapper');
      //   if (el) {
      //     el.style.display = 'block';
      //   }
      // });
  }
  