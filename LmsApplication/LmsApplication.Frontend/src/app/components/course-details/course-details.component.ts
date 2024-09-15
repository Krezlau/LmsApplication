import {Component, Input} from '@angular/core';
import {CourseModel} from "../../types/courses/course-model";
import {NgIf} from "@angular/common";

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
}
