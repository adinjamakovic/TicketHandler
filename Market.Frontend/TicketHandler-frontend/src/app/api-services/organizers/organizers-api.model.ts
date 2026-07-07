import { BasePagedQuery } from "../../core/models/paging/base-paged-query";
import { PageResult } from "../../core/models/paging/page-result";

// ================================================
// ================== LIST QUERY ==================
// ================================================

export class ListOrganizersRequest extends BasePagedQuery{
    search?: string | null;
}

export interface ListOrganizersQueryDto {
    id: number;
    name: string;
    description?: string | null;
    cityName: string;
    userName: string;
    emailAddress: string;
    isDeleted: boolean;
}

export type ListOrganizersResponse = PageResult<ListOrganizersQueryDto>;

// ================================================
// ================== GET BY ID  ==================
// ================================================

export interface GetOrganizerByIdQueryEventDto{
    id: number;
    Name: string;
    Description?: string | null;
    ScheduledDate: string;
}

export interface GetOrganizerByIdQueryDtoUser {
    firstName: string;
    lastName: string;
    birthDate: string;
    cityId: number;
    address: string;
    gender: string;
    phone: string;
    email: string;
}

export interface GetOrganizerByIdQueryDto {
    id: number;
    name: string;
    description?: string | null;
    email: string;
    city: string;
    logo: string;
    address: string;
    cityId: number;
    user: GetOrganizerByIdQueryDtoUser;
    events: GetOrganizerByIdQueryEventDto[];
}

export interface GetOrganizerByUserIdQueryDto {
    id: number;
    Name: string;
    description?: string | null;
    email: string;
    city: string;
    events: GetOrganizerByUserIdQueryEventDto[];
}
export interface GetOrganizerByUserIdQueryEventDto{
    id: number;
    Name: string;
    Description?: string | null;
    ScheduledDate: string;
}
// ================================================
// ================= COMMANDS =====================
// ================================================

export interface CreateAndUpdateOrganizerCommandUser{
  firstName: string;
  lastName: string;
  birthDate: string;
  cityId: number;
  address: string;
  gender: string;
  phone: string;
  email: string;
  password: string;
}

export interface CreateOrganizerCommand {
    name: string;
    description: string;
    address: string;
    cityid: number;
    logo?: File | null;
    user: CreateAndUpdateOrganizerCommandUser;
}

export interface DeleteOrganizerCommand{
    id: number;
}

export interface UpdateOrganizerCommand{
    name: string;
    description: string;
    address: string;
    cityid: number;
    logo?: File | null;
    user: CreateAndUpdateOrganizerCommandUser;
}
