export interface Review {
    id: string;
    productId: string;
    clientId: string;
    username: string;
    firstName: string;
    lastName: string;
    rating: number;
    comment: string;
    createdOn: string;
    updatedOn: string;
}

export interface UpsertReview {
    productId: string;
    username: string;
    rating: number;
    comment: string;
}
