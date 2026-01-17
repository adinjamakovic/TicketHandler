
/**
 * Response for GET /Person/{id}
 * Corresponds to: GetPersonByIdQueryDto.cs
 */
export interface GetPersonByIdQueryDto {
  firstName:string;
  lastName:string;
  birthDate:string;
  cityId:number;
  address:string;
  gender:string;
  phone:string;
}

// ================================================
// ================= COMMANDS =====================
// ================================================

export interface CreatePersonCommand {
  firstName: string;
  lastName: string;
  birthDate: string;
  cityId: string;
  address: string;
  gender: string;
  phone: string;
  username: string;
  email: string;
  password: string;
  isAdmin: boolean;
  isOrganiser: boolean;
  isUser: boolean
}