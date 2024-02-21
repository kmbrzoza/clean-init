export type Cache = Record<number, CacheData>;

export type CacheData<T extends CacheDataFilters = CacheDataFilters> = {
    filters: T;
};

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export type CacheDataFilters = { [key: string]: any };
