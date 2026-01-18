import {BasePagedQuery} from '../../core/models/paging/base-paged-query';
import {PageResult} from '../../core/models/paging/page-result';

export class ListEventTypesRequest extends BasePagedQuery{
  search?: string | null;
}

export interface ListEventTypesQueryDto{
  id: number;
  name: string;
}

export type ListEventTypesResponse = PageResult<ListEventTypesQueryDto>;
