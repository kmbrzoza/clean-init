import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import URLParse from 'url-parse';

import { catchError, EMPTY, Observable, switchMap, throwError } from 'rxjs';

import { AuthService } from '../auth.service';

@Injectable()
export class HttpAuthInterceptor implements HttpInterceptor {
    allowedUrls = [
        AppConfig.ClientUrl,
        AppConfig.ApiUrl,
        AppConfig.Auth.Authority
    ];

    constructor(
        private _authService: AuthService,
        private _router: Router
    ) { }
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        if (!this.checkUrl(req.url.toLowerCase())) {
            return next.handle(req);
        }

        return this._authService.getAuthorizationToken$()
            .pipe(
                switchMap(authResult => {
                    if (authResult) {
                        const newReq = req.clone({ setHeaders: { authorization: `${authResult.tokenType} ${authResult.accessToken}` } });
                        return next.handle(newReq);
                    } else {
                        return next.handle(req);
                    }
                }),
                catchError(err => {
                    if (err instanceof HttpErrorResponse) {
                        switch (err.status) {
                            case 401:
                                return this.handle401Error(err);
                            case 404:
                                return this.handle404Error(err);
                            default:
                                return EMPTY;
                        }
                    }
                    return throwError(() => err as unknown);
                })
            );
    }

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    private handle401Error(_: HttpErrorResponse): Observable<HttpEvent<any>> {
        this._router.navigateByUrl('access-denied');
        return EMPTY;
    }

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    private handle404Error(_: HttpErrorResponse): Observable<HttpEvent<any>> {
        this._router.navigateByUrl('not-found');
        return EMPTY;
    }

    private checkUrl(callUrl: string): boolean {
        return this.allowedUrls.some(url => this.isFromService(url, callUrl));
    }

    private isFromService(url: string, callUrl: string): boolean {
        if (!callUrl.startsWith(url)) {
            return false;
        }

        const serviceUrlBase = new URLParse(url);
        const callUrlBase = new URLParse(callUrl);

        return serviceUrlBase.hostname === callUrlBase.hostname
            && serviceUrlBase.protocol === callUrlBase.protocol;
    }
}
