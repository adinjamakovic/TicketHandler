export const environment = {
  production: false,
  apiUrl: 'https://localhost:7260',
  oidcAuthority: 'https://localhost:5001',
  oidcClientId: 'market.spa',
  oidcScope: 'openid profile email market.api offline_access',
  GApiKey: "", //Google Maps API key goes here
  SentryDsn: "", //Sentry DSN goes here
  sentryEnvironment: "development" //shown as the "environment" filter on the Sentry dashboard
};
