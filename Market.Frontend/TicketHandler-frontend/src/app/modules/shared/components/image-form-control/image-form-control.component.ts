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

  protected onChange: Function | undefined;

  @ViewChild('fileInput', {static: true})
  protected fileInput!: ElementRef<HTMLInputElement>;

  writeValue(value: string): void {
    this.fileInput.nativeElement.value = '';
    this.imgSrc = value ? value : `https://sttickethandlerrs1.blob.core.windows.net/web/no_image.jpg`;
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