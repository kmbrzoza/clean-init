/* eslint-disable @typescript-eslint/no-unsafe-return */
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

import { Observable, throwError, catchError } from 'rxjs';

@Injectable()
export class ToastHttpInterceptor implements HttpInterceptor {
    constructor(private _toastr: ToastrService) { }

    intercept(req: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
        return next.handle(req).pipe(
            catchError(err => {
                if (err.status === 403) {
                    this._toastr.error('Lack of required credentials.', 'Error');
                } else {
                    this._toastr.error('Problem when processing a request.', 'Error');
                }

                return throwError(() => err);
            })
        );
    }
}
