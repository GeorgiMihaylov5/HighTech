import { AppUser } from "src/api-authorization/models/user.model";
import { Product } from "./product.model";

export interface OrderedProduct {
  id: string;
  productId: string;
  product: Product;
  orderedPrice: number;
  count: number;
  buildId?: string;
}

export interface Order {
  id: string;
  orderedOn: string;
  user: AppUser;
  username: string;
  status: Status;
  notes: string;
  city: string;
  postalCode: string;
  deliveryAddress: string;
  phoneNumber: string;
  paymentMethod: PaymentMethod;
  orderedProducts: OrderedProduct[];
}

export enum PaymentMethod {
  OnDelivery = 0
}

export enum Status {
  Pending = 0,
  Confirmed = 1,
  Preparing = 2,
  Shipped = 3,
  Completed = 4,
  Cancelled = 5,
  Rejected = 6
}
