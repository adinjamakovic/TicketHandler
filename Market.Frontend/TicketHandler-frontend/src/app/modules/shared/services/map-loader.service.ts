import { Injectable } from "@angular/core";
import { environment } from "../../../../environments/environment";

@Injectable({ providedIn: 'root' })
export class MapLoaderService {
    private isLoaded = false;

    load(): Promise<void> {
        if(this.isLoaded) return Promise.resolve();

        return new Promise((resolve, reject) => {
            const script = document.createElement('script');
            const params = new URLSearchParams({
                key: environment.GApiKey,
                v: 'beta',
                libraries: 'marker,routes',
                callback: 'initMap'
            });

            script.src = `https://maps.googleapis.com/maps/api/js?${params.toString()}`;
            script.async = true;
            script.defer = true;

            (window as any).initMap = () => resolve(undefined);

            script.onerror = () => reject(new Error('Failed to load Google Maps'));
            document.head.appendChild(script);
        }).then(() => {
            this.isLoaded = true;
        })
    }

    getCurrentPosition(): Promise<GeolocationPosition> {
        return new Promise((resolve, reject) => {
            if (!navigator.geolocation) {
                reject(new Error('Geolocation is not supported by this browser'));
                return;
            }

            navigator.geolocation.getCurrentPosition(resolve, reject, {
                enableHighAccuracy: true,
                timeout: 10000,
            });
        });
    }

    getRoute(
        origin: google.maps.LatLngLiteral,
        destination: string,
        travelMode: google.maps.TravelMode = google.maps.TravelMode.DRIVING,
    ): Promise<google.maps.DirectionsResult> {
        const directionsService = new google.maps.DirectionsService();

        return directionsService.route({
            origin,
            destination,
            travelMode,
        });
    }
}