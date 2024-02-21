import { Component, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav';
import { RouterOutlet } from '@angular/router';

import { AdminMenuComponent } from './admin-menu/admin-menu.component';
import { AdminAccountComponent } from './admin-account/admin-account.component';

@Component({
    selector: 'app-admin-wrapper',
    templateUrl: './admin-wrapper.component.html',
    styleUrl: './admin-wrapper.component.scss',
    standalone: true,
    imports: [
        RouterOutlet,
        MatSidenavModule,
        MatButtonModule,
        MatIconModule,
        AdminMenuComponent,
        AdminAccountComponent
    ]
})
export class AdminWrapperComponent {
    collapsed = signal<boolean>(false);

    toggleDrawer(): void {
        this.collapsed.set(!this.collapsed());
    }

}
