import { Injectable } from '@angular/core';
import { BasePagedQuery } from '../../core/models/paging/base-paged-query';
import { PageResult } from '../../core/models/paging/page-result';

// ==============Queries==============
export class ListTicketsRequest extends BasePagedQuery {
  eventName?: string| null;
}
export interface ListTicketsQueryDto{
  id:number;
  event:ListTicketsQueryEvent;
  ticketType:ListTicketsQueryTicketType;
  quantityInStock: number;
  unitPrice:number;
  benefits:string;
}
export interface ListTicketsQueryEvent{
  id:number;
  name:string;
}
export interface ListTicketsQueryTicketType{
  id:number;
  name:string;
}
export type ListTicketsResponse = PageResult<ListTicketsQueryDto>;

export interface GetTicketsByIdQueryDto{
  id:number;
  event:GetTicketsByIdQueryDtoEvent;
  ticketType:GetTicketsByIdQueryDtoTicketType;
  quantityInStock: number;
  unitPrice:number;
  benefits:string;
}
export interface GetTicketsByIdQueryDtoEvent{
  id:number;
  name:string;
}
export interface GetTicketsByIdQueryDtoTicketType{
  id:number;
  name:string;
}
export interface GetTicketsByEventIdQueryDto{
  id:number;
  eventId:number;
  ticketTypeId:number;
  quantityInStock: number;
  unitPrice:number;
  benefits:string;
}
export interface GetTicketsByEventNameQueryDto{
  id:number;
  event:GetTicketsByEventNameQueryDtoEvent;
  ticketType:GetTicketsByEventNameQueryDtoTicketType;
  quantityInStock: number;
  unitPrice:number;
  benefits:string;
}
export interface GetTicketsByEventNameQueryDtoEvent{
  id:number;
  name:string;
}
export interface GetTicketsByEventNameQueryDtoTicketType{
  id:number;
  name:string;
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