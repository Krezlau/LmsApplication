import { Component, OnDestroy, Input } from '@angular/core';
import { UserListComponent } from '../user-list/user-list.component';
import { UserService } from '../../services/user.service';
import { UserModel } from '../../types/users/user-model';
import { Subscription } from 'rxjs';
import { NgFor, NgIf } from '@angular/common';
import { CourseEditionService } from '../../services/course-edition.service';

@Component({
  selector: 'app-course-edition-add-user-dialog',
  standalone: true,
  imports: [UserListComponent, NgFor, NgIf],
  templateUrl: './course-edition-add-user-dialog.component.html',
})
export class CourseEditionAddUserDialogComponent implements OnDestroy {
  @Input() courseEditionId: string = '';
  @Input() alreadyAddedUserIds: string[] = [];

  constructor(
    private userService: UserService,
    private courseEditionService: CourseEditionService,
  ) {}

  searchUser = '';
  userList: UserModel[] = [];
  sub = new Subscription();

  updateSearchUser(event: any) {
    this.searchUser = event.target.value;
    this.sub.add(
      this.userService.searchUsersByEmail(this.searchUser).subscribe((res) => {
        this.userList = res;
      }),
    );
  }

  addUser(user: UserModel) {
    this.sub.add(
      this.courseEditionService
        .addUserToCourseEdition(this.courseEditionId, user.id)
        .subscribe((_) => {
          this.alreadyAddedUserIds = [...this.alreadyAddedUserIds, user.id];
        }),
    );
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
