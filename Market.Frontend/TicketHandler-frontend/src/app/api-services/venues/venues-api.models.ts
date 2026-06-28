import {BasePagedQuery} from '../../core/models/paging/base-paged-query';
import {PageResult} from '../../core/models/paging/page-result';

export class ListVenuesRequest extends BasePagedQuery{
  search?: string | null;
}

export interface ListVenuesQueryDto {
  id: number;
  name: string;
  seated: number;
  standing: number;
  location: string;
}

export type ListVenuesResponse = PageResult<ListVenuesQueryDto>;
