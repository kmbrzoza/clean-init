import { Inject, Injectable, OnDestroy } from '@angular/core';
import { FullCalendarComponent } from '@fullcalendar/angular';
import { Calendar, CalendarOptions, EventClickArg, EventInput, EventMountArg } from '@fullcalendar/core';
import dayGridPlugin from '@fullcalendar/daygrid';
import interactionPlugin, { DateClickArg } from '@fullcalendar/interaction';
import listPlugin from '@fullcalendar/list';
import timeGridPlugin from '@fullcalendar/timegrid';
import dayjs from 'dayjs';
import tippy from 'tippy.js';

import { defer, iif, Observable, ReplaySubject, Subject, switchMap, takeUntil, tap } from 'rxjs';

import { Helper } from '@domains/shared/util-date';
import { CacheData } from '@domains/shared/util-cache';

import { CalendarDataApi } from './calendar-data-api.interface';
import { CalendarCachedModel } from './calendar-events.model';
import { CalendarViewType } from './calendar-view-type.enum';
import { CALENDAR_DATA_API } from './calendar.token';
import { DateRange } from './date-range.model';

@Injectable()
export class CalendarService implements OnDestroy {
    dateChanged$ = new ReplaySubject<DateRange>();
    eventClicked$ = new Subject<EventClickArg>();
    dateClicked$ = new Subject<DateClickArg>();

    private _api!: Calendar;
    private _dateRangeLoaded: DateRange[] = [];
    private _cachedEvents: EventInput[] = [];
    private _destroy = new Subject<void>();
    private _currentCalendarViewType = CalendarViewType.Month;

    constructor(@Inject(CALENDAR_DATA_API) private _calendarDataApi: CalendarDataApi) {
        if (!this._calendarDataApi) {
            throw new Error('Provide CALENDAR_DATA_API to use CalendarComponent');
        }

        this.dateChanged$.pipe(
            switchMap(dateRange => iif(
                () => !this.isDateRangeLoaded(dateRange),
                defer(() => this._calendarDataApi.getData(dateRange).pipe(tap(() => this.addCurrentDateRange(dateRange)))),
                []
            )),
            takeUntil(this._destroy)
        ).subscribe(data => {
            const events = this._calendarDataApi.getEventInputs(data);

            this.setEvents(events);
        });
    }

    set calendar(calendar: FullCalendarComponent) {
        this._api = calendar.getApi();

        this._api.on('datesSet', _ => {
            this.setDateRangeInCache();
        });
    }

    getDefaultOptions(options: CalendarOptions, events: Nullable<EventInput[]>): CalendarOptions {
        return {
            plugins: [
                dayGridPlugin,
                timeGridPlugin,
                listPlugin,
                interactionPlugin
            ],
            height: 800,
            noEventsContent: 'Brak zadań.',
            allDayContent: 'cały dzień',
            moreLinkContent: args => `+${args.num} Więcej`,
            buttonText: {
                today: 'Dzisiaj'
            },
            locale: 'pl',
            headerToolbar: {
                left: 'prev next today',
                right: `${CalendarViewType.Month},${CalendarViewType.Week},${CalendarViewType.Day},${CalendarViewType.List}`,
                center: 'title'
            },
            initialView: 'dayGridMonth',
            longPressDelay: 0,
            eventLongPressDelay: 0,
            selectLongPressDelay: 0,
            weekends: true,
            selectable: true,
            dayMaxEvents: true,
            unselectAuto: false,
            firstDay: 1,
            events: events || [],
            eventClick: this.onEventClick.bind(this),
            customButtons: {
                prev: {
                    text: 'prev',
                    click: this.previousDate.bind(this)
                },
                next: {
                    text: 'next',
                    click: this.nextDate.bind(this)
                },
                [CalendarViewType.Month]: {
                    text: 'Miesięczny',
                    click: () => this.changeView(CalendarViewType.Month)
                },
                [CalendarViewType.Week]: {
                    text: 'Tygodniowy',
                    click: () => this.changeView(CalendarViewType.Week)
                },
                [CalendarViewType.Day]: {
                    text: 'Dzienny',
                    click: () => this.changeView(CalendarViewType.Day)
                },
                [CalendarViewType.List]: {
                    text: 'Lista',
                    click: () => this.changeView(CalendarViewType.List)
                }
            },
            eventDidMount: (arg: EventMountArg) => {
                tippy(arg?.el, {
                    content: arg?.event?.extendedProps.description,
                    placement: 'top',
                    trigger: 'mouseenter',
                    appendTo: document.querySelector('app-calendar') as HTMLElement
                });
            },
            dateClick: this.onDateClick.bind(this),
            eventTimeFormat: {
                hour: '2-digit',
                minute: '2-digit',
                meridiem: false
            },
            ...options
        };
    }

