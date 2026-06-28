// === QUERIES (READ) ===

import {BasePagedQuery} from '../../core/models/paging/base-paged-query';
import {PageResult} from '../../core/models/paging/page-result';

export class ListPerformersRequest extends BasePagedQuery {
  search?: string | null;
}

export interface ListPerformersQueryDto {
  id: number;
  name: string;
  description?: string | null;
  genre: string;
}

export type ListPerformersResponse = PageResult<ListPerformersQueryDto>;

export interface GetPerformerByIdQueryDto {
  id: number;
  name: string;
  description?: string | null;
  genre: string;
}
