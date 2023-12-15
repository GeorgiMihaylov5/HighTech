export interface IToken {
    given_name: string;
    family_name: string;
    jwt: string;
    exp: number;
    role: string[] | string;
    nameid: string;
}