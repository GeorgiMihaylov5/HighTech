import { Field } from "./field.model";

export interface Product {
    id: string;
    manufacturer: string;
    model: string;
    warranty: number;
    price: number;
    discount: number;
    quantity: number;
    image: string;
    categoryName: string;
    fields: Field[];
}