import { Injectable } from '@angular/core';

import { Observable } from 'rxjs';

import { ApiClientService } from '@domains/shared/util-api';
import { Table, PaginationCriteria, Page } from '@domains/shared/util-common';

import { Notification } from './notification.model';

@Injectable({
    providedIn: 'root'
})
export class NotificationService implements Table<Notification, null> {
    constructor(private readonly _apiClientService: ApiClientService) { }

    getData(paginationCriteria: PaginationCriteria): Observable<Page<Notification>> {
        return this._apiClientService.get(`${AppConfig.ApiUrl}/notifications`, { queryParams: paginationCriteria });
    }

    addNotification(): Observable<void> {
        return this._apiClientService.post(`${AppConfig.ApiUrl}/notifications`, { data: {} });
    }

    readNotification(id: number): Observable<void> {
        return this._apiClientService.patch(
            `${AppConfig.ApiUrl}/notifications/{id}/completed`,
            { segmentParams: { id } }
        );
    }
}
