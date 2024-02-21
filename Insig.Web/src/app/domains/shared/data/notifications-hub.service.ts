import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, IHttpConnectionOptions, LogLevel } from '@microsoft/signalr';

import { lastValueFrom, map } from 'rxjs';

import { MessageService } from './message.service';
import { NotificationMessage } from './models/message.model';
import { AuthService } from './auth.service';

@Injectable({
    providedIn: 'root'
})
export class NotificationsHubService {
    notificationsHubConnection: Nullable<HubConnection>;
    connectionFailureCount!: number;

    constructor(
        private _authService: AuthService,
        private _messageService: MessageService
    ) {
        this._authService.isAuthenticated$.subscribe(status => {
            if (status) {
                this.startConnection();
            } else {
                this.stopConnection();
            }
        });
    }

    init(): void {
        this.connectionFailureCount = 0;
    }

    private startConnection(): void {
        this.notificationsHubConnection = new HubConnectionBuilder()
            .configureLogging(LogLevel.None)
            .withUrl(`${AppConfig.ApiUrl}/hubs/notifications`, {
                accessTokenFactory: () => lastValueFrom(
                    this._authService.getAuthorizationToken$().pipe(
                        map(result => result?.accessToken)
                    )
                )
            } as IHttpConnectionOptions)
            .build();

        this.notificationsHubConnection.start().then(() => {
            this.connectionFailureCount = 0;
        }).catch((_err: Error) => {
            setTimeout(() => {
                this.connectionFailureCount++;
                if (this.connectionFailureCount === 10) {
                    return;
                }

                this.notificationsHubConnection = null;
                this.startConnection();
            }, 3000);
        });

        this.notificationsHubConnection.on('UserNotification', (_message: NotificationMessage) => {
            this._messageService.refereshNotifications();
        });
    }

    private stopConnection(): void {
        if (this.notificationsHubConnection) {
            this.notificationsHubConnection.stop();
            this.notificationsHubConnection = null;
        }
    }
}
