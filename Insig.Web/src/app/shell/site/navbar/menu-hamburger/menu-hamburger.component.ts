import { Component, ElementRef, HostListener, OnInit } from '@angular/core';
import { NavigationStart, Router, RouterLink, RouterLinkActive } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { AsyncPipe, NgClass } from '@angular/common';

import { Observable } from 'rxjs';

import { AuthService } from '@domains/shared/data';

@Component({
    selector: 'app-menu-hamburger',
    templateUrl: './menu-hamburger.component.html',
    styleUrls: ['./menu-hamburger.component.scss'],
    standalone: true,
    imports: [
        RouterLink,
        RouterLinkActive,
        MatButtonModule,
        MatIconModule,
        NgClass,
        AsyncPipe
    ]
})
export class MenuHamburgerComponent implements OnInit {
    authStatus$!: Observable<boolean>;
    toggleMenu = false;

    constructor(
        router: Router,
        private _authService: AuthService,
        private _el: ElementRef
    ) {
        router.events.subscribe(event => {
            if (event instanceof NavigationStart) {
                this.toggleMenu = false;
            }
        });
    }

    @HostListener('document:click', ['$event.target'])
    public onClick(target: Element) {
        // eslint-disable-next-line @typescript-eslint/no-unsafe-call
        const clickedInside = this._el.nativeElement.contains(target);
        if (!clickedInside) {
            this.toggleMenu = false;
        }
    }

    ngOnInit(): void {
        this.authStatus$ = this._authService.isAuthenticated$;
    }

    signout(): void {
        this._authService.logout();
    }
}
