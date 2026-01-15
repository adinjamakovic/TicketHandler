import { Injectable } from '@angular/core';
import { BasePagedQuery } from '../../core/models/paging/base-paged-query';
import { PageResult } from '../../core/models/paging/page-result';

// ==============Queries==============
export class ListTicketsRequest extends BasePagedQuery {
  id?: number| null;
}
export interface ListTicketsQueryDto{
  id:number;
  eventId:number;
  ticketTypeId:number;
  quantityInStock: number;
  unitPrice:number;
  benefits:string;
}
export type ListTicketsResponse = PageResult<ListTicketsQueryDto>;

export interface GetTicketsByIdQueryDto{
  id:number;
  eventId:number;
  ticketTypeId:number;
  quantityInStock: number;
  unitPrice:number;
  benefits:string;
}
export interface GetTicketsByEventIdQueryDto{
  id:number;
  eventId:number;
  ticketTypeId:number;
  quantityInStock: number;
  unitPrice:number;
  benefits:string;
}
// ==============Commands==============
export interface CreateTicketsCommand{
  eventId:number;
  ticketTypeId:number;
  quantityInStock: number;
  unitPrice:number;
  benefits:string;
}

export interface UpdateTicketsCommand{
  id:number;
  eventId:number;
  ticketTypeId:number;
  quantityInStock: number;
  unitPrice:number;
  benefits:string;
}

export interface DeleteTicketsCommand{
  id:number;
}