import { SelectionModel } from '@angular/cdk/collections';
import { Inject, Injectable, OnDestroy, Optional } from '@angular/core';

import { BehaviorSubject, Observable, skip, Subject, switchMap, takeUntil } from 'rxjs';

import { indicate, Page, PaginationCriteria, Table, TABLE_TOKEN } from '@domains/shared/util-common';

import { PageInfo } from '../paginator/paginator.component';


@Injectable()
export class TableService<T, K = undefined> implements OnDestroy {
    paginationCriteria$ = new BehaviorSubject<PaginationCriteria>({
        pageNumber: 1,
        pageSize: 10
    });
    isLoading$: Observable<boolean>;
    isAllSelected$: Observable<boolean>;
    readonly selections = new SelectionModel<number>(true, []);

    private readonly _isAllSelected$ = new BehaviorSubject<boolean>(false);
    private readonly _destroyed$ = new Subject<void>();
    private readonly _isLoadingSubject$ = new BehaviorSubject<boolean>(true);

    constructor(@Inject(TABLE_TOKEN) @Optional() private readonly _table: Table<T, K>) {
        if (!this._table) {
            throw new Error('Did you forget to provide TABLE_TOKEN ?');
        }

        this._table.searchCriteria$
            ?.pipe(
                skip(1),
                takeUntil(this._destroyed$)
            )?.subscribe(() => {
                this.nextPage({
                    page: this.paginationCriteria$.value.pageNumber,
                    pageSize: this.paginationCriteria$.value.pageSize
                });
            });

        this._table.clearSelections$
            ?.pipe(
                takeUntil(this._destroyed$)
            )?.subscribe(() => {
                this.clearSelections();
            });

        this.isLoading$ = this._isLoadingSubject$.asObservable();
        this.isAllSelected$ = this._isAllSelected$.asObservable();
    }

    getData(): Observable<Page<T>> {
        return this.paginationCriteria$.pipe(
            switchMap(
                paginationCriteria => this._table.getData(paginationCriteria)
                    .pipe(indicate(this._isLoadingSubject$))
            )
        );
    }

    nextPage(pageInfo: PageInfo): void {
        this.paginationCriteria$.next({
            ...this.paginationCriteria$.getValue(),
            pageSize: pageInfo.pageSize,
            pageNumber: pageInfo.page
        });
    }

    setOrderBy(orderBy: string): void {
        this.paginationCriteria$.next({
            ...this.paginationCriteria$.getValue(),
            orderBy
        });
    }

    refresh(): void {
        this.paginationCriteria$.next(this.paginationCriteria$.getValue());
    }

    selectAll(): void {
        this._isAllSelected$.next(true);
        this.selections.clear();
    }

    clearSelections(): void {
        this._isAllSelected$.next(false);
        this.selections.clear();
    }

    ngOnDestroy(): void {
        this._destroyed$.next();
        this._destroyed$.complete();
    }
}
