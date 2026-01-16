import { Injectable } from '@angular/core';
import { BasePagedQuery } from '../../core/models/paging/base-paged-query';
import { PageResult } from '../../core/models/paging/page-result';


// ==============Queries==============

export class ListTicketTypesRequest extends BasePagedQuery{
  search?: string | null;
}

export interface ListTicketTypesQueryDto{
  id:number;
  name: string;
  description: string;
}

export type ListTicketTypesResponse = PageResult<ListTicketTypesQueryDto>

export interface GetTicketTypesByIdQueryDto{
  name: string;
  description:string;
}

// ==============Commands==============

export interface CreateTicketTypesCommand{
  name:string;
  description: string;
}

export interface UpdateTicketTypesCommand{
  id:number;
  name:string;
  description:string;
}

export interface DeleteTicketTypesCommand{
  id:number;
}