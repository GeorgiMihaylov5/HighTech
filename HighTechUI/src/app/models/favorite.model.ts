import { Product } from "./product.model";

export interface Favorite {
    id: string;
    productId: string;
    userId: string;
    createdOn: string;
    product: Product;
}

export interface FavoriteAction {
    productId: string;
    username: string;
}
