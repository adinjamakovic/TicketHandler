import { ChangeDetectionStrategy, Component, EventEmitter, Input, OnChanges, Output } from '@angular/core';
import { ListEventsQueryDto } from '../../../../api-services/events/events-api.model';

@Component({
  selector: 'app-upcoming-events-carousel',
  standalone: false,
  templateUrl: './upcoming-events-carousel.component.html',
  styleUrl: './upcoming-events-carousel.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UpcomingEventsCarouselComponent implements OnChanges {
  @Input() events: ListEventsQueryDto[] = [];
  @Input() isLoading = false;

  @Output() allEvents = new EventEmitter<void>();

  carouselIndex = 0;

  private readonly pageSize = 4;

  ngOnChanges(): void {
    this.carouselIndex = 0;
  }

  prevSlide(): void {
    if (this.canGoPrev) {
      this.carouselIndex--;
    }
  }

  nextSlide(): void {
    if (this.canGoNext) {
      this.carouselIndex++;
    }
  }

  get visibleSlides(): ListEventsQueryDto[] {
    return this.events.slice(this.carouselIndex, this.carouselIndex + this.pageSize);
  }

  get canGoPrev(): boolean {
    return this.carouselIndex > 0;
  }

  get canGoNext(): boolean {
    return this.carouselIndex < this.events.length - this.pageSize;
  }
}
