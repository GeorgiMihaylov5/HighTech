export interface AuthUser {
    given_name: string;
    family_name: string;
    jwt: string;
    exp: number;
    role: string;
    email: string;
}