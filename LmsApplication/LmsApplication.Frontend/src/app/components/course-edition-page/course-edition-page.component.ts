import { Component } from '@angular/core';
import { CourseEditionService } from '../../services/course-edition.service';
import { Subscription, tap } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { CourseEditionModel } from '../../types/courses/course-edition-model';
import { ApiResponse } from '../../types/api-response';
import { AlertService } from '../../services/alert.service';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-course-edition-page',
  standalone: true,
  imports: [NgIf],
  templateUrl: './course-edition-page.component.html',
})
export class CourseEditionPageComponent {
  constructor(
    private courseEditionService: CourseEditionService,
    private route: ActivatedRoute,
    private alertService: AlertService,
  ) {}

  sub = new Subscription();
  edition: CourseEditionModel | null = null;
  editionLoading = false;

  ngOnInit() {
    this.sub = this.route.params.subscribe((params) => {
      const editionId = params['editionId'];
      if (!editionId) {
        return;
      }

      this.editionLoading = true;
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
        .subscribe();
    });
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }
}
