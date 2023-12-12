import { Field } from "./field.model";

export interface Category {
    id: string;
    name: string;
    fields: Field[]
}