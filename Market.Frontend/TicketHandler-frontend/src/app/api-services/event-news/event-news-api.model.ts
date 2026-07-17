import { BasePagedQuery } from "../../core/models/paging/base-paged-query";
import { PageResult } from "../../core/models/paging/page-result";

// ================================================
// ================== LIST QUERY ==================
// ================================================

export class ListEventNewsRequest extends BasePagedQuery {
    organizerId?: number | null;
    eventId?: number | null;
}

export interface ListEventNewsQueryDto{
    id: number;
    event: string;
    organizer: string;
    header: string;
    body: string;
    image: string;
}

export type ListEventNewsResponse = PageResult<ListEventNewsQueryDto>;

// ================================================
// ================== GET BY ID  ==================
// ================================================

export interface GetEventNewsByIdQueryDto {
    id: number;
    event: string;
    organizer: string;
    header: string;
    body: string;
    image: string;
}

// ================================================
// =================== COMMANDS  ==================
// ================================================

export interface CreateEventNewsCommand {
    eventId: number;
    header: string;
    body: string;
    image?: File | null;
}

export interface  UpdateEventNewsCommand {
    header: string;
    body: string;
    image?: File | null;
}

export interface DeleteEventNewsCommand {
    id: number;
}
