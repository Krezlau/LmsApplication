import { Component, OnDestroy } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { AlertService } from '../../services/alert.service';
import { Subscription } from 'rxjs';
import { tap } from 'rxjs/operators';
import { UserService } from '../../services/user.service';
import { ApiResponse } from '../../types/api-response';
import { UserModel } from '../../types/users/user-model';
import { LoginResponse } from '../../types/users/login-response';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [RouterLink, ReactiveFormsModule, NgIf],
  templateUrl: './login-page.component.html',
})
export class LoginPageComponent implements OnDestroy {
  constructor(
    private authService: AuthService,
    private alertService: AlertService,
    private userService: UserService,
    private router: Router,
  ) {}

  emailControl: FormControl = new FormControl('', [
    Validators.required,
    Validators.email,
  ]);
  passwordControl: FormControl = new FormControl('', [
    Validators.required,
    Validators.minLength(8),
  ]);

  sub: Subscription = new Subscription();
  loading = false;

  login(event: Event) {
    event.preventDefault();
    if (this.emailControl.invalid || this.passwordControl.invalid) {
      this.alertService.show(
        'Please provide valid email and password',
        'error',
      );
      return;
    }

    this.loading = true;
    this.sub.add(
      this.authService
        .login(this.emailControl.value, this.passwordControl.value)
        .pipe(
          tap({
            next: (response: LoginResponse) => {
              this.sub.add(
                this.userService
                  .getMe(response.accessToken)
                  .pipe(
                    tap({
                      next: (userData: UserModel) => {
                        this.authService.loadState(
                          response.accessToken,
                          response.refreshToken,
                          userData!,
                        );
                        this.loading = false;
                        this.alertService.show('Logged in', 'success');
                        this.router.navigate(['/home']);
                      },
                      error: (error) => {
                        this.loading = false;
                        this.alertService.show(
                          'Failed to get user data',
                          'error',
                        );
                      },
                    }),
                  )
                  .subscribe(),
              );
            },
            error: (error: any) => {
              this.loading = false;
              if (!error?.error?.message) {
                this.alertService.show('Failed to login', 'error');
              } else {
                this.alertService.show(error.error.message, 'error');
              }
            },
          }),
        )
        .subscribe(),
    );
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }
}
