export interface IToken {
    given_name: string;
    family_name: string;
    jwt: string;
    exp: number;
    role: string[];
    nameid: string;
}