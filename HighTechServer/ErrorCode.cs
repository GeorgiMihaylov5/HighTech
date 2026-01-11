namespace HighTechServer
{
	public enum ErrorCode
	{
		// General Errors
		UnknownError = 0,
		InvalidRequest = 1,
		UnauthorizedAccess = 2,
		NotFound = 3,

		// Category Errors (100-199)
		CategoryIdMissing = 100,
		CategoryNameMissing = 101,
		CategoryNotFound = 102,
		CategoryAlreadyExists = 103,
		CategoryCreateError = 104,
		CategoryUpdateError = 105,
		CategoryDeleteError = 106,

		// Product Errors (200-299)
		ProductIdMissing = 200,
		ProductNotFound = 201,
		ProductCategoryIdMissing = 202,
		ProductCreateError = 203,
		ProductUpdateError = 204,
		ProductDeleteError = 205,
		ProductDiscountError = 206,

		// Field Errors (300-399)
		FieldIdMissing = 300,
		FieldNameMissing = 301,
		FieldNotFound = 302,
		FieldCreateError = 303,
		FieldUpdateError = 304,
		FieldDeleteError = 305,

		// Order Errors (400-499)
		OrderIdMissing = 400,
		OrderNotFound = 401,
		OrderCreateError = 402,
		OrderUpdateError = 403,
		OrderUsernameMissing = 404,

		// Client Errors (500-599)
		ClientIdMissing = 500,
		ClientNotFound = 501,
		ClientUsernameMissing = 502,
		ClientCreateError = 503,
		ClientUpdateError = 504,
		InvalidCredentials = 505,
		EmailAlreadyExists = 506,
		PasswordMismatch = 507,
		PasswordChangeError = 508,

		// Employee Errors (600-699)
		EmployeeIdMissing = 600,
		EmployeeNotFound = 601,
		EmployeeUsernameMissing = 602,
		EmployeeAlreadyExists = 603,
		EmployeeCreateError = 604,
		EmployeeUpdateError = 605,
		EmployeePromotionError = 606,
		EmployeeDemotionError = 607,

		// Review Errors (700-799)
		ReviewIdMissing = 700,
		ReviewNotFound = 701,
		ReviewProductIdMissing = 702,
		ReviewRatingInvalid = 703,
		ReviewCreateError = 704,
		ReviewUpdateError = 705,
		ReviewDeleteError = 706,
		ReviewAlreadyExists = 707,

		// Favorite Errors (800-899)
		FavoriteProductIdMissing = 800,
		FavoriteNotFound = 801,
		FavoriteCreateError = 802,
		FavoriteDeleteError = 803,
		FavoriteAlreadyExists = 804,

		// Save/Delete Generic Errors (900-999)
		SaveError = 900,
		DeleteError = 901,
		DatabaseError = 902
	}
}
