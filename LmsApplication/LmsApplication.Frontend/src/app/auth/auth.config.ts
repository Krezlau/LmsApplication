import { PassedInitialConfig } from 'angular-auth-oidc-client';

export const authConfig: PassedInitialConfig = {
  config: {
            authority: 'https://login.microsoftonline.com/40dcee2a-c051-4a80-acba-953dac9a3942/v2.0',
            authWellknownEndpointUrl: 'https://login.microsoftonline.com/common/v2.0',
            redirectUrl: window.location.origin,
            clientId: 'cb578bf0-f9bc-4fd0-9329-645b7cdbe367',
            scope: 'openid profile offline_access User.Read', // 'openid profile offline_access ' + your scopes
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
