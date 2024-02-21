import { NgIf } from '@angular/common';
import { ChangeDetectorRef, DestroyRef, Directive, inject, Input, TemplateRef, ViewContainerRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { iif, Observable, of, switchMap } from 'rxjs';

import { AuthService, Roles } from '@domains/shared/data';


@Directive({
    selector: '[appRequiredRoles]',
    standalone: true
})
export class RequiredRolesDirective extends NgIf {
    private _destroyRef = inject(DestroyRef);

    constructor(
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        templateRef: TemplateRef<any>,
        viewContainerRef: ViewContainerRef,
        private _authService: AuthService,
        private _cdr: ChangeDetectorRef
    ) {
        super(viewContainerRef, templateRef);
    }

    @Input() set appRequiredRoles(roles: Roles[]) {
        this.ngIf = false;
        if (!roles) {
            this.ngIf = false;
        } else {
            this.checkIfHasOneOfRole(roles)
                .pipe(
                    takeUntilDestroyed(this._destroyRef)
                ).subscribe((isAllowed: boolean) => {
                    this.ngIf = isAllowed;
                    this._cdr.markForCheck();
                });
        }
    }

    private checkIfHasOneOfRole(roles: Roles[]): Observable<boolean> {
        return this._authService.isAuthenticated$.pipe(
            switchMap(isAuth => iif(
                () => isAuth === true,
                this._authService.hasOneOfRoles$(roles),
                of(false)
            ))
        );
    }
}

