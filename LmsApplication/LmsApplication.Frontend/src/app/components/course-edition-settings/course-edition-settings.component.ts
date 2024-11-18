import { NgIf } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { CourseEditionSettingsService } from '../../services/course-edition-settings.service';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { UserRole } from '../../types/users/user-role';
import { Router } from '@angular/router';
import { Subscription, tap } from 'rxjs';
import { CourseEditionSettings } from '../../types/courses/course-edition-settings';
import { ApiResponse } from '../../types/api-response';
import { AlertService } from '../../services/alert.service';

@Component({
  selector: 'app-course-edition-settings',
  standalone: true,
  imports: [NgIf, ReactiveFormsModule],
  templateUrl: './course-edition-settings.component.html',
})
export class CourseEditionSettingsComponent implements OnInit, OnDestroy {
  constructor(
    private authService: AuthService,
    private settingService: CourseEditionSettingsService,
    private router: Router,
    private alertService: AlertService,
  ) {}

  authState = this.authService.authState;
  courseEditionId: string = '';

  settingsLoading = false;
  submitLoading = false;
  sub = new Subscription();
  allowAllToPostControl = new FormControl(false);

  editionSettingsSubmit(event: Event) {
    event.preventDefault();

    this.submitLoading = true;
    this.sub.add(
      this.settingService
        .updateCourseEditionSettings(
          this.courseEditionId,
          this.allowAllToPostControl.value ?? false,
        )
        .pipe(
          tap({
            next: () => {
              this.submitLoading = false;
              this.alertService.show('Settings updated', 'success');
            },
            error: (err) => {
              this.submitLoading = false;
              if (err.error.message) {
                this.alertService.show(err.error.message, 'error');
              } else {
                this.alertService.show('An error occurred', 'error');
              }
            },
          }),
        )
        .subscribe(),
    );
  }

  ngOnInit() {
    this.courseEditionId = this.router.url.split('/')[2];
    if (this.authState().userData?.role === UserRole.Student) {
      this.router.navigate(['/editions', this.courseEditionId]);
      return;
    }

    this.settingsLoading = true;
    this.sub.add(
      this.settingService
        .getCourseEdtionSettings(this.courseEditionId)
        .pipe(
          tap({
            next: (response: ApiResponse<CourseEditionSettings>) => {
              this.settingsLoading = false;
              this.allowAllToPostControl.setValue(
                response.data!.allowAllToPost,
              );
            },
            error: (err) => {
              this.settingsLoading = false;
              if (err.error.message) {
                this.alertService.show(err.error.message, 'error');
              } else {
                this.alertService.show('An error occurred', 'error');
              }
            },
          }),
        )
        .subscribe(),
    );
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }
}
