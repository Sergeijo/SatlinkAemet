export const environment = {
  production: false,

  // Satlink.Api base URL (WPF default: http://localhost:5273/)
  // Use same-origin in dev; requests are proxied to Satlink.Api via proxy.conf.json
  baseApiUrl: '',

  // AEMET base URL (WPF AppConfig:url)
  aemetUrl: 'https://opendata.aemet.es/opendata/api/prediccion/maritima/altamar/area',

  // AEMET API Key (WPF AppConfig:api_key)
  apiKey: 'eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJzZXJnZWlqbzg0QGdtYWlsLmNvbSIsImp0aSI6ImFlNGUzZGMyLTk2MmMtNDlhZi05NDQxLTQ2MmZlNTI2ODViZSIsImlzcyI6IkFFTUVUIiwiaWF0IjoxNzI2ODU5NjgyLCJ1c2VySWQiOiJhZTRlM2RjMi05NjJjLTQ5YWYtOTQ0MS00NjJmZTUyNjg1YmUiLCJyb2xlIjoiIn0.UZUPEJBD6pLn-AZJOrjXkRkqtK9MSE9lRmoyz7PXG_A'
};
