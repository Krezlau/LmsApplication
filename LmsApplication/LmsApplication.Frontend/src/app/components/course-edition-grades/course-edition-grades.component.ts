import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { NgIf } from '@angular/common';
import { CourseEditionStudentGradesComponent } from '../course-edition-student-grades/course-edition-student-grades.component';
import { CourseEditionTeacherGradesComponent } from '../course-edition-teacher-grades/course-edition-teacher-grades.component';

@Component({
  selector: 'app-course-edition-grades',
  standalone: true,
  imports: [NgIf, CourseEditionStudentGradesComponent, CourseEditionTeacherGradesComponent],
  templateUrl: './course-edition-grades.component.html',
})
export class CourseEditionGradesComponent {
  constructor(private authService: AuthService) {}

  authState = this.authService.authState;

  isStudent() {
    return this.authState().userData?.role === 0;
  }
}
