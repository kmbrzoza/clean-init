import { inject } from '@angular/core';
import { CanActivateFn, ActivatedRouteSnapshot, UrlTree, Router } from '@angular/router';

import { Observable, combineLatest, tap, map, skipWhile, switchMap } from 'rxjs';

import { AuthService } from '../auth.service';
import { Roles } from '../models/role.model';

export const CanActivateRoleGuard: CanActivateFn = (
    route: ActivatedRouteSnapshot
): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree => {
    const router: Router = inject(Router);
    const authService: AuthService = inject(AuthService);
    const roles = Object.values(route.data) as Roles[];

    return authService.isLoading$.pipe(
        skipWhile(isLoading => isLoading === true),
        switchMap(_ => combineLatest({
            hasAccess: authService.hasOneOfRoles$(roles),
            isLogedIn: authService.isAuthenticated$
        })),
        tap(result => {
            if (!result.hasAccess || !result.isLogedIn) {
                router.navigate(['access-denied']);
            }
        }),
        map(result => result.hasAccess)
    );
};
