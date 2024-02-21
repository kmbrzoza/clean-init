import { SelectionModel } from '@angular/cdk/collections';
import {
    AfterViewInit,
    ChangeDetectorRef,
    Component,
    ContentChild,
    ContentChildren,
    EventEmitter,
    Input,
    OnInit,
    Output,
    QueryList,
    ViewChild
} from '@angular/core';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { MatTable, MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSortModule } from '@angular/material/sort';
import { AsyncPipe } from '@angular/common';

import { BehaviorSubject, Observable, ReplaySubject, take } from 'rxjs';

import { TableOptions, Page } from '@domains/shared/util-common';

import { PageInfo, PaginatorComponent } from '../paginator/paginator.component';
import { TABLE_COLUMN_KEY_DEFINITION, TableActionsComponent } from './table-actions/table-actions.component';
import { TableColumnComponent } from './table-column/table-column.component';
import { TableService } from './table.service';

const SELECTION_COLUMN_KEY = 'selections';

@Component({
    selector: 'app-table',
    templateUrl: './table.component.html',
    styleUrls: ['./table.component.scss'],
    standalone: true,
    imports: [
        MatProgressSpinnerModule,
        PaginatorComponent,
        MatTableModule,
        MatSortModule,
        AsyncPipe
    ],
    providers: [TableService]
})
export class TableComponent<T> implements OnInit, AfterViewInit {
    @Input() pageSize!: number;
    @Input() tableOptions!: TableOptions;
    @Output() rowClicked = new EventEmitter<T>();
    @Output() pageChanged = new EventEmitter<number>();
    @ContentChildren(TableColumnComponent) tableColumns!: QueryList<TableColumnComponent<T>>;
    @ContentChild(TableActionsComponent) tableActions!: TableActionsComponent<T>;
    @ViewChild('selecionsColumn') selectionsColumn!: TableColumnComponent<T>;

    columns$ = new BehaviorSubject<string[]>([]);
    data$!: Observable<Page<T>>;
    isLoading$ = this._tableService.isLoading$;
    isAllSelected$ = this._tableService.isAllSelected$;
    selections!: SelectionModel<number>;
    selecionsColumnKey = SELECTION_COLUMN_KEY;

    private _table$ = new ReplaySubject<MatTable<T>>();

    constructor(
        private _tableService: TableService<T>,
        private _cdr: ChangeDetectorRef
    ) { }

    @ViewChild('table') set table(content: MatTable<T>) {
        if (content) {
            this._table$.next(content);
        }
    }

    ngOnInit(): void {
        this.data$ = this._tableService.getData();
        this.selections = this._tableService.selections;
    }

    ngAfterViewInit(): void {
        this._table$
            .pipe(
                take(1)
            )
            .subscribe(table => {
                this.setColumns(table);
                this.setActions(table);
                this._cdr.detectChanges();
            });
    }

    onRowClick(row: T): void {
        this.rowClicked.emit(row);
    }

    onPageChange(pageInfo: PageInfo): void {
        this._tableService.nextPage(pageInfo);
    }

    toggleAllRows(event: MatCheckboxChange) {
        if (event.checked) {
            this._tableService.selectAll();
        } else {
            this._tableService.clearSelections();
        }
    }

    private setColumns(table: MatTable<T>): void {
        for (const column of this.tableColumns) {
            table.addColumnDef(column.columnDef);
        }

        let columns = this.tableColumns.map(t => t.column.key);

        if (this.tableOptions?.showSelections) {
            table.addColumnDef(this.selectionsColumn.columnDef);
            columns = [this.selectionsColumn.column.key, ...columns];
        }

        this.columns$.next(columns);
    }

    private setActions(table: MatTable<T>): void {
        if (this.tableActions) {
            table.addColumnDef(this.tableActions.columnDef);
            this.columns$.next([
                ...this.columns$.getValue(),
                TABLE_COLUMN_KEY_DEFINITION
            ]);
        }
    }
}
