export interface IIAuthorizationRequest {
  code: string;
  state: string;
  clientName: string;
  clientDescription: string;
  username: string;
  scopes: IAuthorizationScope[];
  redirectUri: string;
}

export interface IAuthorizationScope {
  name: string;
  description: string;
}
