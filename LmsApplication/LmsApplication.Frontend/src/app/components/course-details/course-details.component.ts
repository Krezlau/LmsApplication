import { Component, Input } from '@angular/core';
import { CourseModel } from '../../types/courses/course-model';
import { Location, NgIf } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { UserRole } from '../../types/users/user-role';
import { CourseService } from '../../services/course.service';
import { ApiResponse } from '../../types/api-response';
import { GradeRowDefinitionDeleteModalComponent } from '../grade-row-definition-delete-modal/grade-row-definition-delete-modal.component';

@Component({
  selector: 'app-course-details',
  standalone: true,
  imports: [NgIf, GradeRowDefinitionDeleteModalComponent],
  templateUrl: './course-details.component.html',
})
export class CourseDetailsComponent {
  @Input() course: ApiResponse<CourseModel> | null = null;

  authState = this.authService.authState;
  constructor(
    private authService: AuthService,
    private courseService: CourseService,
    private location: Location,
  ) {}

  deleteCourse() {
    if (!this.course || !this.course.data) return;

    this.courseService.deleteCourse(this.course.data.id).subscribe(() => {
      this.location.back();
    });
  }

  protected readonly UserRole = UserRole;
}
