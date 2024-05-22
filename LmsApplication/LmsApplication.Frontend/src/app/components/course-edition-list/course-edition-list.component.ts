import { Component, Input } from '@angular/core';
import {CourseEditionModel} from "../../types/courses/course-edition-model";

@Component({
  selector: 'app-course-edition-list',
  standalone: true,
  imports: [],
  templateUrl: './course-edition-list.component.html'
})
export class CourseEditionListComponent {
  @Input() courseEditions: CourseEditionModel[] | null = [];

  constructor() {
  }
}
