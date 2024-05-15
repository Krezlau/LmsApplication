import { PassedInitialConfig } from 'angular-auth-oidc-client';

export const authConfig: PassedInitialConfig = {
  config: {
            authority: 'https://login.microsoftonline.com/40dcee2a-c051-4a80-acba-953dac9a3942/b2x_susi/v2.0',
            authWellknownEndpointUrl: 'https://login.microsoftonline.com/common/v2.0',
            redirectUrl: window.location.origin,
            // unauthorizedRoute: window.location.origin,
            clientId: '9029144a-5bb5-4a29-875c-81d66249d483',
            scope: 'api://lms-api/tasks.read', // 'openid profile offline_access ' + your scopes
            responseType: 'code',
            silentRenew: false,
            useRefreshToken: true,
            ignoreNonceAfterRefresh: false,
            maxIdTokenIatOffsetAllowedInSeconds: 600,
            issValidationOff: true,
            autoUserInfo: false,
            customParamsAuthRequest: {
              prompt: 'select_account', // login, consent
            },
    }
}
