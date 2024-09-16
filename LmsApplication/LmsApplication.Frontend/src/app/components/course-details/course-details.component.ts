import {Component, Input} from '@angular/core';
import {CourseModel} from "../../types/courses/course-model";
import {NgIf} from "@angular/common";
import {AuthService} from "../../services/auth.service";
import {UserRole} from "../../types/users/user-role";
import {CourseService} from "../../services/course.service";

@Component({
  selector: 'app-course-details',
  standalone: true,
  imports: [
    NgIf
  ],
  templateUrl: './course-details.component.html'
})
export class CourseDetailsComponent {
  @Input() course: CourseModel | null = null;

  authState = this.authService.authState;
  constructor(private authService: AuthService, private courseService: CourseService) {
  }

  deleteCourse() {
    if (!this.course) return;

    // this.courseService.deleteCourse(this.course.id).subscribe(() => {});
  }

  protected readonly UserRole = UserRole;
}
