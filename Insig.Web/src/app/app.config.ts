import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { MAT_FORM_FIELD_DEFAULT_OPTIONS } from '@angular/material/form-field';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { PreloadAllModules, provideRouter, withInMemoryScrolling, withPreloading, withViewTransitions } from '@angular/router';
import { MSAL_GUARD_CONFIG, MSAL_INSTANCE, MsalBroadcastService, MsalGuard, MsalGuardConfiguration, MsalService } from '@azure/msal-angular';
import { IPublicClientApplication, InteractionType, PublicClientApplication } from '@azure/msal-browser';
import { provideToastr } from 'ngx-toastr';

import { AuthModule, HttpAuthInterceptor, ToastHttpInterceptor } from '@domains/shared/data';

import APP_ROUTES from './app.routes';

function MsalInstanceFactory(): IPublicClientApplication {
    return new PublicClientApplication({
        auth: {
            clientId: AppConfig.Auth.ClientId,
            authority: AppConfig.Auth.Authority,
            knownAuthorities: [AppConfig.Auth.Domain],
            redirectUri: '/auth',
            postLogoutRedirectUri: '/'
        }
    });
}

function MsalGuardConfigFactory(): MsalGuardConfiguration {
    return {
        interactionType: InteractionType.Redirect,
        authRequest: {
            scopes: AppConfig.Auth.Scopes
        }
    };
}

export const appConfig: ApplicationConfig = {
    providers: [
        importProvidersFrom(HttpClientModule, AuthModule.forRoot()),
        provideToastr(),
        provideAnimationsAsync(),
        provideRouter(
            APP_ROUTES,
            withInMemoryScrolling({ scrollPositionRestoration: 'enabled' }),
            withViewTransitions({ skipInitialTransition: true }),
            withPreloading(PreloadAllModules)
        ),
        {
            provide: MAT_FORM_FIELD_DEFAULT_OPTIONS,
            useValue: {
                appearance: 'outline'
            }
        },
        {
            provide: MSAL_INSTANCE,
            useFactory: MsalInstanceFactory
        },
        {
            provide: MSAL_GUARD_CONFIG,
            useFactory: MsalGuardConfigFactory
        },
        {
            provide: HTTP_INTERCEPTORS,
            useClass: HttpAuthInterceptor,
            multi: true
        },
        {
            provide: HTTP_INTERCEPTORS,
            useClass: ToastHttpInterceptor,
            multi: true
        },
        MsalService,
        MsalBroadcastService,
        MsalGuard
    ]
}
