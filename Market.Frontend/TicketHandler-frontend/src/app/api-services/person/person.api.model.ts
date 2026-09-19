
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
  email:string;
}

// ================================================
// ================= COMMANDS =====================
// ================================================

/**
 * Creates a regular user account. The security roles are set by the server and are not part
 * of the request - granting admin/organiser rights goes through UpdatePersonRolesCommand.
 */
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
}

/** Admin-only: PUT /Person/{id}/roles */
export interface UpdatePersonRolesCommand {
  isAdmin: boolean;
  isOrganiser: boolean;
  isUser: boolean;
}

export interface UpdatePersonCommand {
  firstName: string;
  lastName: string;
  birthDate: string;
  cityId: number;
  address: string;
  gender: string;
  phone: string;
  email: string;
  password?: string | null;
}