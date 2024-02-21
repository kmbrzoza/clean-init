import { Injectable } from '@angular/core';

import { BehaviorSubject, map, Observable } from 'rxjs';

import { Cache, CacheDataFilters, CacheData } from './cache.model';
import { CachedOptions } from './cache-options.enum';

export type CacheStorage = 'calendarCachedOptions';

@Injectable({
    providedIn: 'root'
})
export class CacheService<T extends CacheDataFilters> {
    private _cacheSubject = new BehaviorSubject<Nullable<Cache>>(null);

    isEmpty(key: number, nameStorage: CacheStorage): boolean {
        this.setCacheSubject(nameStorage);

        const cache = this._cacheSubject.getValue() ?? this.getFromLocalStorage(nameStorage);

        return !cache[key]?.filters;
    }

    getData(key: CachedOptions, nameStorage: CacheStorage): Observable<CacheData<T>> {
        this.setCacheSubject(nameStorage);

        return this._cacheSubject.asObservable().pipe(
            map(cache => cache ?? this.getFromLocalStorage(nameStorage)),
            map(cache => ((cache && cache[key]) ?? { filters: {} }) as CacheData<T>)
        );
    }

    getValue(key: number, nameStorage: CacheStorage): CacheData<T> {
        this.setCacheSubject(nameStorage);

        const cache = this._cacheSubject.getValue() ?? this.getFromLocalStorage(nameStorage);

        return ((cache && cache[key]) ?? { filters: {} }) as CacheData<T>;
    }

    setDataFilters(key: number, filters: T, nameStorage: CacheStorage): void {
        this.setToLocalStorage(key, { filters }, nameStorage);
    }

    clear(nameStorage: CacheStorage): void {
        this.setCacheSubject(nameStorage);
        localStorage.removeItem(nameStorage);
    }

    private setCacheSubject(nameStorage: CacheStorage): void {
        this._cacheSubject.next(this.getFromLocalStorage(nameStorage));
    }

    private getFromLocalStorage(nameStorage: CacheStorage): Cache {
        const data = localStorage.getItem(nameStorage) || '';

        try {
            const parsedData = JSON.parse(data);

            this._cacheSubject.next(parsedData);

            return parsedData;
        } catch {
            return {} as Cache;
        }
    }

    private setToLocalStorage(key: number, data: Partial<CacheData<T>>, nameStorage: CacheStorage): void {
        this.setCacheSubject(nameStorage);

        const cache = this.getFromLocalStorage(nameStorage);
        const cachedFilters = (cache && cache[key]?.filters);
        const newData = {
            ...cache,
            [key]: {
                filters: data.filters ?? cachedFilters
            }
        };

        localStorage.setItem(nameStorage, JSON.stringify(newData));
        this._cacheSubject.next(newData);
    }
}
