import { Component } from '@angular/core';
import {AuthService} from "../../services/auth.service";
import {CourseEditionListComponent} from "../course-edition-list/course-edition-list.component";
import {AsyncPipe, NgIf} from "@angular/common";

@Component({
  selector: 'app-home-page',
  standalone: true,
  imports: [
    CourseEditionListComponent,
    AsyncPipe,
    NgIf
  ],
  templateUrl: './home-page.component.html'
})
export class HomePageComponent {

  authState = this.authService.authState;
  constructor(private authService: AuthService) {
  }

}
