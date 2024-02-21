import { Component } from '@angular/core';

import { CALENDAR_DATA_API, CalendarComponent } from '@domains/shared/ui-common';

import { CalendarDataService } from './data/calendar-data.service';


@Component({
    selector: 'app-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrl: './dashboard.component.scss',
    standalone: true,
    imports: [
        CalendarComponent
    ],
    providers: [
        { provide: CALENDAR_DATA_API, useClass: CalendarDataService }
    ]
})
export class DashboardComponent {
}
