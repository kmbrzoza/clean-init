import { Component, Input, TemplateRef, ViewChild } from '@angular/core';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatColumnDef, MatTableModule } from '@angular/material/table';
import { NgTemplateOutlet } from '@angular/common';

import { TableService } from '../table.service';

/* eslint-disable @typescript-eslint/no-explicit-any */
type TableColumns = {
    key: string;
    title?: string;
    templateRef?: TemplateRef<any>;
    width?: number;
    headerTemplateRef?: TemplateRef<any>;
    sortDisabled?: boolean;
};
@Component({
    selector: 'app-table-column',
    templateUrl: './table-column.component.html',
    styleUrls: ['./table-column.component.scss'],
    standalone: true,
    imports: [
        MatTableModule,
        MatSortModule,
        NgTemplateOutlet
    ]
})
export class TableColumnComponent<T> {
    @Input() column!: TableColumns;
    @ViewChild(MatColumnDef) columnDef!: MatColumnDef;

    constructor(private readonly _tableService: TableService<T>) { }

    onSortChange(sort: Sort): void {
        const orderBy = sort.direction ? `${sort.active},${sort.direction}` : '';

        this._tableService.setOrderBy(orderBy);
    }
}
