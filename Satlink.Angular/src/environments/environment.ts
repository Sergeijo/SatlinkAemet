export const environment = {
  production: false,

  // Satlink.Api base URL — empty string uses the Angular dev-server proxy (proxy.conf.json)
  baseApiUrl: '',

  // Angular app origin — used for OIDC redirect URIs (no window dependency)
  appUrl: 'http://localhost:4200',

  // Duende Identity Server URL
  identityServerUrl: 'https://localhost:5001',

  // AEMET base URL (WPF AppConfig:url)
  aemetUrl: 'https://opendata.aemet.es/opendata/api/prediccion/maritima/altamar/area',

  // AEMET API Key (WPF AppConfig:api_key)
  apiKey: 'eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJzZXJnZWlqbzg0QGdtYWlsLmNvbSIsImp0aSI6ImFlNGUzZGMyLTk2MmMtNDlhZi05NDQxLTQ2MmZlNTI2ODViZSIsImlzcyI6IkFFTUVUIiwiaWF0IjoxNzI2ODU5NjgyLCJ1c2VySWQiOiJhZTRlM2RjMi05NjJjLTQ5YWYtOTQ0MS00NjJmZTUyNjg1YmUiLCJyb2xlIjoiIn0.UZUPEJBD6pLn-AZJOrjXkRkqtK9MSE9lRmoyz7PXG_A'
};
