import {JsonPipe} from '@angular/common';
import {ChangeDetectionStrategy, Component, OnInit, inject} from '@angular/core';
import {FormControl, FormGroup, FormsModule, ReactiveFormsModule, FormBuilder} from '@angular/forms';
import {provideNativeDateAdapter} from '@angular/material/core';
import {MatDatepickerModule} from '@angular/material/datepicker';
import { MatIcon } from "@angular/material/icon";
import {MatFormFieldModule} from '@angular/material/form-field';
import { SharedModule } from '../../shared/shared-module';
@Component({
  selector: 'app-public-layout',
  standalone: false,
  templateUrl: './public-layout.component.html',
  styleUrl: './public-layout.component.scss',
  providers: [provideNativeDateAdapter()],
  changeDetection: ChangeDetectionStrategy.OnPush,
})

export class PublicLayoutComponent {
  
}
