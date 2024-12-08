import { NgClass, NgIf } from '@angular/common';
import {
  Component,
  Input,
  OnInit,
  ViewChild,
  ElementRef,
  Output,
  EventEmitter,
} from '@angular/core';
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
import { CourseEditionModel } from '../../types/courses/course-edition-model';
import { ApiResponse } from '../../types/api-response';

@Component({
  selector: 'app-course-edition-add-form',
  standalone: true,
  imports: [NgIf, ReactiveFormsModule, FormsModule, NgClass],
  templateUrl: './course-edition-add-form.component.html',
})
export class CourseEditionAddFormComponent implements OnInit {
  @Input() course: CourseModel = {} as CourseModel;
  @Output() added = new EventEmitter<CourseEditionModel>();

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
  openRegistrationChecked = false;
  registrationStartDateControl = new FormControl(null);
  registrationEndDateControl = new FormControl(null);

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

    if (this.openRegistrationChecked) {
      if (
        !this.registrationStartDateControl.value ||
        !this.registrationEndDateControl.value
      ) {
        this.alertService.show(
          'Registration start and end dates are required.',
          'error',
        );
        return;
      }

      if (
        new Date(this.registrationStartDateControl.value!) >
        new Date(this.registrationEndDateControl.value!)
      ) {
        this.alertService.show(
          'Registration start date must be before registration end date.',
          'error',
        );
        return;
      }

      if (
        new Date(this.registrationEndDateControl.value!) >
        new Date(this.startDateControl.value!)
      ) {
        this.alertService.show(
          'Registration must end before the course edition starts.',
          'error',
        );
        return;
      }
    }

    this.createLoading = true;
    this.sub.add(
      this.courseEditionService
        .createCourseEdition(
          this.course.id,
          this.titleControl.value!,
          this.studentLimitControl.value!,
          new Date(this.startDateControl.value!),
          this.openRegistrationChecked
            ? new Date(this.registrationStartDateControl.value!)
            : null,
          this.openRegistrationChecked
            ? new Date(this.registrationEndDateControl.value!)
            : null,
        )
        .pipe(
          tap({
            next: (response: ApiResponse<CourseEditionModel>) => {
              this.titleControl.reset();
              this.studentLimitControl.reset();
              this.startDateControl.reset();
              this.dialogElement?.nativeElement.close();
              this.createLoading = false;
              this.added.emit(response.data!);
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