    setEvents(events: EventInput[]): void {
        for (const event of events) {
            this.addEventInput(event);
        }
    }

    loadEvents(): void {
        const [from, to] = this.getRange();
        this.dateChanged$.next({ from, to });
    }

    ngOnDestroy(): void {
        this._destroy.next();
        this._destroy.complete();
    }

    changeView(type: CalendarViewType): void {
        this._api.changeView(type);
        this._currentCalendarViewType = type;
        this._calendarDataApi.saveDataCacheCalendarType(type);
        this.clearEvents();
    }

    isDataCacheEmpty(): boolean {
        return this._calendarDataApi.isDataCacheEmpty();
    }

    getCacheData(): Observable<CacheData<CalendarCachedModel>> {
        return this._calendarDataApi.getCacheData();
    }

    private onEventClick(arg: EventClickArg): void {
        this.eventClicked$.next(arg);
    }

    private previousDate(): void {
        this._api.prev();
        this.loadEvents();
    }

    private nextDate(): void {
        this._api.next();
        this.loadEvents();
    }

    private clearEvents(): void {
        this._api.removeAllEvents();
        this._cachedEvents = [];
        this._dateRangeLoaded = [];
    }

    private getRange(): [string, string] {
        const { view } = this._api;

        return [
            Helper.formatDate(view.activeStart),
            Helper.formatDate(view.activeEnd)
        ];
    }

    private onDateClick(arg: DateClickArg): void {
        this.dateClicked$.next(arg);
    }

    private isEventInputExists(eventInput: EventInput): boolean {
        return !!this._cachedEvents.find(c => c.id === eventInput.id);
    }

    private addEventInput(eventInput: EventInput): void {
        if (!this.isEventInputExists(eventInput)) {
            this._api.addEvent(eventInput);
            this._cachedEvents = [...this._cachedEvents, eventInput];
        }
    }

    private addCurrentDateRange(dateRange: DateRange): void {
        if (!this.isDateRangeLoaded(dateRange)) {
            this._dateRangeLoaded = [...this._dateRangeLoaded, dateRange];
        }
    }

    private isDateRangeLoaded(dateRange: DateRange): boolean {
        return this._dateRangeLoaded.some(d => d.from === dateRange.from && d.to === dateRange.to);
    }

    private setDateRangeInCache(): void {
        const [from, to] = this.getRange();

        if (this._currentCalendarViewType === CalendarViewType.Month) {
            const fromDate = dayjs(from);
            let formattedDateFrom = dayjs(fromDate.clone().startOf('month').format('YYYY-MM-DD'));
            if (!formattedDateFrom.isAfter(fromDate, 'day')) {
                formattedDateFrom = formattedDateFrom.add(1, 'month');
            }
            const formattedDateTo = formattedDateFrom.clone().endOf('month').format('YYYY-MM-DD');
            this._calendarDataApi.saveDataCacheDateRange({ from: formattedDateFrom.format('YYYY-MM-DD'), to: formattedDateTo });
        } else {
            this._calendarDataApi.saveDataCacheDateRange({ from, to });
        }
    }
}
