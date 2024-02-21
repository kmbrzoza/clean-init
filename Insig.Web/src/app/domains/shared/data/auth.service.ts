

import { Inject, Injectable, OnDestroy, Signal, signal } from '@angular/core';
import { MSAL_GUARD_CONFIG, MsalBroadcastService, MsalGuardConfiguration, MsalService } from '@azure/msal-angular';
import { AccountInfo, AuthenticationResult, InteractionStatus, RedirectRequest } from '@azure/msal-browser';

import { BehaviorSubject, filter, iif, map, Observable, of, shareReplay, Subject, switchMap, takeUntil, tap } from 'rxjs';

import { UserService } from './user.service';
import { UserRole } from './models/user.model';
import { Roles } from './models/role.model';


const ROLE_KEY = 'role';

@Injectable()
export class AuthService implements OnDestroy {
    account: Signal<AccountInfo>;

    private _authStatus$ = new BehaviorSubject<boolean>(false);
    private _isLoading$ = new BehaviorSubject<boolean>(true);
    private _unsubscribe$ = new Subject<void>();
    private _role$: Observable<Nullable<UserRole>>;
    private _accountSignal = signal<AccountInfo>({} as AccountInfo);

    constructor(
        private _msalService: MsalService,
        private _msalBroadcastService: MsalBroadcastService,
        private _userService: UserService,
        @Inject(MSAL_GUARD_CONFIG) private _msalGuardConfig: MsalGuardConfiguration
    ) {
        this.account = this._accountSignal.asReadonly();
        this._role$ = this._msalBroadcastService.inProgress$.pipe(
            filter((status: InteractionStatus) => status === InteractionStatus.None),
            takeUntil(this._unsubscribe$),
            tap(() => {
                this._isLoading$.next(true);
                this.setAuthenticationStatus();
                this.setAccount();
            }),
            switchMap(_ => this.getRoleAndRegisterInDomain$()),
            tap(() => this._isLoading$.next(false)),
            shareReplay({ refCount: true, bufferSize: 1 })
        );

        this._role$.subscribe();
    }

    get isAuthenticated$(): Observable<boolean> {
        return this._authStatus$.asObservable();
    }

    get isLoading$(): Observable<boolean> {
        return this._isLoading$.asObservable();
    }

    login(): void {
        this._msalService.instance.loginRedirect({
            scopes: AppConfig.Auth.Scopes
        });
    }

    register(): void {
        if (this._msalGuardConfig.authRequest) {
            this._msalService.loginRedirect({
                authority: AppConfig.Auth.SignUp,
                ...this._msalGuardConfig.authRequest
            } as RedirectRequest);
        } else {
            this.login();
        }
    }

    logout(): void {
        this.setUserRoleInSessionStorage(null);
        this._msalService.logoutRedirect();
    }

    hasOneOfRoles$(requiredRoles: Roles[]): Observable<boolean> {
        return this.getRole$().pipe(
            map(userRole => !!userRole && requiredRoles.findIndex(role => role === userRole.roleId) !== -1)
        );
    }

    getAuthorizationToken$(): Observable<Nullable<AuthenticationResult>> {
        if (this._authStatus$.value) {
            return this.getToken$();
        }
        return of(null);
    }

    ngOnDestroy(): void {
        this._unsubscribe$.next();
        this._unsubscribe$.complete();
    }

    private setAuthenticationStatus(): void {
        const isLogged = this._msalService.instance.getAllAccounts().length > 0;
        this._authStatus$.next(isLogged);
    }

    private setAccount(): void {
        const [account] = this._msalService.instance.getAllAccounts();

        this._accountSignal.set(account ?? {});
    }

    private getRoleAndRegisterInDomain$(): Observable<Nullable<UserRole>> {
        if (this._authStatus$.value) {
            return of(this.getRoleFromSessionStorage()).pipe(
                switchMap((role: Nullable<UserRole>) => iif(
                    () => role === null,
                    this._userService.getUserRole().pipe(
                        switchMap((userRole: Nullable<UserRole>) => iif(
                            () => userRole === null,
                            this._userService.registerUser().pipe(
                                switchMap(() => this._userService.getUserRole())
                            ),
                            of(userRole)
                        )),
                        tap((userRole: Nullable<UserRole>) => this.setUserRoleInSessionStorage(userRole))
                    ),
                    of(role)
                ))
            );
        }
        return of(null);
    }

    private getRole$(): Observable<Nullable<UserRole>> {
        if (this._authStatus$.value) {
            return of(this.getRoleFromSessionStorage()).pipe(
                switchMap((userRole: Nullable<UserRole>) => iif(
                    () => userRole === null,
                    this._role$,
                    of(userRole)
                ))
            );
        }
        return of(null);
    }

    private getRoleFromSessionStorage(): Nullable<UserRole> {
        return JSON.parse(sessionStorage.getItem(ROLE_KEY) || 'null') as Nullable<UserRole>;
    }

    private setUserRoleInSessionStorage(value: Nullable<UserRole>): void {
        sessionStorage.setItem(ROLE_KEY, JSON.stringify(value));
    }

    private getToken$(): Observable<AuthenticationResult> {
        return this._msalService.acquireTokenSilent({
            scopes: AppConfig.Auth.Scopes,
            account: this._msalService.instance.getAllAccounts()[0]
        });
    }
}
