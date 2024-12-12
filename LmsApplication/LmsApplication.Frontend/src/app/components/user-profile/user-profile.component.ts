import {
  Component,
  Input,
  OnDestroy,
  ViewChild,
  ElementRef,
} from '@angular/core';
import { UserModel } from '../../types/users/user-model';
import { NgIf } from '@angular/common';
import { UserRole, toHumanReadable } from '../../types/users/user-role';
import { AuthService } from '../../services/auth.service';
import { RouterLink } from '@angular/router';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { UserService } from '../../services/user.service';
import { Subscription, tap } from 'rxjs';
import { AlertService } from '../../services/alert.service';
import { CourseEditionListComponent } from '../course-edition-list/course-edition-list.component';

@Component({
  selector: 'app-user-profile',
  standalone: true,
  imports: [NgIf, RouterLink, ReactiveFormsModule, CourseEditionListComponent],
  templateUrl: './user-profile.component.html',
})
export class UserProfileComponent implements OnDestroy {
  protected readonly UserRole = UserRole;
  @Input() user: UserModel | null = null;
  @ViewChild('dialogElement') dialogElement: ElementRef | undefined;

  constructor(
    private authService: AuthService,
    private userService: UserService,
    private alertService: AlertService,
  ) {}

  authState = this.authService.authState;
  roleFormControl = new FormControl(UserRole.Student);
  updateRoleLoading = false;
  sub = new Subscription();

  changeUserRole(event: Event) {
    event.preventDefault();

    if (!this.user) {
      return;
    }

    this.updateRoleLoading = true;
    this.sub.add(
      this.userService
        .updateUserRole(
          this.user.id,
          this.roleFormControl.value ?? UserRole.Student,
        )
        .pipe(
          tap({
            next: () => {
              this.updateRoleLoading = false;

              const value = this.roleFormControl.value;
              if (!value) {
                this.user!.role = UserRole.Student;
              } else if (+value === 1) {
                this.user!.role = UserRole.Teacher;
              } else if (+value === 2) {
                this.user!.role = UserRole.Admin;
              } else {
                this.user!.role = UserRole.Student;
              }

              this.dialogElement?.nativeElement.close();
            },
            error: (err) => {
              this.updateRoleLoading = false;
              if (err.error?.message) {
                this.alertService.show(err.error.message, 'error');
              } else {
                this.alertService.show('Failed to update user role', 'error');
              }
            },
          }),
        )
        .subscribe(),
    );
  }

  toHumanReadable(role: UserRole) {
    return toHumanReadable(role);
  }

  isLoggedInUser() {
    return this.user?.id == (this.authState().userData?.id ?? '');
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
