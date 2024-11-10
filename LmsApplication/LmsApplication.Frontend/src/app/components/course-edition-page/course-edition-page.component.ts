import { Component } from '@angular/core';
import { CourseEditionService } from '../../services/course-edition.service';
import { Subscription, tap } from 'rxjs';
import {
  ActivatedRoute,
  RouterLink,
  RouterLinkActive,
  RouterOutlet,
} from '@angular/router';
import { CourseEditionModel } from '../../types/courses/course-edition-model';
import { ApiResponse } from '../../types/api-response';
import { AlertService } from '../../services/alert.service';
import { NgIf } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { CourseEditionRegisterComponent } from '../course-edition-register/course-edition-register.component';

@Component({
  selector: 'app-course-edition-page',
  standalone: true,
  imports: [
    NgIf,
    RouterLink,
    RouterLinkActive,
    RouterOutlet,
    CourseEditionRegisterComponent,
  ],
  templateUrl: './course-edition-page.component.html',
})
export class CourseEditionPageComponent {
  constructor(
    private courseEditionService: CourseEditionService,
    private route: ActivatedRoute,
    private alertService: AlertService,
    private authService: AuthService,
  ) {}

  sub = new Subscription();
  edition: CourseEditionModel | null = null;
  editionLoading = false;
  authState = this.authService.authState;

  ngOnInit() {
    this.sub.add(
      this.route.params.subscribe((params) => {
        const editionId = params['editionId'];
        if (!editionId) {
          return;
        }

        this.editionLoading = true;
        this.sub.add(
          this.courseEditionService
            .getCourseEditionById(editionId)
            .pipe(
              tap({
                next: (response: ApiResponse<CourseEditionModel>) => {
                  this.edition = response.data;
                  this.editionLoading = false;
                },
                error: (error) => {
                  if (error?.error?.message) {
                    this.alertService.show(error.error.message, 'error');
                  } else {
                    this.alertService.show('An error occurred', 'error');
                  }
                  this.editionLoading = false;
                },
              }),
            )
            .subscribe(),
        );
      }),
    );
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }
}
