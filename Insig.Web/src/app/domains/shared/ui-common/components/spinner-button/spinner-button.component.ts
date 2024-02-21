import { NgIf } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
    selector: 'app-spinner-button',
    standalone: true,
    imports: [
        MatProgressSpinnerModule,
        NgIf,
        MatButtonModule
    ],
    templateUrl: './spinner-button.component.html'
})
export class SpinnerButtonComponent {
    @Input() loading: Nullable<boolean> = false;
    @Output() clicked = new EventEmitter<MouseEvent>();

    onClick(event: MouseEvent): void {
        if (!this.loading) {
            this.clicked.emit(event);
        }
    }
}
