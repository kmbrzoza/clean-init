import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { MatPaginator, MatPaginatorIntl, MatPaginatorModule, PageEvent } from '@angular/material/paginator';

import { Page } from '@domains/shared/util-common';

import { PaginatorIntl } from './paginator-intl';

export interface PageInfo {
    page: number;
    pageSize: number;
}

@Component({
    selector: 'app-paginator',
    templateUrl: './paginator.component.html',
    styleUrls: ['./paginator.component.scss'],
    standalone: true,
    imports: [
        MatPaginatorModule
    ],
    providers: [{ provide: MatPaginatorIntl, useClass: PaginatorIntl }]
})
export class PaginatorComponent<T> {
    @ViewChild('paginator') matPaginator!: MatPaginator;

    @Input() pageSize = 10;
    @Input() pageSizeOptions = [10, 20, 50, 100];
    @Input() set data(value: Page<T>) {
        this.pagedData = value;
        this.totalPages = Math.ceil(this.pagedData.totalCount / this.pagedData.pageSize);
    }

    @Output() pageChanged = new EventEmitter<PageInfo>();
    pagedData!: Page<T>;
    totalPages!: number;


    onPageChanged(pageEvent: PageEvent): void {
        this.pageChanged.emit({
            page: pageEvent.pageIndex + 1,
            pageSize: pageEvent.pageSize
        });
    }
}
