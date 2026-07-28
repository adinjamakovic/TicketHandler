export const environment = {
  production: true,
  apiUrl: 'https://api.myapp.com', // stvarna produkcijska ruta
  oidcAuthority: 'https://identity.myapp.com',
  oidcClientId: 'market.spa',
  oidcScope: 'openid profile email market.api offline_access',
  GApiKey: "", //Google Maps API key goes here
  SentryDsn: "",//Sentry DSN goes here
  sentryEnvironment: "production"
};
