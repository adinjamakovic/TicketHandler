export const environment = {
  production: false,     // staging obično nije "production"
  apiUrl: 'https://staging.api.myapp.com',
  oidcAuthority: 'https://staging.identity.myapp.com',
  oidcClientId: 'market.spa',
  oidcScope: 'openid profile email market.api offline_access'
};
