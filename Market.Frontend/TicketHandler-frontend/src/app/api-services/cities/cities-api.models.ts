import {BasePagedQuery} from '../../core/models/paging/base-paged-query';
import {PageResult} from '../../core/models/paging/page-result';

export class ListCitiesRequest extends BasePagedQuery {
  search?: string | null;
}

export interface ListCitiesQueryDto {
  id: number;
  name: string;
  postalCode: string;
}

export type ListCitiesResponse = PageResult<ListCitiesQueryDto>
