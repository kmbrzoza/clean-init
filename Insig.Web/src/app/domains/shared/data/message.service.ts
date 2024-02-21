import { Injectable } from '@angular/core';

import { map, Observable, shareReplay, startWith, Subject, switchMap } from 'rxjs';

import { ApiClientService } from '../util-api';


@Injectable({
    providedIn: 'root'
})
export class MessageService {
    unreadNotificationsCount$: Observable<string>;

    private _refreshUnreadNotificationsCount$ = new Subject<void>();

    constructor(private readonly _apiClientService: ApiClientService) {
        this.unreadNotificationsCount$ = this._refreshUnreadNotificationsCount$
            .pipe(
                startWith(null),
                switchMap(_ => this.getUnreadNotificationsCount()),
                map(count => {
                    if (count > 9) {
                        return '9+';
                    } else if (count === 0) {
                        return '';
                    }

                    return count.toString();

                }),
                shareReplay({ refCount: true, bufferSize: 1 })
            );
    }

    refereshNotifications(): void {
        this._refreshUnreadNotificationsCount$.next();
    }

    private getUnreadNotificationsCount(): Observable<number> {
        return this._apiClientService.get<number>(`${AppConfig.ApiUrl}/notifications/pending/count`);
    }
}
