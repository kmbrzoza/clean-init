export type SearchCriteria<T> = PaginationCriteria & GenericSearch<T>;

export type PaginationCriteria = {
    pageNumber: number;
    pageSize: number;
    orderBy?: string;
};

type GenericSearch<T> = {
    [K in keyof T]: T[K]
};
