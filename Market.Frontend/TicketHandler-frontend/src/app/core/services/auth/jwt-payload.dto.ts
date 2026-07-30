// Payload as it comes from the IdentityServer access token.
// The `is_*` and `email` claims come from ApiResource UserClaims (Market.IdentityServer/Config.cs),
// so they live in the access token - not in the id_token.
export interface JwtPayloadDto {
  sub: string;
  email?: string;
  name?: string;
  is_admin?: string;
  is_organiser?: string;
  is_user?: string;
  iat: number;
  exp: number;
  aud: string | string[];
  iss: string;
}
