import { Component, Input, OnDestroy } from '@angular/core';
import { CourseEditionModel } from '../../types/courses/course-edition-model';
import { CourseEditionService } from '../../services/course-edition.service';
import { Subscription } from 'rxjs';
import { ApiResponse } from '../../types/api-response';
import { NgFor, NgIf } from '@angular/common';
import { CourseEditionAddFormComponent } from '../course-edition-add-form/course-edition-add-form.component';
import { AuthService } from '../../services/auth.service';
import { CourseModel } from '../../types/courses/course-model';

@Component({
  selector: 'app-course-edition-list',
  standalone: true,
  imports: [NgFor, NgIf, CourseEditionAddFormComponent],
  templateUrl: './course-edition-list.component.html',
})
export class CourseEditionListComponent implements OnDestroy {
  @Input() courseEditions: ApiResponse<CourseEditionModel[]> | null = null;
  @Input() course: ApiResponse<CourseModel> | null = null;

  sub = new Subscription();

  constructor(private authService: AuthService) {}

  authState = this.authService.authState;

  ngOnDestroy() {
    this.sub.unsubscribe();
  }
}
