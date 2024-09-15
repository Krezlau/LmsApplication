import {Component, OnDestroy, OnInit} from '@angular/core';
import {CourseEditionService} from "../../services/course-edition.service";
import {CourseService} from "../../services/course.service";
import {ActivatedRoute} from "@angular/router";
import {Observable, Subscription} from "rxjs";
import {CourseModel} from "../../types/courses/course-model";
import {CourseEditionModel} from "../../types/courses/course-edition-model";
import {CourseDetailsComponent} from "../course-details/course-details.component";
import {CourseEditionListComponent} from "../course-edition-list/course-edition-list.component";
import {AsyncPipe} from "@angular/common";

@Component({
  selector: 'app-course-details-page',
  standalone: true,
  imports: [
    CourseDetailsComponent,
    CourseEditionListComponent,
    AsyncPipe
  ],
  templateUrl: './course-details-page.component.html'
})
export class CourseDetailsPageComponent implements OnInit, OnDestroy {

  course$: Observable<CourseModel> | null = null;
  courseEditions$: Observable<CourseEditionModel[]> | null = null;

  sub = new Subscription();
  constructor(
    private courseService: CourseService,
    private courseEditionService: CourseEditionService,
    private route: ActivatedRoute) {
  }

  ngOnInit() {
    this.sub = this.route.params.subscribe(params => {
      this.course$ = this.courseService.getCourseById(params['courseId']);
      this.courseEditions$ = this.courseEditionService.getCourseEditionsByCourseId(params['courseId']);
    });
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }
}
