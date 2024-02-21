import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
    selector: 'app-access-denied',
    templateUrl: './access-denied.component.html',
    styleUrls: ['./access-denied.component.scss'],
    standalone: true,
    imports: [
        RouterLink
    ]
})
export class AccessDeniedComponent { }
