import {Component, Input} from '@angular/core';
import {CourseModel} from "../../types/courses/course-model";

@Component({
  selector: 'app-course-details',
  standalone: true,
  imports: [],
  templateUrl: './course-details.component.html'
})
export class CourseDetailsComponent {
  @Input() course: CourseModel | null = null;
}
