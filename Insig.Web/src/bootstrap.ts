import { bootstrapApplication } from '@angular/platform-browser';

import { AppComponent } from './app/app.component';
import { appConfig } from './app/app.config';


fetch('assets/appsettings.json')
    .then((response) => response.json())
    .then((json) => {
        // @ts-expect-error: Neccessary for global AppConfig initialization
        window.AppConfig = json.AppConfig;

        bootstrapApplication(AppComponent, appConfig)
            // eslint-disable-next-line no-console
            .catch((err) => console.error(err));
    });
