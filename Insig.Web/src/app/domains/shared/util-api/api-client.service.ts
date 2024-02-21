
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import URI from 'urijs';
import URITemplate from 'urijs/src/URITemplate';

import { Observable } from 'rxjs';

/* eslint-disable*/
interface DataParam { [param: string]: any }
interface DataBody { [param: string]: any }
/* eslint-disable*/

interface QueryParameters {
    segmentParams?: DataParam;
    queryParams?: DataParam;
}

interface CommandParameters {
    segmentParams?: DataParam;
    data?: DataBody;
}

interface DeleteParameters {
    segmentParams?: DataParam;
}

@Injectable({
    providedIn: 'root'
})
export class ApiClientService {
    constructor(private readonly _http: HttpClient) { }

    get<T>(
        uriTemplate: string,
        queryParameters?: QueryParameters
    ): Observable<T> {
        const url: string = this.buildUrl(uriTemplate, queryParameters?.segmentParams, queryParameters?.queryParams);
        return this._http.get<T>(url);
    }

    getBlob(
        uriTemplate: string,
        queryParameters?: QueryParameters
    ): Observable<Blob> {
        const url: string = this.buildUrl(uriTemplate, queryParameters?.segmentParams, queryParameters?.queryParams);
        return this._http.get(url, { responseType: 'blob' });
    }

    post<T>(
        uriTemplate: string,
        commandParameters?: CommandParameters
    ): Observable<T> {
        const url: string = this.buildUrl(uriTemplate, commandParameters?.segmentParams);
        return this._http.post<T>(url, commandParameters?.data);
    }

    postBlob(
        uriTemplate: string,
        commandParameters?: CommandParameters
    ): Observable<Blob> {
        const url: string = this.buildUrl(uriTemplate, commandParameters?.segmentParams);
        return this._http.post(url, commandParameters?.data, { responseType: 'blob' });
    }

    put<T>(
        uriTemplate: string,
        commandParameters?: CommandParameters
    ): Observable<T> {
        const url: string = this.buildUrl(uriTemplate, commandParameters?.segmentParams);
        return this._http.put<T>(url, commandParameters?.data);
    }

    patch<T>(
        uriTemplate: string,
        commandParameters?: CommandParameters
    ): Observable<T> {
        const url: string = this.buildUrl(uriTemplate, commandParameters?.segmentParams);
        return this._http.patch<T>(url, commandParameters?.data);
    }

    delete<T>(
        uriTemplate: string,
        deleteParameters?: DeleteParameters
    ): Observable<T> {
        const url: string = this.buildUrl(uriTemplate, deleteParameters?.segmentParams);
        return this._http.delete<T>(url);
    }

    private buildUrl(
        uriTemplate: string,
        segmentParams?: DataParam,
        queryParams?: DataParam
    ): string {
        this.parseDateValues(segmentParams);
        this.parseDateValues(queryParams);

        let uri

        if (segmentParams) {
            uri = new URITemplate(uriTemplate).expand(segmentParams)
        } else {
            uri = URI(uriTemplate);
        }

        if (queryParams) {
            uri = uri.query(queryParams);
        }

        return uri.toString();
    }

    private parseDateValues(params: any): void {
        if (params as unknown) {
            for (const prop in params) {
                if (params[prop]?.constructor === Date) {
                    params[prop] = (params[prop] as Date).toISOString();
                }
            }
        }
    }
}
