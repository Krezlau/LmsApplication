import { Component } from '@angular/core';
import { CourseEditionListComponent } from '../course-edition-list/course-edition-list.component';
import { AsyncPipe, NgIf } from '@angular/common';
import { ApiResponse } from '../../types/api-response';
import { CourseEditionService } from '../../services/course-edition.service';
import { CourseEditionModel } from '../../types/courses/course-edition-model';
import { Observable } from 'rxjs';
import { AuthService } from '../../services/auth.service';
import { CollectionResource } from '../../types/collection-resource';

@Component({
  selector: 'app-home-page',
  standalone: true,
  imports: [CourseEditionListComponent, AsyncPipe, NgIf],
  templateUrl: './home-page.component.html',
})
export class HomePageComponent {
  constructor(private courseEditionService: CourseEditionService, private authService: AuthService) {}

  authState = this.authService.authState;

  courseList$: Observable<ApiResponse<CollectionResource<CourseEditionModel>>> =
    this.courseEditionService.getMyCourseEditions();

  openRegistrationList$: Observable<ApiResponse<CollectionResource<CourseEditionModel>>> =
    this.courseEditionService.getOpenRegistrationCourseEditions();
}
