import { Component, input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { RouterLink, RouterLinkActive } from '@angular/router';

import { AuthService } from '@domains/shared/data';

@Component({
    selector: 'app-admin-menu',
    templateUrl: './admin-menu.component.html',
    styleUrl: './admin-menu.component.scss',
    standalone: true,
    imports: [
        MatIconModule,
        MatListModule,
        RouterLink,
        RouterLinkActive
    ]
})
export class AdminMenuComponent {
    collapsed = input.required<boolean>();

    constructor(private readonly _authService: AuthService) { }

    onLogoutClick(): void {
        this._authService.logout();
    }
}
