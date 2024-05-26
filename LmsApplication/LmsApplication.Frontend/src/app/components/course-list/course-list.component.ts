import {Component, Input} from '@angular/core';
import {CourseModel} from "../../types/courses/course-model";
import {CourseAddFormComponent} from "../course-add-form/course-add-form.component";
import {NgForOf} from "@angular/common";
import {AuthService} from "../../services/auth.service";
import {Router} from "@angular/router";

@Component({
  selector: 'app-course-list',
  standalone: true,
  imports: [
    CourseAddFormComponent,
    NgForOf
  ],
  templateUrl: './course-list.component.html'
})
export class CourseListComponent {

  @Input() courses: CourseModel[] | null = null;

  authState = this.authService.authState;
  constructor(private authService: AuthService, private router: Router) {
  }

  async redirectToDetails(courseId: string) {
    await this.router.navigate([this.authState().tenantId, 'courses', courseId]);
  }
}
