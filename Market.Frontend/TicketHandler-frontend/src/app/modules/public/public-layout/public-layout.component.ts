import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { provideNativeDateAdapter } from '@angular/material/core';
import { EventsApiService } from '../../../api-services/events/events-api.service';
import { ListEventsQueryDto } from '../../../api-services/events/events-api.model';
import { LandingSearchEvent } from '../../shared/components/landing-page-search/landing-page-search.component';

@Component({
  selector: 'app-public-layout',
  standalone: false,
  templateUrl: './public-layout.component.html',
  styleUrl: './public-layout.component.scss',
  providers: [provideNativeDateAdapter()],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PublicLayoutComponent implements OnInit {
  featuredEvent: ListEventsQueryDto | null = null;
  carouselEvents: ListEventsQueryDto[] = [];
  carouselIndex = 0;
  isLoading = true;

  private eventsApi = inject(EventsApiService);
  private cdr = inject(ChangeDetectorRef);

  ngOnInit(): void {
    this.loadEvents({});
  }

  onSearch(filters: LandingSearchEvent): void {
    this.loadEvents(filters);
  }

  clearFilters(): void {
    this.loadEvents({});
  }

  prevSlide(): void {
    if (this.carouselIndex > 0) {
      this.carouselIndex--;
      this.cdr.markForCheck();
    }
  }

  nextSlide(): void {
    if (this.carouselIndex < this.carouselEvents.length - 4) {
      this.carouselIndex++;
      this.cdr.markForCheck();
    }
  }

  get visibleSlides(): ListEventsQueryDto[] {
    return this.carouselEvents.slice(this.carouselIndex, this.carouselIndex + 4);
  }

  get canGoPrev(): boolean {
    return this.carouselIndex > 0;
  }

  get canGoNext(): boolean {
    return this.carouselIndex < this.carouselEvents.length - 4;
  }

  private loadEvents(filters: LandingSearchEvent): void {
    this.isLoading = true;
    this.cdr.markForCheck();

    this.eventsApi
      .list({
        search: filters.search ?? undefined,
        date: filters.date ?? undefined,
        city: filters.city ?? undefined,
        paging: { page: 1, pageSize: 100 },
      } as any)
      .subscribe({
        next: result => {
          const items = result.items ?? [];
          if (items.length > 0) {
            const idx = Math.floor(Math.random() * Math.min(items.length, 5));
            this.featuredEvent = items[idx];
            this.carouselEvents = items.filter((_, i) => i !== idx);
          } else {
            this.featuredEvent = null;
            this.carouselEvents = [];
          }
          this.carouselIndex = 0;
          this.isLoading = false;
          this.cdr.markForCheck();
        },
        error: () => {
          this.isLoading = false;
          this.cdr.markForCheck();
        },
      });
  }
}
