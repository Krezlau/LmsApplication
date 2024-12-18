import { Component } from '@angular/core';
import { Subscription, tap } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { CourseEditionService } from '../../services/course-edition.service';
import { CourseEditionModel } from '../../types/courses/course-edition-model';
import { ApiResponse } from '../../types/api-response';
import { AlertService } from '../../services/alert.service';
import { NgFor, NgIf } from '@angular/common';
import { DatePipe } from '@angular/common';
import { CourseEditionStatusLabelComponent } from '../course-edition-status-label/course-edition-status-label.component';

@Component({
  selector: 'app-course-edition-overview',
  standalone: true,
  imports: [NgIf, NgFor, DatePipe, CourseEditionStatusLabelComponent],
  templateUrl: './course-edition-overview.component.html',
})
export class CourseEditionOverviewComponent {
  constructor(
    private courseEditionService: CourseEditionService,
    private route: ActivatedRoute,
    private alertService: AlertService,
  ) {}

  sub = new Subscription();
  edition: CourseEditionModel | null = null;
  editionLoading = false;

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
