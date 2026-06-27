import { Component, ElementRef, OnDestroy, ViewChild } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-image-form-control',
  standalone: false,
  templateUrl: './image-form-control.component.html',
  styleUrl: './image-form-control.component.scss',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: ImageFormControlComponent,
      multi: true
    }
  ]
})
export class ImageFormControlComponent implements ControlValueAccessor, OnDestroy {
  protected imgSrc?: string;

  // A function to notify the form of value changes (set by Angular)
  protected onChange: Function | undefined;



  /**
   * Obtains a reference to the file input element using ViewChild.
   * This reference is used to clear the input's value after an image is selected or written.
   * The {static: true} option ensures the reference is available during initialization.
   */
  @ViewChild('fileInput', {static: true})
  protected fileInput!: ElementRef<HTMLInputElement>;

  writeValue(value: string): void {
    this.fileInput.nativeElement.value = '';
    this.imgSrc = value ? `${environment.apiUrl}/${value}` : `${environment.apiUrl}/no_image.jpg`;
  }
  registerOnChange(fn: any): void {
    this.onChange = fn;
  }
  registerOnTouched(fn: any): void {
  }
  setDisabledState?(isDisabled: boolean): void {
  }
  
  
  protected onImageSelected(event: Event) {
    const file = (event.target as HTMLInputElement).files?.[0];

    if (file) {
      this.revokeImageURL();
      this.imgSrc = this.generateImageURL(file);
      this.onChange?.(file);
    }
  }

  ngOnDestroy() {
    // Ensures any existing blob URLs are revoked to prevent memory leaks.
    this.revokeImageURL()
  }

  private revokeImageURL() {
    if (this.imgSrc && this.imgSrc.startsWith('blob:')) {
      URL.revokeObjectURL(this.imgSrc);
      // Reset the imgSrc because it points to an invalid URL
      this.imgSrc = undefined;
    }
  }

  private generateImageURL(file: File): string {
    return URL.createObjectURL(file)
  }

}