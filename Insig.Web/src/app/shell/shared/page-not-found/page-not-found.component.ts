import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { RouterLink } from '@angular/router';

@Component({
    selector: 'app-page-not-found',
    templateUrl: './page-not-found.component.html',
    styleUrls: ['page-not-found.component.scss'],
    standalone: true,
    imports: [
        MatCardModule,
        RouterLink
    ]
})
export class PageNotFoundComponent { }
