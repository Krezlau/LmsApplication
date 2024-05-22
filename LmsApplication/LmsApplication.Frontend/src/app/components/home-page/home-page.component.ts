import { Component } from '@angular/core';
import {AuthService} from "../../services/auth.service";
import {CourseEditionListComponent} from "../course-edition-list/course-edition-list.component";
import {CourseEditionService} from "../../services/course-edition.service";
import {AsyncPipe} from "@angular/common";

@Component({
  selector: 'app-home-page',
  standalone: true,
  imports: [
    CourseEditionListComponent,
    AsyncPipe
  ],
  templateUrl: './home-page.component.html'
})
export class HomePageComponent {

  authState = this.authService.authState;

  courseEditions$ = this.courseEditionService.getCourseEditions();

  constructor(private authService: AuthService, private courseEditionService: CourseEditionService) {
  }

}
