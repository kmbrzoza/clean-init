import { AsyncPipe, NgIf } from '@angular/common';
import { Component, Inject, OnInit } from '@angular/core';
import { MAT_FORM_FIELD_DEFAULT_OPTIONS, MatFormFieldDefaultOptions } from '@angular/material/form-field';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { RouterOutlet } from '@angular/router';
import { MSAL_GUARD_CONFIG, MsalBroadcastService, MsalGuardConfiguration, MsalService } from '@azure/msal-angular';
import { NgSelectConfig } from '@ng-select/ng-select';

import { AuthService, NotificationsHubService } from '@domains/shared/data';

import { NavbarComponent } from './shell/site/navbar/navbar.component';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrl: './app.component.scss',
    standalone: true,
    imports: [
        NavbarComponent,
        RouterOutlet,
        MatProgressSpinnerModule,
        NgIf,
        AsyncPipe
    ]
})
export class AppComponent implements OnInit {
    isLoading$ = this._authService.isLoading$;

    constructor(
        private _msalService: MsalService,
        // Guard and Broadcast are unused but required for msal authentication
        @Inject(MSAL_GUARD_CONFIG) private _msalGuardConfig: MsalGuardConfiguration,
        private _msalBroadcastService: MsalBroadcastService,
        private _config: NgSelectConfig,
        @Inject(MAT_FORM_FIELD_DEFAULT_OPTIONS) private _matOptions: MatFormFieldDefaultOptions,
        private _authService: AuthService,
        private _notificationsHubService: NotificationsHubService
    ) {
        if (this._matOptions.appearance) {
            this._config.appearance = this._matOptions.appearance;
            this._config.bindLabel = 'name';
            this._config.bindValue = 'id';
        }
    }

    ngOnInit(): void {
        this._notificationsHubService.init();
        this._msalService.handleRedirectObservable().subscribe();
    }
}
