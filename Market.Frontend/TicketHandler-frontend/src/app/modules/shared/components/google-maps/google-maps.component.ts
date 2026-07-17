import { ChangeDetectorRef, Component, inject, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { MapLoaderService } from '../../services/map-loader.service';

@Component({
  selector: 'app-google-maps',
  standalone: false,
  templateUrl: './google-maps.component.html',
  styleUrl: './google-maps.component.scss',
})
export class GoogleMapsComponent implements OnInit, OnChanges {
  @Input() address?: string | null;

  isMapLoaded = false;
  geocodeFailed = false;

  center: google.maps.LatLngLiteral = { lat: 43.3433, lng: 17.8081 };
  zoom = 13;
  markerPosition: google.maps.LatLngLiteral | null = null;

  mapOptions: google.maps.MapOptions = {
    mapId: 'myMap',
  };

  private mapLoaderService = inject(MapLoaderService);
  private cdr = inject(ChangeDetectorRef);

  ngOnInit(): void {
    this.mapLoaderService
      .load()
      .then(() => {
        this.isMapLoaded = true;
        this.cdr.markForCheck();
        return this.geocodeAddress();
      })
      .catch(err => console.error('Failed to load Google Maps', err));
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['address'] && !changes['address'].firstChange && this.isMapLoaded) {
      this.geocodeAddress();
    }
  }

  private async geocodeAddress(): Promise<void> {
    this.geocodeFailed = false;

    if (!this.address?.trim()) {
      this.markerPosition = null;
      this.cdr.markForCheck();
      return;
    }

    try {
      const geocoder = new google.maps.Geocoder();
      const { results } = await geocoder.geocode({ address: this.address });
      const location = results[0]?.geometry?.location;

      if (!location) {
        this.markerPosition = null;
        this.geocodeFailed = true;
        return;
      }

      const position: google.maps.LatLngLiteral = { lat: location.lat(), lng: location.lng() };
      this.center = position;
      this.markerPosition = position;
      this.zoom = 15;
    } catch (err) {
      console.error('Geocoding failed for address:', this.address, err);
      this.markerPosition = null;
      this.geocodeFailed = true;
    } finally {
      this.cdr.markForCheck();
    }
  }
}
