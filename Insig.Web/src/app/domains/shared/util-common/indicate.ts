import { WritableSignal } from '@angular/core';

import { defer, Observable, Subject, finalize } from 'rxjs';

const prepare = <T>(callback: () => void) => (source: Observable<T>): Observable<T> => defer(() => {
    callback();
    return source;
});

export const indicate = <T>(indicator: Subject<boolean>) => (source: Observable<T>): Observable<T> => source.pipe(
    prepare(() => indicator.next(true)),
    finalize(() => indicator.next(false))
);

export const indicateSignal = <T>(indicator: WritableSignal<boolean>) => (source: Observable<T>): Observable<T> => source.pipe(
    prepare(() => indicator.set(true)),
    finalize(() => indicator.set(false))
)
