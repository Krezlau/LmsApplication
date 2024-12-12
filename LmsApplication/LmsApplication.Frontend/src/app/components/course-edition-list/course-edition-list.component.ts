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
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { UserRole } from '../../types/users/user-role';
import { CourseEditionService } from '../../services/course-edition.service';
import { CollectionResource } from '../../types/collection-resource';

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
  @Input() course: ApiResponse<CourseModel> | null = null;
  @Input() type: 'all' | 'my' | 'open-registration' = 'all';

  sub = new Subscription();

  constructor(
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute,
    private courseEditionService: CourseEditionService,
  ) {}

  authState = this.authService.authState;
  courseId: string = '';
  courseEditions: CourseEditionModel[] = [];

  page = 0;
  pageSize = 10;
  nextPage = true;

  addEdition(edition: CourseEditionModel) {
    this.courseEditions = [...this.courseEditions, edition];
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
  ngOnInit() {
    this.sub = this.route.params.subscribe((params) => {
      this.courseId = params['courseId'];
      if (this.type === 'all') {
        this.sub.add(
          this.courseEditionService
            .getCourseEditionsByCourseId(
              this.courseId,
              this.page + 1,
              this.pageSize,
            )
            .subscribe(
              (data: ApiResponse<CollectionResource<CourseEditionModel>>) => {
                this.courseEditions = [...this.courseEditions, ...data.data!.items];
                this.page++;
                this.nextPage = data.data?.totalCount! > this.page * this.pageSize;
              },
            ),
        );
      }
      if (this.type === 'my') {
        this.sub.add(
          this.courseEditionService
            .getMyCourseEditions(this.page + 1, this.pageSize)
            .subscribe(
              (data: ApiResponse<CollectionResource<CourseEditionModel>>) => {
                this.courseEditions = [...this.courseEditions, ...data.data!.items];
                this.page++;
                this.nextPage = data.data?.totalCount! > this.page * this.pageSize;
              },
            ),
        );
      }
      if (this.type === 'open-registration') {
        this.sub.add(
          this.courseEditionService
            .getOpenRegistrationCourseEditions(this.page + 1, this.pageSize)
            .subscribe(
              (data: ApiResponse<CollectionResource<CourseEditionModel>>) => {
                this.courseEditions = [...this.courseEditions, ...data.data!.items];
                this.page++;
                this.nextPage = data.data?.totalCount! > this.page * this.pageSize;
              },
            ),
        );
      }
    });
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }
}
