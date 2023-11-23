export interface AuthUser {
    firstName: string;
    lastName: string;
    jwt: string;
    exp: number;
    role: string[];
    email: string;
}