import {Component, Input, OnDestroy, OnInit} from '@angular/core';
import {CourseEditionModel} from "../../types/courses/course-edition-model";
import {CourseEditionService} from "../../services/course-edition.service";
import {Subscription} from "rxjs";

@Component({
  selector: 'app-course-edition-list',
  standalone: true,
  imports: [],
  templateUrl: './course-edition-list.component.html'
})
export class CourseEditionListComponent implements OnInit, OnDestroy {
  courseEditions: CourseEditionModel[] = [];

  sub = new Subscription();

  constructor(private courseEditionService: CourseEditionService) {
  }

  redirectToDetails(courseEditionId: string) {}

  ngOnInit() {
    this.sub.add(this.courseEditionService.getCourseEditions().subscribe(
      courseEditions => this.courseEditions = courseEditions
    ));
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }
}
