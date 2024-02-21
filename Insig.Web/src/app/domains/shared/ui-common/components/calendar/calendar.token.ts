import { InjectionToken } from '@angular/core';

import { CalendarDataApi } from './calendar-data-api.interface';

export const CALENDAR_DATA_API = new InjectionToken<CalendarDataApi>('CALENDAR_DATA_API');
