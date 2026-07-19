import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-qr-overlay',
  standalone: false,
  templateUrl: './qr-overlay.component.html',
  styleUrl: './qr-overlay.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class QrOverlayComponent {
  @Input() visible = false;
  @Input() title = 'QR Code';
  @Input() value = 'placeholder';


  @Output() closed = new EventEmitter<void>();

  close(): void {
    this.closed.emit();
  }
}
