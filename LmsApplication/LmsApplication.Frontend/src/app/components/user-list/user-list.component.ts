import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { UserModel } from '../../types/users/user-model';
import { NgClass, NgForOf, NgIf } from '@angular/common';
import { UserCardComponent } from '../user-card/user-card.component';
import { UserService } from '../../services/user.service';
import { Subscription } from 'rxjs';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { toHumanReadable } from '../../types/users/user-role';
import { CourseEditionAddUserDialogComponent } from '../course-edition-add-user-dialog/course-edition-add-user-dialog.component';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [
    NgForOf,
    NgIf,
    UserCardComponent,
    NgClass,
    CourseEditionAddUserDialogComponent,
  ],
  templateUrl: './user-list.component.html',
})
export class UserListComponent implements OnInit, OnDestroy {
  @Input() courseEditionId: string | null = null;
  protected readonly toHumanReadable = toHumanReadable;

  users: UserModel[] = [];

  sub = new Subscription();

  usersLoading = false;
  authState = this.authService.authState;

  constructor(
    private userService: UserService,
    private authService: AuthService,
    private router: Router,
  ) {}

  async onUserClick(user: UserModel) {
    if (user) {
      await this.router.navigate(['users', user.email]);
    }
  }

  getUserIds() {
    return this.users.map((user) => user.id);
  }

  removeUser(user: UserModel) {
    // TODO
    console.log('removeUser', user);
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
            this.usersLoading = false;
          }),
      );
    }
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
