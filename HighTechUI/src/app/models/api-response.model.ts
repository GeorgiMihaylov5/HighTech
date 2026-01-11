/**
 * Represents a standardized API response from the backend
 */
export interface ApiResponse<T = any> {
	statusCode: number;
	data: T;
}
