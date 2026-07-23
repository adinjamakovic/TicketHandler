import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BaseComponent } from '../../../core/components/base-classes/base-component';
import { AuthFacadeService } from '../../../core/services/auth/auth-facade.service';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent extends BaseComponent implements OnInit {
  private auth = inject(AuthFacadeService);
  private route = inject(ActivatedRoute);

  private returnUrl: string | null = null;

  ngOnInit(): void {
    this.returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');

    if (this.auth.isAuthenticated()) {
      return;
    }

    this.signIn();
  }

  signIn(): void {
    this.startLoading();
    this.auth.login(this.returnUrl ?? undefined);
  }
}
