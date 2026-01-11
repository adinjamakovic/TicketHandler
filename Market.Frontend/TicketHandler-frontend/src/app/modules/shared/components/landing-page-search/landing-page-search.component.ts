import { Component, EventEmitter, Output } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-landing-page-search',
  standalone: false,
  templateUrl: './landing-page-search.component.html',
  styleUrl: './landing-page-search.component.scss',
})
export class LandingPageSearchComponent {
  @Output() search = new EventEmitter<any>();

  searchForm: FormGroup;

  constructor(private fb: FormBuilder) {
    this.searchForm = this.fb.group({
      location: [''],
      startDate: [''],
      endDate: [''],
      eventName: ['']
    });
  }

  submit(){
    this.search.emit(this.searchForm.value);
  }
}
