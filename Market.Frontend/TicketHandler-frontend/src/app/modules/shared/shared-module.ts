import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import {FitPaginatorBarComponent} from './components/fit-paginator-bar/fit-paginator-bar.component';
import {materialModules} from './material-modules';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {TranslatePipe} from '@ngx-translate/core';
import { FitConfirmDialogComponent } from './components/fit-confirm-dialog/fit-confirm-dialog.component';
import {DialogHelperService} from './services/dialog-helper.service';
import { FitLoadingBarComponent } from './components/fit-loading-bar/fit-loading-bar.component';
import { FitTableSkeletonComponent } from './components/fit-table-skeleton/fit-table-skeleton.component';
import { LandingPageSearchComponent } from './components/landing-page-search/landing-page-search.component';
import { ImageFormControlComponent } from './components/image-form-control/image-form-control.component';
import { NavbarComponent } from './components/navbar/navbar.component';
import { FooterComponent } from './components/footer/footer.component';
import { UpcomingEventsCarouselComponent } from './components/upcoming-events-carousel/upcoming-events-carousel.component';
import { QrOverlayComponent } from './components/qr-overlay/qr-overlay.component';
import { GoogleMapsComponent } from './components/google-maps/google-maps.component';
import { GoogleMapsModule } from '@angular/google-maps'


@NgModule({
  declarations: [
    ImageFormControlComponent,
    FitPaginatorBarComponent,
    FitConfirmDialogComponent,
    FitLoadingBarComponent,
    FitTableSkeletonComponent,
    LandingPageSearchComponent,
    NavbarComponent,
    FooterComponent,
    UpcomingEventsCarouselComponent,
    QrOverlayComponent,
    GoogleMapsComponent,
  ],
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    FormsModule,
    TranslatePipe,
    GoogleMapsModule,
    ...materialModules
  ],
  providers: [
    DialogHelperService
  ],
  exports:[
    ImageFormControlComponent,
    FitPaginatorBarComponent,
    NavbarComponent,
    FooterComponent,
    UpcomingEventsCarouselComponent,
    QrOverlayComponent,
    GoogleMapsComponent,
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    TranslatePipe,
    FormsModule,
    FitLoadingBarComponent,
    FitTableSkeletonComponent,
    LandingPageSearchComponent,
    materialModules
  ]
})
export class SharedModule { }
