import { PassedInitialConfig } from 'angular-auth-oidc-client';

export const authConfig: PassedInitialConfig = {
  config: [{
    configId: 'tenant1',
    authority: 'https://login.microsoftonline.com/40dcee2a-c051-4a80-acba-953dac9a3942/b2x_susi/v2.0',
    authWellknownEndpointUrl: 'https://login.microsoftonline.com/40dcee2a-c051-4a80-acba-953dac9a3942/v2.0',
    redirectUrl: 'http://localhost:4200/tenant1/home',
    // unauthorizedRoute: window.location.origin,
    clientId: '9029144a-5bb5-4a29-875c-81d66249d483',
    scope: 'openid profile offline_access email api://lms-api/tasks.read', // 'openid profile offline_access ' + your scopes
    responseType: 'code',
    silentRenew: true,
    useRefreshToken: true,
    ignoreNonceAfterRefresh: false,
    maxIdTokenIatOffsetAllowedInSeconds: 600,
    issValidationOff: false,
    autoUserInfo: false,
    customParamsAuthRequest: {
      prompt: 'select_account', // login, consent
    },
    secureRoutes: ['http://localhost:8080/'],
  },
    {
    configId: 'tenant2',
    authority: 'https://login.microsoftonline.com/93ba8a8c-bd33-4b98-af36-dcc63dc2f84e/b2x_susi/v2.0',
    authWellknownEndpointUrl: 'https://login.microsoftonline.com/93ba8a8c-bd33-4b98-af36-dcc63dc2f84e/v2.0',
    redirectUrl: 'http://localhost:4200/tenant2/home',
    // unauthorizedRoute: window.location.origin,
    clientId: '1ef76dc1-ceb1-4243-b477-fc4d72de170a',
    scope: 'openid profile offline_access email api://lms-api-2/app.access', // 'openid profile offline_access ' + your scopes
    responseType: 'code',
    silentRenew: true,
    useRefreshToken: true,
    ignoreNonceAfterRefresh: false,
    maxIdTokenIatOffsetAllowedInSeconds: 600,
    issValidationOff: false,
    autoUserInfo: false,
    customParamsAuthRequest: {
      prompt: 'select_account', // login, consent
    },
    secureRoutes: ['http://localhost:8080/'],
  }]
}
