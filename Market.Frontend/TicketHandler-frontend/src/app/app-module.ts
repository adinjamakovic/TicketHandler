import {
  APP_INITIALIZER,
  ErrorHandler,
  NgModule,
  inject,
  provideAppInitializer,
  provideBrowserGlobalErrorListeners,
  provideZoneChangeDetection
} from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { provideAnimations} from '@angular/platform-browser/animations';
import {HttpClient, provideHttpClient, withInterceptors} from '@angular/common/http';
import {LogLevel, OidcSecurityService, provideAuth} from 'angular-auth-oidc-client';
import {catchError, of} from 'rxjs';
import {environment} from '../environments/environment';
import * as Sentry from "@sentry/angular";
import { AppRoutingModule } from './app-routing-module';
import { AppComponent } from './app.component';
import {authInterceptor} from './core/interceptors/auth-interceptor.service';
import {AuthFacadeService} from './core/services/auth/auth-facade.service';
import {loadingBarInterceptor} from './core/interceptors/loading-bar-interceptor.service';
import {errorLoggingInterceptor} from './core/interceptors/error-logging-interceptor.service';
import {TranslateLoader, TranslateModule} from '@ngx-translate/core';
import {CustomTranslateLoader} from './core/services/custom-translate-loader';
import {materialModules} from './modules/shared/material-modules';
import {SharedModule} from './modules/shared/shared-module';
import { TicketsComponent } from './modules/organizers/tickets/tickets.component';
import { TicketTypesComponent } from './modules/organizers/ticket-types/ticket-types.component';
import { Router } from '@angular/router';

@NgModule({
  declarations: [
    AppComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: (http: HttpClient) => new CustomTranslateLoader(http),
        deps: [HttpClient]
      }
    }),
    SharedModule,
    materialModules,
  ],
  providers: [
    {
      provide: ErrorHandler,
      useValue: Sentry.createErrorHandler({
        showDialog: true
      })
    }, 
    {
      provide: Sentry.TraceService,
      deps: [Router]
    },
    {
      provide: APP_INITIALIZER,
      useFactory: () => () => {},
      deps: [Sentry.TraceService],
      multi: true
    },
    provideAnimations(),
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection(),
    provideHttpClient(
      withInterceptors([
        loadingBarInterceptor,
        authInterceptor,
        errorLoggingInterceptor
      ])
    ),
    provideAuth({
      config: {
        authority: environment.oidcAuthority,
        clientId: environment.oidcClientId,
        scope: environment.oidcScope,
        responseType: 'code',
        redirectUrl: `${window.location.origin}/auth/callback`,
        postLogoutRedirectUri: window.location.origin,
        silentRenew: true,
        useRefreshToken: true,
        ignoreNonceAfterRefresh: true,
        renewTimeBeforeTokenExpiresInSeconds: 60,
        autoUserInfo: false,
        logLevel: environment.production ? LogLevel.Error : LogLevel.Warn
      }
    }),
    provideAppInitializer(() => {
      inject(AuthFacadeService); 
      return inject(OidcSecurityService).checkAuth().pipe(
        catchError((err) => {
          console.error('OIDC checkAuth failed:', err);
          return of(null);
        })
      );
    })
  ],
  exports: [
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
