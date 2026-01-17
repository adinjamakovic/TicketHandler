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

export interface GetOrganizerByIdQueryDto {
    id: number;
    name: string;
    description?: string | null;
    email: string;
    city: string;
    cityId: number;
    events: GetOrganizerByIdQueryEventDto[];
}

// ================================================
// ================= COMMANDS =====================
// ================================================

export interface CreateOrganizerCommand {
    name: string;
    description: string;
    address: string;
    cityid: number;
    //implement later:
    //logo: ??????;
    //
    userid: number;
}

export interface DeleteOrganizerCommand{
    id: number;
}

export interface UpdateOrganizerCommand{
    id: number;
    name: string;
    description?: string| null;
    Address: string;
    cityid: number
}
