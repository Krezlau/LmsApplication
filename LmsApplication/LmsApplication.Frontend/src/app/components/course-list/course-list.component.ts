import {Component, Input} from '@angular/core';
import {CourseModel} from "../../types/courses/course-model";

@Component({
  selector: 'app-course-list',
  standalone: true,
  imports: [],
  templateUrl: './course-list.component.html'
})
export class CourseListComponent {

  @Input() courses: CourseModel[] | null = null;
}
