import { ReturnUrlType } from "../api-authorization.constants";

export interface INavigationState {
    [ReturnUrlType]: string;
}

export type IAuthenticationResult =
    SuccessAuthenticationResult |
    FailureAuthenticationResult |
    RedirectAuthenticationResult;

export interface SuccessAuthenticationResult {
    status: AuthenticationResultStatus.Success;
    state: any;
}

export interface FailureAuthenticationResult {
    status: AuthenticationResultStatus.Fail;
    message: string;
}

export interface RedirectAuthenticationResult {
    status: AuthenticationResultStatus.Redirect;
}

export enum AuthenticationResultStatus {
    Success,
    Redirect,
    Fail
}

export interface IUser {
    name?: string;
}
