import { NotificationStatus } from './notification-status.enum';

export interface Notification {
    id: number;
    statusId: NotificationStatus;
    title: string;
    body: string;
    redirectUrl: string;
    createdOn: string;
}
