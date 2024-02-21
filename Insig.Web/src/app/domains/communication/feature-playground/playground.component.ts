import { Component, signal } from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { ToastrService } from 'ngx-toastr';

import { PageHeaderComponent, ValidationFeedbackComponent, SpinnerButtonComponent } from '@domains/shared/ui-common';
import { indicateSignal } from '@domains/shared/util-common';

import { NotificationService, PlaygroundService } from '../data';

const emailPattern = /^[a-zA-Z0-9_.+-]*@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$/;

type UploadedFile = { name: string, filePath: string } | null;

@Component({
    selector: 'app-playground',
    templateUrl: './playground.component.html',
    standalone: true,
    imports: [
        PageHeaderComponent,
        MatButtonModule,
        ReactiveFormsModule,
        MatButtonModule,
        MatFormFieldModule,
        MatInputModule,
        ValidationFeedbackComponent,
        SpinnerButtonComponent
    ]
})
export class PlaygroundComponent {
    email = new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.pattern(emailPattern)] });
    isEmailSending = signal<boolean>(false);
    isNotificationSending = signal<boolean>(false);
    isFileUploading = signal<boolean>(false);
    uploadedFile = signal<UploadedFile>(null);

    constructor(
        private readonly _playgroundService: PlaygroundService,
        private readonly _notificationsService: NotificationService,
        private readonly _toastrService: ToastrService
    ) { }


    onSendNotificationClick(): void {
        this._notificationsService.addNotification()
            .pipe(indicateSignal(this.isNotificationSending))
            .subscribe({
                next: () => {
                    this._toastrService.success('Powiadomienie zostało wysłane');
                }
            });
    }

    onSendEmailClick(): void {
        if (this.email.valid) {
            this._playgroundService.sendEmail(this.email.value)
                .pipe(indicateSignal(this.isEmailSending))
                .subscribe({
                    next: () => {
                        this._toastrService.success('E-mail został wysłany');
                    }
                })
        } else {
            this.email.markAsTouched();
        }
    }

    onFileChange(event: Event): void {
        this.uploadedFile.set(null);

        const { files } = event.target as HTMLInputElement;

        if (files && files.length > 0) {
            const file = files.item(0) as File;

            this._playgroundService.sendFile(file)
                .pipe(indicateSignal(this.isFileUploading))
                .subscribe({
                    next: ({ filePath }) => {
                        this.uploadedFile.set({ name: file.name, filePath })
                        this._toastrService.success('Plik został wysłany');
                    }
                })
        }
    }
}
