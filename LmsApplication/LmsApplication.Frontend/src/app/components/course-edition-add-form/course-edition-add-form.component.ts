import { NgIf } from '@angular/common';
import { Component, Input, OnInit, ViewChild, ElementRef } from '@angular/core';
import {
  FormControl,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { CourseModel } from '../../types/courses/course-model';
import { AlertService } from '../../services/alert.service';
import { CourseEditionService } from '../../services/course-edition.service';
import { Subscription, tap } from 'rxjs';

@Component({
  selector: 'app-course-edition-add-form',
  standalone: true,
  imports: [NgIf, ReactiveFormsModule, FormsModule],
  templateUrl: './course-edition-add-form.component.html',
})
export class CourseEditionAddFormComponent implements OnInit {
  @Input() course: CourseModel = {} as CourseModel;

  constructor(
    private alertService: AlertService,
    private courseEditionService: CourseEditionService,
  ) {}

  createLoading = false;
  sub = new Subscription();
  @ViewChild('dialogElement') dialogElement: ElementRef | undefined;

  studentLimitControl = new FormControl(20, [
    Validators.required,
    Validators.min(1),
  ]);
  titleControl = new FormControl(this.course.title, [Validators.required]);
  startDateControl = new FormControl(new Date(), [Validators.required]);

  onSubmit() {
    if (this.titleControl.invalid) {
      this.alertService.show('Title is required.', 'error');
      return;
    }
    if (this.studentLimitControl.invalid) {
      this.alertService.show(
        'Student limit is required and must be greater than 0.',
        'error',
      );
      return;
    }
    if (this.startDateControl.invalid) {
      this.alertService.show('Start date is required.', 'error');
      return;
    }
    if (new Date(this.startDateControl.value!) < new Date()) {
      this.alertService.show('Start date must be in the future.', 'error');
      return;
    }

    this.createLoading = true;
    this.sub.add(
      this.courseEditionService
        .createCourseEdition(
          this.course.id,
          this.titleControl.value!,
          this.studentLimitControl.value!,
          new Date(this.startDateControl.value!),
        )
        .pipe(
          tap({
            next: () => {
              this.titleControl.reset();
              this.studentLimitControl.reset();
              this.startDateControl.reset();
              this.dialogElement?.nativeElement.close();
              this.createLoading = false;
            },
            error: (error) => {
              if (error.error.message) {
                this.alertService.show(error.error.message, 'error');
              } else {
                this.alertService.show(
                  'Could not create a course editon.',
                  'error',
                );
              }
              this.createLoading = false;
            },
          }),
        )
        .subscribe(),
    );
  }

  ngOnInit(): void {
    this.titleControl.setValue(
      `${this.course.title.toLowerCase().replace(' ', '_')}:${new Date().toLocaleDateString('en-GB')}`,
    );
  }
}
