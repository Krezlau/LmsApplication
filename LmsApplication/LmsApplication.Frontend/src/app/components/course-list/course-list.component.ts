import {Component, Input} from '@angular/core';
import {CourseModel} from "../../types/courses/course-model";
import {CourseAddFormComponent} from "../course-add-form/course-add-form.component";

@Component({
  selector: 'app-course-list',
  standalone: true,
  imports: [
    CourseAddFormComponent
  ],
  templateUrl: './course-list.component.html'
})
export class CourseListComponent {

  @Input() courses: CourseModel[] | null = null;
}
