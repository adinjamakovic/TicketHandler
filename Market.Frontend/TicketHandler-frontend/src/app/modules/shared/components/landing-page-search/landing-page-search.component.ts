import { Component, EventEmitter, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-landing-page-search',
  standalone: false,
  templateUrl: './landing-page-search.component.html',
  styleUrl: './landing-page-search.component.scss',
})
export class LandingPageSearchComponent {
  name = new FormControl('');

}
