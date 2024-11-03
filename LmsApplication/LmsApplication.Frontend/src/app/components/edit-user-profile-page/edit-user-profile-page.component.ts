import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription, tap } from 'rxjs';
import { UserService } from '../../services/user.service';
import { ActivatedRoute, Router } from '@angular/router';
import { UserModel } from '../../types/users/user-model';
import { AlertService } from '../../services/alert.service';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-edit-user-profile-page',
  standalone: true,
  imports: [ReactiveFormsModule, NgIf],
  templateUrl: './edit-user-profile-page.component.html',
})
export class EditUserProfilePageComponent implements OnInit, OnDestroy {
  sub = new Subscription();
  user: UserModel | null = null;
  editLoading = false;
  loading = false;

  nameControl: FormControl = new FormControl('', [Validators.required]);
  surnameControl: FormControl = new FormControl('', [Validators.required]);
  bioControl: FormControl = new FormControl('', []);

  constructor(
    private userService: UserService,
    private route: ActivatedRoute,
    private alertService: AlertService,
    private router: Router,
  ) {}

  edit(event: Event) {
    event.preventDefault();

    if (
      !this.nameControl.valid ||
      !this.surnameControl.valid ||
      !this.bioControl.valid
    ) {
      this.alertService.show('Form invalid', 'error');
      return;
    }

    this.editLoading = true;
    this.sub.add(
      this.userService
        .updateUser(
          this.nameControl.value,
          this.surnameControl.value,
          this.bioControl.value === '' ? null : this.bioControl.value,
        )
        .pipe(
          tap({
            next: (_: UserModel) => {
              this.alertService.show('Profile updated successfully', 'success');
              this.router.navigate(['/users', this.user!.email]);
              this.editLoading = false;
            },
            error: (err) => {
              if (err.error.message) {
                this.alertService.show(err.error.message, 'error');
              } else {
                this.alertService.show(
                  'Something went wrong while updating profile.',
                  'error',
                );
              }
              this.nameControl.setValue(this.user!.name);
              this.surnameControl.setValue(this.user!.surname);
              this.bioControl.setValue(this.user!.bio ?? '');
              this.editLoading = false;
            },
          }),
        )
        .subscribe(),
    );
  }

  async ngOnInit() {
    this.loading = true;
    this.sub.add(
      this.route.params.subscribe((params) => {
        this.sub.add(
          this.userService
            .getUser(params['email'])
            .pipe(
              tap({
                next: (response: UserModel) => {
                  this.user = response;
                  this.loading = false;
                  this.nameControl.setValue(this.user.name);
                  this.surnameControl.setValue(this.user.surname);
                  this.bioControl.setValue(this.user.bio ?? '');
                },
                error: () => {
                  this.alertService.show(
                    'Could not fetch user data. Try again.',
                    'error',
                  );
                  this.loading = false;
                },
              }),
            )
            .subscribe(),
        );
      }),
    );
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
