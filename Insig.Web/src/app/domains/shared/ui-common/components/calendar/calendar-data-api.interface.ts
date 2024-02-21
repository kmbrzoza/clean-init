import { EventInput } from '@fullcalendar/core';

import { Observable } from 'rxjs';

import { CacheData } from '@domains/shared/util-cache';

import { CalendarCachedModel } from './calendar-events.model';
import { CalendarViewType } from './calendar-view-type.enum';
import { DateRange } from './date-range.model';

export interface CalendarDataApi<T = unknown> {
    getData(dateRange: DateRange): Observable<T[]>;
    getEventInputs(data: T[]): EventInput[];
    saveDataCacheCalendarType(type: CalendarViewType): void;
    saveDataCacheDateRange(dateRange: DateRange): void;
    isDataCacheEmpty(): boolean;
    getCacheData(): Observable<CacheData<CalendarCachedModel>>;
}
