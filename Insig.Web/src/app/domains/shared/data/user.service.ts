import { Injectable } from '@angular/core';

import { Observable } from 'rxjs';

import { UserRole } from './models/user.model';
import { ApiClientService } from '../util-api';


@Injectable({
    providedIn: 'root'
})
export class UserService {
    constructor(private _apiClientService: ApiClientService) { }

    registerUser(): Observable<void> {
        return this._apiClientService.post(`${AppConfig.ApiUrl}/users`, { data: {} });
    }

    getUserRole(): Observable<Nullable<UserRole>> {
        return this._apiClientService.get(`${AppConfig.ApiUrl}/users/role`);
    }
}
