import { AsyncPipe } from '@angular/common';
import { Component, input } from '@angular/core';
import { MatBadgeModule } from '@angular/material/badge';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { RouterLink, RouterLinkActive } from '@angular/router';

import { AuthService, MessageService } from '@domains/shared/data';

@Component({
    selector: 'app-admin-account',
    templateUrl: './admin-account.component.html',
    styleUrls: [
        '../admin-menu/admin-menu.component.scss',
        './admin-account.component.scss'
    ],
    standalone: true,
    imports: [
        MatIconModule,
        MatListModule,
        MatBadgeModule,
        RouterLink,
        RouterLinkActive,
        AsyncPipe
    ]
})
export class AdminAccountComponent {
    collapsed = input.required<boolean>();

    account = this._authService.account;
    unreadMessagesCount$ = this._messageService.unreadNotificationsCount$;

    constructor(
        private readonly _authService: AuthService,
        private readonly _messageService: MessageService
    ) { }
}
