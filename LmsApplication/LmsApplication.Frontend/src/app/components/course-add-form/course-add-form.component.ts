import { Component } from '@angular/core';
import {FormControl, FormsModule, ReactiveFormsModule} from "@angular/forms";
import {CourseService} from "../../services/course.service";

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

  constructor(private courseService: CourseService) {
  }

  onSubmit() {
    this.courseService.createCourse({
        title: this.titleFormControl.value!,
        description: this.descriptionFormControl.value!
      }).subscribe(() => {
      this.titleFormControl.setValue('');
      this.descriptionFormControl.setValue('');
    });
  }
}
