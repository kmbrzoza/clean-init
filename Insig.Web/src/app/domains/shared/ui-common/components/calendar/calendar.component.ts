import {
    AfterViewInit,
    ChangeDetectionStrategy,
    Component,
    EventEmitter,
    Input,
    OnChanges,
    OnDestroy,
    OnInit,
    Output,
    SimpleChanges,
    ViewChild
} from '@angular/core';
import { FullCalendarComponent, FullCalendarModule } from '@fullcalendar/angular';
import { CalendarOptions, EventInput } from '@fullcalendar/core';
import { NgSelectModule } from '@ng-select/ng-select';
import { ReactiveFormsModule } from '@angular/forms';

import { Subject, take, takeUntil } from 'rxjs';

import { CacheData } from '@domains/shared/util-cache';

import { CalendarService } from './calendar.service';
import { CalendarCachedModel, CalendarDateClick, CalendarEventClick } from './calendar-events.model';

@Component({
    selector: 'app-calendar',
    templateUrl: './calendar.component.html',
    styleUrls: ['./calendar.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush,
    standalone: true,
    imports: [
        FullCalendarModule,
        ReactiveFormsModule,
        NgSelectModule
    ],
    providers: [CalendarService]
})
export class CalendarComponent implements OnInit, OnDestroy, OnChanges, AfterViewInit {
    @ViewChild('calendar') calendar!: FullCalendarComponent;

    @Input() events: Nullable<EventInput[]> = [];
    @Input() options: CalendarOptions = {};
    @Output() eventClicked = new EventEmitter<CalendarEventClick>();
    @Output() dateClicked = new EventEmitter<CalendarDateClick>();

    private _destroy = new Subject<void>();

    constructor(private readonly _calendarService: CalendarService) { }

    ngOnInit(): void {
        this.setOptions();
        this.setEventClickListener();
        this.setDateClickListener();
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (this.options && changes.events) {
            this.options.events = changes.events.currentValue;
        }
    }

    ngAfterViewInit(): void {
        this.setCalendar();
        setTimeout(() => this.calendar.getApi().render(), 100);

        if (this._calendarService.isDataCacheEmpty()) {
            this._calendarService.loadEvents();
        } else {
            this._calendarService.getCacheData().pipe(take(1))
                .subscribe(data => {
                    this.loadDataFilters(data);
                    this._calendarService.loadEvents();
                });
        }
    }

    ngOnDestroy(): void {
        this._destroy.next();
        this._destroy.complete();
    }

    private setCalendar(): void {
        this._calendarService.calendar = this.calendar;
    }

    private setOptions(): void {
        this.options = this._calendarService.getDefaultOptions(this.options, this.events);
    }

    private setEventClickListener(): void {
        this._calendarService.eventClicked$
            .pipe(takeUntil(this._destroy))
            .subscribe(args => {
                this.eventClicked.emit({ args, calendar: this.calendar.getApi() });
            });
    }

    private setDateClickListener(): void {
        this._calendarService.dateClicked$
            .pipe(takeUntil(this._destroy))
            .subscribe(args => {
                this.dateClicked.emit({ args, calendar: this.calendar.getApi() });
            });
    }

    private loadDataFilters(data: CacheData<CalendarCachedModel>): void {
        if (data.filters.calendarViewType) {
            this._calendarService.changeView(data.filters.calendarViewType);
        }
        if (data.filters.dateRange.from) {
            this.calendar.getApi().gotoDate(data.filters.dateRange.from);
        }
    }
}
