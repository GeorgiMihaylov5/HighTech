import { Client } from "./client.model";
import { IEmployee } from "./employee.model";

export interface IProfile {
    value: Client | IEmployee;
    originalValue: Client | IEmployee;
}