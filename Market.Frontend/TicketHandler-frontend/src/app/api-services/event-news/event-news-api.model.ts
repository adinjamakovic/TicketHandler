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
    //THIS needs to be fixed later on!
    image: string;
}

export type ListEventNewsResponse = PageResult<ListEventNewsQueryDto>;

// ================================================
// ================== GET BY ID  ==================
// ================================================

export interface GetEventNewsByIdDto {
    id: number;
    event: string;
    organizer: string;
    header: string;
    body: string;
    //FIX LATER
    image: string;
}

// ================================================
// =================== COMMANDS  ==================
// ================================================

export interface CreateEventNewsCommand {
    eventId: number;
    header: string;
    body: string;
    //ADD LATER
    //image: ?????
}

export interface  UpdateEventNewsCommand {
    header: string;
    body: string;
    //ADD LATER
    //image: ?????
}

export interface DeleteEventNewsCommand {
    id: number;
}