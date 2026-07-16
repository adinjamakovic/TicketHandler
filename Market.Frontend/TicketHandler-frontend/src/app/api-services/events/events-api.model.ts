    import { BasePagedQuery } from "../../core/models/paging/base-paged-query";
import { PageResult } from "../../core/models/paging/page-result";


// ================================================
// ================== LIST QUERY ==================
// ================================================

export class ListEventsRequest extends BasePagedQuery {
    search?: string | null;
    dateFrom?: Date | null;
    dateTo?: Date | null;
    city?: string | null;
}

export interface ListEventsQueryDtoOrganizer {
    id: number;
    name: string;
    address: string;
    city: string;
    username: string;
}

export interface ListEventsQueryDto {
    id: number;
    name: string;
    description?: string | null;
    scheduledDate: string;
    organizer: ListEventsQueryDtoOrganizer;
    image: string;
    venueName: string;
    eventType: string;
    venueCity?: string | null;
}

export type ListEventsResponse = PageResult<ListEventsQueryDto>;

export class ListEventsWithPerformersRequest extends BasePagedQuery{
    search?: string | null;
}

export interface ListEventsWithPerformersQueryDtoPerformers {
    name: string;
    description?: string | null;
    genre: string;
}

export interface ListEventsWithPerformersQueryDto {
    id: number;
    name: string;
    description?: string | null;
    scheduledDate: string;
    organizerName: string;
    venueName: string;
    //IMPLEMENT LATER
    image: string;
    eventTypeName: string;
    performers: ListEventsWithPerformersQueryDtoPerformers[];
}

export type ListEventsWithPerformersResponse = PageResult<ListEventsWithPerformersQueryDto>;

export interface GetEventByIdQueryDtoPerformers {
    id: number,
    performerId: number,
    timeStamp: string
}

export interface GetEventByIdQueryDto{
   id: number,
  name: string,
  description: string,
  scheduledDate: string,
  venueId: number,
  venueName: string,
  venueCity?: string | null,
  image: string|Blob,
  eventTypeId: number,
  eventTypeName: string,
  organizerName: string,
  performers: GetEventByIdQueryDtoPerformers[]
}

export class GetEventsByOrganizerIdRequest extends BasePagedQuery {
    id: number = 1;
}

export interface GetEventsByOrganizerIdDtoPerformers {
    name: string;
    description?: string | null;
    genre: string;
}


export interface GetEventsByOrganizerIdDto{
    id: number;
    name: string;
    description?: string | null;
    scheduledDate: string;
    organizerName: string;
    venueName: string;
    //IMPLEMENT LATER
    image: string;
    eventTypeName: string;
    performers: GetEventsByOrganizerIdDtoPerformers[];
}


export type GetEventByOrganizerIdResponse = PageResult<GetEventsByOrganizerIdDto>;

// ================================================
// ================== COMMANDS ====================
// ================================================
export interface CreateEventCommandPerformer{
    performerId: number;
    timeStamp: string;
}

export interface CreateEventCommand{
    name: string;
    description?: string | null;
    scheduledDate: string;
    venueId: number;
    image?: File | null;
    eventTypeId: number;
    performers: CreateEventCommandPerformer[];
}

export interface DeleteEventCommand{
    id: number;
}

export interface UpdateEventCommandPerformer{
    id: number;
    performerId: number;
    timeStamp: string;
}

export interface UpdateEventCommand {
    name: string;
    description?: string | null;
    scheduledDate: string;
    venueId: number;
    image?: File | null;
    eventTypeId: number;
    performers: UpdateEventCommandPerformer[];
}
