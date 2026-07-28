import { platformBrowser } from '@angular/platform-browser';
import { AppModule } from './app/app-module';
import * as Sentry from "@sentry/angular";

import 'zone.js'; // Razvoj softvera 1 setup, prethodno instalirati "npm install zone.js"
import { environment } from './environments/environment';

if (!environment.SentryDsn) {
  console.warn('Sentry DSN is empty - no errors will be reported for this build.');
}

Sentry.init({
  dsn: environment.SentryDsn,
  // Keeps dev/staging/prod issues in separate buckets on the dashboard.
  environment: environment.sentryEnvironment,
  integrations: [
    Sentry.browserTracingIntegration(),
    Sentry.replayIntegration()
  ],
  // Tracing
  tracesSampleRate: 1.0, //  Capture 100% of the transactions
  // Set 'tracePropagationTargets' to control for which URLs distributed tracing should be enabled.
  tracePropagationTargets: ["localhost", environment.apiUrl],
  // Session Replay
  replaysSessionSampleRate: 0.1, // This sets the sample rate at 10%. You may want to change it to 100% while in development and then sample at a lower rate in production.
  replaysOnErrorSampleRate: 1.0, // If you're not already sampling the entire session, change the sample rate to 100% when sampling sessions where errors occur.,
  // Enable sending logs to Sentry
  enableLogs: true
});

platformBrowser()
  .bootstrapModule(AppModule)
  .catch((err) => console.error(err));
