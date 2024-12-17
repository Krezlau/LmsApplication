import { Component, OnDestroy } from '@angular/core';
import { CourseModel } from '../../types/courses/course-model';
import { CourseService } from '../../services/course.service';
import { Subscription, tap } from 'rxjs';
import { ApiResponse } from '../../types/api-response';
import { CollectionResource } from '../../types/collection-resource';
import { NgClass, NgFor, NgIf } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-search-bar',
  standalone: true,
  imports: [NgIf, NgFor, NgClass],
  templateUrl: './search-bar.component.html',
})
export class SearchBarComponent implements OnDestroy {
  constructor(private courseService: CourseService, private router: Router) {}

  sub = new Subscription();

  isLoading = false;
  results: CourseModel[] = [];
  input = '';

  updateInput(event: any) {
    if (event.target.value.length < 2) return;

    this.isLoading = true;
    this.results = [];

    this.sub.add(
      this.courseService
        .searchCourses(event.target.value)
        .pipe(
          tap({
            next: (res: ApiResponse<CollectionResource<CourseModel>>) => {
              this.isLoading = false;
              this.results = res.data!.items;
            },
            error: (err) => {
              this.isLoading = false;
            },
          }),
        )
        .subscribe(),
    );
  }

  resetAndNavigate(courseId: string) {
    this.results = [];
    this.isLoading = false;
    this.input = '';
    this.router.navigate(['/courses', courseId]);
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
