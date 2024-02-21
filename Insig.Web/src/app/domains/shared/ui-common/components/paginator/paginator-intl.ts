import { Injectable } from '@angular/core';
import { MatPaginatorIntl } from '@angular/material/paginator';

import { Subject } from 'rxjs';

@Injectable()
export class PaginatorIntl implements MatPaginatorIntl {
    nextPageLabel!: string;
    previousPageLabel!: string;
    firstPageLabel!: string;
    lastPageLabel!: string;
    changes = new Subject<void>();
    itemsPerPageLabel = 'Ilość na stronę:';

    getRangeLabel(page: number, pageSize: number, length: number): string {
        if (length === 0) {
            return '1 z 1';
        }
        const amountPages = Math.ceil(length / pageSize);

        return `${page + 1} z ${amountPages}`;
    }
}
