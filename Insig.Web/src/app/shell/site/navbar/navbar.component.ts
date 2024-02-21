import { Component } from '@angular/core';
import { MatToolbarModule } from '@angular/material/toolbar';
import { RouterLink } from '@angular/router';

import { MenuComponent } from './menu/menu.component';
import { UserMenuComponent } from './user-menu/user-menu.component';
import { MenuHamburgerComponent } from './menu-hamburger/menu-hamburger.component';

@Component({
    selector: 'app-navbar',
    templateUrl: 'navbar.component.html',
    styleUrl: './navbar.component.scss',
    standalone: true,
    imports: [
        MatToolbarModule,
        RouterLink,
        MenuComponent,
        UserMenuComponent,
        MenuHamburgerComponent
    ]
})
export class NavbarComponent {}
