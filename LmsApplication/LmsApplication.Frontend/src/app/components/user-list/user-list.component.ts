import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { UserModel } from '../../types/users/user-model';
import { NgClass, NgForOf, NgIf } from '@angular/common';
import { UserCardComponent } from '../user-card/user-card.component';
import { UserService } from '../../services/user.service';
import { Subscription, tap } from 'rxjs';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { toHumanReadable } from '../../types/users/user-role';
import { CourseEditionAddUserDialogComponent } from '../course-edition-add-user-dialog/course-edition-add-user-dialog.component';
import { CourseEditionService } from '../../services/course-edition.service';
import { AlertService } from '../../services/alert.service';
import { ShowUserGradesModalComponent } from '../../show-user-grades-modal/show-user-grades-modal.component';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [
    NgForOf,
    NgIf,
    UserCardComponent,
    NgClass,
    CourseEditionAddUserDialogComponent,
    ShowUserGradesModalComponent,
  ],
  templateUrl: './user-list.component.html',
})
export class UserListComponent implements OnInit, OnDestroy {
  @Input() courseEditionId: string | null = null;
  @Input() listType = 'users';
  protected readonly toHumanReadable = toHumanReadable;

  users: UserModel[] = [];

  sub = new Subscription();

  usersLoading = false;
  authState = this.authService.authState;

  constructor(
    private userService: UserService,
    private authService: AuthService,
    private courseEditionService: CourseEditionService,
    private alertService: AlertService,
    private router: Router,
  ) {}

  async onUserClick(user: UserModel) {
    if (user) {
      await this.router.navigate(['users', user.email]);
    }
  }

  userAdded(user: UserModel) {
    this.users = [...this.users, user];
  }

  getUserIds() {
    return this.users.map((user) => user.id);
  }

  removeUser(user: UserModel) {
    if (!this.courseEditionId) {
      return;
    }
    this.sub.add(
      this.courseEditionService
        .removeUserFromCourseEdition(this.courseEditionId, user.id)
        .pipe(
          tap({
            next: () => {
              this.users = this.users.filter((u) => u.id !== user.id);
            },
            error: (err) => {
              if (err.error?.message) {
                this.alertService.show(err.error.message, 'error');
              } else {
                this.alertService.show(
                  'Failed to remove user from course',
                  'error',
                );
              }
            },
          }),
        )
        .subscribe(),
    );
  }

  ngOnInit(): void {
    this.usersLoading = true;
    if (!this.courseEditionId) {
      this.sub.add(
        this.userService.getUsers().subscribe((users) => {
          this.users = users!;
          this.usersLoading = false;
        }),
      );
    } else {
      this.sub.add(
        this.userService
          .getUsersByCourseEdition(this.courseEditionId)
          .subscribe((users) => {
            this.users = users!;
            if (this.listType === 'grades') {
              this.users = this.users.filter((user) => user.role === 0);
            }
            this.usersLoading = false;
          }),
      );
    }
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
