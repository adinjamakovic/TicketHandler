// Payload kako dolazi iz IdentityServer access tokena.
// `is_*` i `email` claimovi dolaze iz ApiResource UserClaims (Market.IdentityServer/Config.cs),
// pa žive u access tokenu – ne u id_tokenu.
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
