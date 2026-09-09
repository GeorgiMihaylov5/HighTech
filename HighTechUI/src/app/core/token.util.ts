import { IToken } from 'src/api-authorization/models/token.model';

export function getStoredToken(): IToken | null {
    const accessToken = localStorage.getItem('token');
    if (!accessToken) return null;
    try {
        return JSON.parse(atob(accessToken.split('.')[1])) as IToken;
    } catch {
        return null;
    }
}
