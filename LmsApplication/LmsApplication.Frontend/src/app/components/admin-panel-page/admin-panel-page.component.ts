import { Component } from '@angular/core';
import {CourseEditionListComponent} from "../course-edition-list/course-edition-list.component";
import {CourseListComponent} from "../course-list/course-list.component";
import {UserListComponent} from "../user-list/user-list.component";
import {UserService} from "../../services/user.service";
import {CourseService} from "../../services/course.service";
import {AsyncPipe} from "@angular/common";
import {CourseAddFormComponent} from "../course-add-form/course-add-form.component";

@Component({
  selector: 'app-admin-panel-page',
  standalone: true,
  imports: [
    CourseEditionListComponent,
    CourseListComponent,
    UserListComponent,
    AsyncPipe,
    CourseAddFormComponent
  ],
  templateUrl: './admin-panel-page.component.html'
})
export class AdminPanelPageComponent {

  users$ = this.userService.getUsers();
  courses$ = this.courseService.getAllCourses();

  constructor(private userService: UserService, private courseService: CourseService) { }
}
