import { Field } from "./field.model";

export interface Product {
    Id: string;
    Manufacturer: string;
    Model: string;
    Warranty: number;
    Price: number;
    Discount: number;
    Quantity: number;
    Image: string;
    CategoryName: string;
    Fields: Field[];
}