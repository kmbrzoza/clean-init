import { SelectionModel } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, Output, TemplateRef, ViewChild } from '@angular/core';
import { MatColumnDef, MatTableModule } from '@angular/material/table';
import { ActivatedRoute, Router } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { AsyncPipe, NgTemplateOutlet } from '@angular/common';

import { Observable } from 'rxjs';

import { TableService } from '../table.service';

/* eslint-disable @typescript-eslint/no-explicit-any */
export const TABLE_COLUMN_KEY_DEFINITION = 'actions';

@Component({
    selector: 'app-table-actions',
    templateUrl: './table-actions.component.html',
    styleUrls: ['./table-actions.component.scss'],
    standalone: true,
    imports: [
        MatTableModule,
        MatIconModule,
        MatButtonModule,
        NgTemplateOutlet,
        AsyncPipe
    ]
})
export class TableActionsComponent<T> {
    @Input() editable = true;
    @Input() deletable = false;
    @Input() previewVisible = false;
    @Input() actionTmpl!: TemplateRef<any>;
    @Input() selectionMenuActionsTmpl!: TemplateRef<any>;
    @Input() identifierFieldName = 'id';
    @Input() defaultEditEvent = true;
    @Output() deleted = new EventEmitter<T>();
    @Output() edited = new EventEmitter<T>();
    @Output() previewClicked = new EventEmitter<T>();
    @ViewChild(MatColumnDef) columnDef!: MatColumnDef;

    columnKey = TABLE_COLUMN_KEY_DEFINITION;
    selections: SelectionModel<number>;
    isAllSelected$: Observable<boolean>;

    constructor(
        private _router: Router,
        private _activatedRoute: ActivatedRoute,
        private _tableService: TableService<T>
    ) {
        this.selections = this._tableService.selections;
        this.isAllSelected$ = this._tableService.isAllSelected$;
    }

    edit(element: any): void {
        if (this.defaultEditEvent) {
            this._router.navigate(['edit', element[this.identifierFieldName]], { relativeTo: this._activatedRoute });
        } else {
            this.edited.emit(element as T);
        }
    }

    delete(element: T): void {
        this.deleted.emit(element);
    }

    onPreviewClicked(element: T): void {
        this.previewClicked.emit(element);
    }
}
