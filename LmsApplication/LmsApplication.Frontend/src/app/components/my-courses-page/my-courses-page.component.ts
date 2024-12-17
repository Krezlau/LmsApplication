import { Component, OnDestroy, OnInit } from '@angular/core';
import { CourseEditionService } from '../../services/course-edition.service';
import { CourseEditionModel } from '../../types/courses/course-edition-model';
import { Subscription } from 'rxjs';
import { ApiResponse } from '../../types/api-response';
import { CollectionResource } from '../../types/collection-resource';
import { NgClass, NgFor, NgIf } from '@angular/common';
import { CourseEditionStatusLabelComponent } from '../course-edition-status-label/course-edition-status-label.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-my-courses-page',
  standalone: true,
  imports: [NgIf, NgFor, NgClass, CourseEditionStatusLabelComponent],
  templateUrl: './my-courses-page.component.html',
})
export class MyCoursesPageComponent implements OnInit, OnDestroy {
  constructor(private courseEditionService: CourseEditionService, private router: Router) {}

  isLoading = false;
  courseEditions: CourseEditionModel[] = [];

  page = 0;
  pageSize = 25;
  nextPage = true;

  sub = new Subscription();

  navigateToEdition(edition: CourseEditionModel) {
    this.router.navigate(['editions', edition.id]);
  }

  ngOnInit(): void {
    this.isLoading = true;

    this.sub.add(
      this.courseEditionService
        .getMyCourseEditions(this.page + 1, this.pageSize)
        .subscribe(
          (data: ApiResponse<CollectionResource<CourseEditionModel>>) => {
            this.courseEditions = [...this.courseEditions, ...data.data!.items];
            this.page++;
            this.nextPage = data.data?.totalCount! > this.page * this.pageSize;
            this.isLoading = false;
          },
        ),
    );
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
