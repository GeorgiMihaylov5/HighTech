import { AppUser } from "src/api-authorization/models/user.model";
import { Product } from "./product.model";

export interface OrderedProduct {
    id: string;
    productId: string;
    product: Product;
    orderedPrice: number;
    count: number;
  }

  export interface Order {
    id: string;
    orderedOn: string;
    user: AppUser;
    status: string;
    notes: string;
    orderedProducts: OrderedProduct[];
  }