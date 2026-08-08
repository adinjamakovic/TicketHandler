import {BasePagedQuery} from '../../core/models/paging/base-paged-query';
import {PageResult} from '../../core/models/paging/page-result';

export class ListCountriesRequest extends BasePagedQuery {
  search?: string | null;
}

export interface ListCountriesQueryDto {
  id: number;
  name: string;
  /** ISO 3166-1 alpha-2, e.g. "BA" — what Stripe expects for billing addresses. */
  isoCode: string;
  phoneCode: string;
  flag: string | null;
}

export type ListCountriesResponse = PageResult<ListCountriesQueryDto>
