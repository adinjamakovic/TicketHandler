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
                libraries: 'marker',
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
}