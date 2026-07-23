import { Component, OnInit, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthFacadeService } from '../../../core/services/auth/auth-facade.service';
import { CurrentUserService } from '../../../core/services/auth/current-user.service';

@Component({
  selector: 'app-auth-callback',
  standalone: false,
  templateUrl: './auth-callback.component.html',
  styleUrl: './auth-callback.component.scss',
})
export class AuthCallbackComponent implements OnInit {
  private router = inject(Router);
  private auth = inject(AuthFacadeService);
  private currentUser = inject(CurrentUserService);

  ngOnInit(): void {
    const returnUrl = this.auth.consumeReturnUrl();

    if (!this.auth.isAuthenticated()) {
      this.router.navigate(['/'], { replaceUrl: true });
      return;
    }

    this.router.navigateByUrl(returnUrl ?? this.currentUser.getDefaultRoute(), {
      replaceUrl: true,
    });
  }
}
