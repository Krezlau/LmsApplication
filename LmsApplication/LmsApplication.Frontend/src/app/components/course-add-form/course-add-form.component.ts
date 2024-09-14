import { Component } from '@angular/core';
import {FormControl, FormsModule, ReactiveFormsModule} from "@angular/forms";
import {CourseService} from "../../services/course.service";
import {CourseDuration} from "../../types/courses/course-duration";

@Component({
  selector: 'app-course-add-form',
  standalone: true,
  imports: [
    FormsModule,
    ReactiveFormsModule
  ],
  templateUrl: './course-add-form.component.html'
})
export class CourseAddFormComponent {

  titleFormControl = new FormControl('');
  descriptionFormControl = new FormControl('');
  durationFormControl = new FormControl(CourseDuration.OneSemester);

  constructor(private courseService: CourseService) {
  }

  onSubmit() {
    this.courseService.createCourse({
        title: this.titleFormControl.value!,
        description: this.descriptionFormControl.value!,
        duration: +this.durationFormControl.value!
      }).subscribe(() => {
      this.titleFormControl.setValue('');
      this.descriptionFormControl.setValue('');
    });
  }

  protected readonly CourseDuration = CourseDuration;
  protected readonly Number = Number;
}
