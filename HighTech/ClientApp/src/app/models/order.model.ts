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
  username: string;
  status: Status;
  notes: string;
  orderedProducts: OrderedProduct[];
}

export enum Status {
  Approved = 0,
  Pending = 1,
  Rejected = 2,
  Completed = 3
}