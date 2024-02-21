import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MatCardModule } from '@angular/material/card';

import { NavbarComponent } from '../navbar/navbar.component';

@Component({
    selector: 'app-site-wrapper',
    templateUrl: './site-wrapper.component.html',
    styleUrl: './site-wrapper.component.scss',
    standalone: true,
    imports: [
        RouterOutlet,
        NavbarComponent,
        MatCardModule
    ]
})
export class SiteWrapperComponent { }
