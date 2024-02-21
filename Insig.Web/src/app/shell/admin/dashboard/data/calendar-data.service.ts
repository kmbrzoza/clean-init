import { Injectable } from '@angular/core';
import { EventInput } from '@fullcalendar/core';
import dayjs from 'dayjs';

import { Observable, of } from 'rxjs';

import { CalendarCachedModel, CalendarDataApi, CalendarViewType, DateRange } from '@domains/shared/ui-common';
import { CacheData, CacheService, CachedOptions } from '@domains/shared/util-cache';

interface ExampleCalendarData {
    id: number,
    text: string;
    date: string;
}

@Injectable({
    providedIn: 'root'
})
export class CalendarDataService implements CalendarDataApi<ExampleCalendarData> {

    constructor(private readonly _cacheService: CacheService<CalendarCachedModel>) { }
    getData(dateRange: DateRange): Observable<ExampleCalendarData[]> {
        if (dateRange.from === '2024-01-29' && dateRange.to === '2024-03-11') {
            return of([
                {
                    id: 1,
                    text: 'Test event',
                    date: dayjs().toISOString()
                }
            ]);
        }

        return of([]);
    }
    getEventInputs(data: ExampleCalendarData[]): EventInput[] {
        return data.map(d => ({
            id: d.id.toString(),
            title: d.text,
            description: 'test',
            display: 'block',
            allDay: false,
            start: d.date,
            end: d.date
        }))
    }

    saveDataCacheCalendarType(type: CalendarViewType): void {
        const data = this._cacheService.getValue(CachedOptions.Calendar, 'calendarCachedOptions');

        data.filters.calendarViewType = type;

        this._cacheService.setDataFilters(CachedOptions.Calendar, data.filters, 'calendarCachedOptions');
    }

    saveDataCacheDateRange(dateRange: DateRange): void {
        const data = this._cacheService.getValue(CachedOptions.Calendar, 'calendarCachedOptions');
        data.filters.dateRange = dateRange;

        this._cacheService.setDataFilters(CachedOptions.Calendar, data.filters, 'calendarCachedOptions');
    }

    isDataCacheEmpty(): boolean {
        return (this._cacheService.isEmpty(CachedOptions.Calendar, 'calendarCachedOptions'));
    }

    getCacheData(): Observable<CacheData<CalendarCachedModel>> {
        return this._cacheService.getData(CachedOptions.Calendar, 'calendarCachedOptions');
    }
}
