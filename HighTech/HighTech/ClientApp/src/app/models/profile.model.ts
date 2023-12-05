import { IClient } from "./client.model";
import { IEmployee } from "./employee.model";

export interface IProfile {
    value: IClient | IEmployee;
    originalValue: IClient | IEmployee;
}