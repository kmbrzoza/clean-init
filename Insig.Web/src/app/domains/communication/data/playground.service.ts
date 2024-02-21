import { Injectable } from '@angular/core';

import { Observable } from 'rxjs';

import { ApiClientService } from '@domains/shared/util-api';


@Injectable({
    providedIn: 'root'
})
export class PlaygroundService {
    constructor(private readonly _apiClientService: ApiClientService) { }

    sendEmail(email: string): Observable<void> {
        return this._apiClientService.post(`${AppConfig.ApiUrl}/e-mails`, { data: { email } });
    }

    sendFile(file: File): Observable<{ filePath: string }> {
        const data = new FormData();

        data.append('fileToAdd', file);

        return this._apiClientService.post(`${AppConfig.ApiUrl}/files`, { data });
    }
}
