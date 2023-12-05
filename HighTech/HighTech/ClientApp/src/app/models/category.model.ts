import { Field } from "./field.model";

export interface ICategory {
    categoryId: string;
    fields: Field[]
}