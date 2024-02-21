import { DatePipe } from '@angular/common';
import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import {
    PageHeaderComponent,
    TableActionsComponent,
    TableColumnComponent,
    TableComponent
} from '@domains/shared/ui-common';
import { MessageService } from '@domains/shared/data';
import { TABLE_TOKEN } from '@domains/shared/util-common';

import { Notification, NotificationService, NotificationStatus } from '../data';

@Component({
    selector: 'app-notifications',
    templateUrl: './notifications.component.html',
    standalone: true,
    imports: [
        TableComponent,
        TableColumnComponent,
        TableActionsComponent,
        DatePipe,
        PageHeaderComponent,
        MatButtonModule,
        MatIconModule
    ],
    providers: [
        {
            provide: TABLE_TOKEN,
            useClass: NotificationService
        }
    ]
})
export class NotificationsComponent {
    notificationStatus = NotificationStatus;

    constructor(
        private readonly _notificationService: NotificationService,
        private readonly _messageService: MessageService
    ) { }

    onPreviewClick(notification: Notification): void {
        this._notificationService.readNotification(notification.id)
            .subscribe(() => {
                notification.statusId = NotificationStatus.Completed;

                this._messageService.refereshNotifications();
            });
    }
}
