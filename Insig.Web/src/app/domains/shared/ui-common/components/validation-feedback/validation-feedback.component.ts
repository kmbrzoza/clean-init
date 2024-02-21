import { Component, computed, Input, input, OnInit, Optional, signal } from '@angular/core';
import { AbstractControl, FormGroupDirective } from '@angular/forms';

@Component({
    selector: 'app-validation-feedback',
    templateUrl: './validation-feedback.component.html',
    standalone: true,
    imports: []
})
export class ValidationFeedbackComponent implements OnInit {
    control = signal<Nullable<AbstractControl>>(null);
    controlName = input<Nullable<string>>();
    error = computed(() => this.getValidationError(this.control()));

    // eslint-disable-next-line @angular-eslint/no-input-rename
    @Input({ alias: 'control' }) set _control(control: Nullable<AbstractControl>) {
        this.control.set(control);
    }

    constructor(@Optional() private _formGroup: FormGroupDirective) { }

    getValidationError(control: Nullable<AbstractControl>): string {
        if (control?.hasError('required')) {
            return 'Pole jest wymagane.';
        } else if (control?.hasError('minlength')) {
            return `Pole musi mieć minimum ${control.getError('minlength').requiredLength} znaków.`;
        } else if (control?.hasError('email')) {
            return 'Format adresu e-mail jest niepoprawny.';
        } else if (
            control?.hasError('matStartDateInvalid') ||
            control?.hasError('matEndDateInvalid') ||
            control?.hasError('dateRangeInvalid')
        ) {
            return 'Nieprawidłowy zakres dat.';
        } else if (control?.hasError('pattern')) {
            return 'Nieprawidłowy format';
        }

        return '';
    }

    ngOnInit(): void {
        if (!this.control && !this.controlName) {
            throw new Error('Validation Feedback must have [control] or [controlName] inputs');
        } else if (this.controlName() && this._formGroup) {
            this.control = signal(this._formGroup.form.get(this.controlName() as string));
        }
    }
}
