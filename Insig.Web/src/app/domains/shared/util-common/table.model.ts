import { InjectionToken } from '@angular/core';

import { BehaviorSubject, Observable, Subject } from 'rxjs';

import { PaginationCriteria } from './search.model';
import { Page } from './page.model';

/* eslint-disable @typescript-eslint/no-explicit-any */
export const TABLE_TOKEN = new InjectionToken<Table<any>>('TableToken');

export interface Table<T, K = undefined> {
    readonly clearSelections$?: Subject<boolean>;
    readonly searchCriteria$?: BehaviorSubject<K>;
    getData(paginationCriteria: PaginationCriteria): Observable<Page<T>>;
}

export type TableActionEmitter<T> = {
    item: T;
    refresh: () => void;
};

export type TableOptions = {
    showSelections: boolean;
};
