import { Component, Input, OnDestroy } from '@angular/core';
import {
  CourseEditionModel,
  CourseEditionStatus,
} from '../../types/courses/course-edition-model';
import { Subscription } from 'rxjs';
import { ApiResponse } from '../../types/api-response';
import { NgFor, NgIf } from '@angular/common';
import { CourseEditionAddFormComponent } from '../course-edition-add-form/course-edition-add-form.component';
import { AuthService } from '../../services/auth.service';
import { CourseModel } from '../../types/courses/course-model';
import { CourseEditionStatusLabelComponent } from '../course-edition-status-label/course-edition-status-label.component';
import { Router, RouterLink } from '@angular/router';
import { UserRole } from '../../types/users/user-role';

@Component({
  selector: 'app-course-edition-list',
  standalone: true,
  imports: [
    NgFor,
    NgIf,
    CourseEditionAddFormComponent,
    CourseEditionStatusLabelComponent,
    RouterLink,
  ],
  templateUrl: './course-edition-list.component.html',
})
export class CourseEditionListComponent implements OnDestroy {
  @Input() courseEditions: ApiResponse<CourseEditionModel[]> | null = null;
  @Input() course: ApiResponse<CourseModel> | null = null;

  sub = new Subscription();

  constructor(
    private authService: AuthService,
    private router: Router,
  ) {}

  authState = this.authService.authState;

  addEdition(edition: CourseEditionModel) {
    if (this.courseEditions?.data) {
      this.courseEditions.data = [...this.courseEditions.data, edition];
    }
  }

  navigateToEdition(edition: CourseEditionModel) {
    if (
      edition.isUserRegistered ||
      this.authState().userData?.role == UserRole.Admin
    ) {
      this.router.navigate(['editions', edition.id]);
      return;
    }

    if (edition.status == CourseEditionStatus.RegistrationOpen) {
      this.router.navigate(['editions', edition.id], {
        queryParams: { page: 'register' },
      });
      return;
    }
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }
}
