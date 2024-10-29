import { Component, OnDestroy } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { AlertService } from '../../services/alert.service';
import { Subscription } from 'rxjs';
import { tap } from 'rxjs/operators';
import { UserService } from '../../services/user.service';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-register-page',
  standalone: true,
  imports: [RouterLink, ReactiveFormsModule, NgIf],
  templateUrl: './register-page.component.html',
})
export class RegisterPageComponent implements OnDestroy {
  constructor(
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
  repeatPasswordControl: FormControl = new FormControl('', [
    Validators.required,
    Validators.minLength(8),
  ]);
  nameControl: FormControl = new FormControl('', [Validators.required]);
  surnameControl: FormControl = new FormControl('', [Validators.required]);

  sub: Subscription = new Subscription();
  loading = false;

  login(event: Event) {
    event.preventDefault();

    if (
      this.emailControl.invalid ||
      this.passwordControl.invalid ||
      this.repeatPasswordControl.invalid ||
      this.nameControl.invalid ||
      this.surnameControl.invalid
    ) {
      this.alertService.show(
        'Please provide valid email and password',
        'error',
      );
      return;
    }

    if (this.passwordControl.value !== this.repeatPasswordControl.value) {
      this.alertService.show('Passwords do not match', 'error');
      return;
    }

    this.loading = true;
    this.sub.add(
      this.userService
        .register(
          this.emailControl.value,
          this.nameControl.value,
          this.surnameControl.value,
          this.passwordControl.value,
        )
        .pipe(
          tap({
            next: (_: any) => {
              this.loading = false;
              this.alertService.show('Successfully registered. You can now log in.', 'success');
              this.router.navigate(['/login']);
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
