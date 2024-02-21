import { Component, input } from '@angular/core';

@Component({
    selector: 'app-page-header',
    templateUrl: './page-header.component.html',
    styleUrl: './page-header.component.scss',
    standalone: true,
    imports: []
})
export class PageHeaderComponent {
    header = input.required<string>();
}
