import { Component, OnDestroy } from '@angular/core';
import { CourseEditionService } from '../../services/course-edition.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription, tap } from 'rxjs';
import { ApiResponse } from '../../types/api-response';
import { AlertService } from '../../services/alert.service';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-course-edition-register',
  standalone: true,
  imports: [NgIf],
  templateUrl: './course-edition-register.component.html',
})
export class CourseEditionRegisterComponent implements OnDestroy {
  constructor(
    private courseEditionService: CourseEditionService,
    private route: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
  ) {}

  loading = false;
  sub = new Subscription();

  registerForCourseEdition() {
    var editionId = this.route.snapshot.params['editionId'];
    if (!editionId) return;

    this.loading = true;
    this.sub.add(
      this.courseEditionService
        .registerForCourseEdition(editionId)
        .pipe(
          tap({
            next: (_: ApiResponse<null>) => {
              this.loading = false;
              this.alertService.show(
                'Successfully registered for course edition',
                'success',
              );
              this.router
                .navigateByUrl('/', { skipLocationChange: true })
                .then(() => {
                  this.router.navigate(['/editions', editionId, 'board']);
                });
            },
            error: (error: any) => {
              this.loading = false;
              if (error?.error?.message) {
                this.alertService.show(error.error.message, 'error');
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
