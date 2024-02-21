

import { AsyncPipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { Router, RouterLink } from '@angular/router';

import { Observable } from 'rxjs';

import { ProfileAvatarComponent, RequiredRolesDirective } from '@domains/shared/ui-common';
import { AuthService, Roles } from '@domains/shared/data';

@Component({
    selector: 'app-user-menu',
    templateUrl: './user-menu.component.html',
    styleUrls: ['./user-menu.component.scss'],
    standalone: true,
    imports: [
        MatIconModule,
        MatBadgeModule,
        MatMenuModule,
        MatButtonModule,
        ProfileAvatarComponent,
        AsyncPipe,
        RouterLink,
        RequiredRolesDirective
    ]
})
export class UserMenuComponent implements OnInit {
    authStatus$!: Observable<boolean>;
    menuOpened = false;
    roles = Roles;
    account = this._authService.account;

    constructor(
        private readonly _authService: AuthService,
        private _router: Router
    ) { }

    ngOnInit(): void {
        this.authStatus$ = this._authService.isAuthenticated$;
    }

    login(): void {
        this._authService.login();
    }

    register(): void {
        this._authService.register();
    }

    signout(): void {
        this._authService.logout();
    }

    messages(): void {
        const messagesPath = '/profile/messages';
        if (this._router.url === messagesPath) {
            window.location.reload();
        } else {
            this._router.navigateByUrl(messagesPath);
        }
    }
}
