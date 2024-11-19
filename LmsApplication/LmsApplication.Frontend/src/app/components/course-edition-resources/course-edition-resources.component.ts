import { Component, OnInit } from '@angular/core';
import { CourseResourcesListComponent } from '../course-resources-list/course-resources-list.component';
import { Router } from '@angular/router';
import { CourseEditionModel } from '../../types/courses/course-edition-model';
import { CourseEditionService } from '../../services/course-edition.service';
import { Subscription } from 'rxjs';
import { ApiResponse } from '../../types/api-response';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-course-edition-resources',
  standalone: true,
  imports: [NgIf, CourseResourcesListComponent],
  templateUrl: './course-edition-resources.component.html',
})
export class CourseEditionResourcesComponent implements OnInit {
  constructor(
    private router: Router,
    private courseEditionService: CourseEditionService,
  ) {}

  editionId: string = '';
  courseId: string = '';

  sub = new Subscription();

  ngOnInit(): void {
    this.editionId = this.router.url.split('/')[2];
    this.sub.add(
      this.courseEditionService
        .getCourseEditionById(this.editionId)
        .subscribe((edition: ApiResponse<CourseEditionModel>) => {
          if (edition.data?.course) this.courseId = edition.data?.course.id;
          console.log('courseId', this.courseId);
        }),
    );
  }
}
