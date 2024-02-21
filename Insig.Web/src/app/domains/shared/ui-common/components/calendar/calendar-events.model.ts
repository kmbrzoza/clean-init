import { Calendar, EventClickArg } from '@fullcalendar/core';
import { DateClickArg } from '@fullcalendar/interaction';

import { CalendarViewType } from './calendar-view-type.enum';
import { DateRange } from './date-range.model';

export interface CalendarEventClick {
    args: EventClickArg,
    calendar: Calendar
}

export interface CalendarDateClick {
    args: DateClickArg,
    calendar: Calendar
}

export type CalendarCachedModel = {
    calendarViewType: CalendarViewType;
    dateRange: DateRange;
};


