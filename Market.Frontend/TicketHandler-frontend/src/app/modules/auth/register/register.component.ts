import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BaseComponent } from '../../../core/components/base-classes/base-component';
import { AuthFacadeService } from '../../../core/services/auth/auth-facade.service';

/**
 * Registration lives on IdentityServer, next to login, so both write to and read from the
 * same person store. This component only hands the browser over to it.
 */
@Component({
  selector: 'app-register',
  standalone: false,
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent extends BaseComponent implements OnInit {
  private auth = inject(AuthFacadeService);
  private route = inject(ActivatedRoute);

  private returnUrl: string | null = null;

  ngOnInit(): void {
    this.returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');

    if (this.auth.isAuthenticated()) {
      return;
    }

    this.signUp();
  }

  signUp(): void {
    this.startLoading();
    this.auth.register(this.returnUrl ?? undefined);
  }
}
