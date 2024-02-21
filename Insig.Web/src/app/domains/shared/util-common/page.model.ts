export interface Page<T> {
    totalCount: number;
    pageNumber: number;
    pageSize: number;
    items: Array<T>;
}

export interface PageInfo {
    page: number;
    pageSize: number;
}
