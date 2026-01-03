export interface CurrentUserDto {
  userId: number;
  email: string;
  isAdmin: boolean;
  IsOrganiser: boolean;
  IsUser: boolean;
  tokenVersion: number;
}
