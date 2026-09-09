export interface ApiResult<T> {
    isSuccess: boolean;
    value: T | null;
    error: string | null;
    errorTitle: string | null;
}
