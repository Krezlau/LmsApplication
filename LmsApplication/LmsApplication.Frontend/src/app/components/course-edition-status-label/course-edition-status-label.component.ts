import { Component, Input } from '@angular/core';
import { CourseEditionModel, CourseEditionStatus } from '../../types/courses/course-edition-model';
import { NgClass, NgIf } from '@angular/common';

@Component({
  selector: 'app-course-edition-status-label',
  standalone: true,
  imports: [NgIf, NgClass],
  templateUrl: './course-edition-status-label.component.html'
})
export class CourseEditionStatusLabelComponent {
  @Input() edition: CourseEditionModel | null = null;

  toHumanReadable(status: CourseEditionStatus) {
    switch (status) {
      case CourseEditionStatus.Planned:
        return 'Planned';
      case CourseEditionStatus.RegistrationOpen:
        return 'Registration open';
      case CourseEditionStatus.RegistrationClosed:
        return 'Registration closed';
      case CourseEditionStatus.InProgress:
        return 'In progress';
      case CourseEditionStatus.Finished:
        return 'Finished';
    }
  }
}
