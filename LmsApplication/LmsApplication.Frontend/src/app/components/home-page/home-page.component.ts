import { Component } from '@angular/core';
import {AuthService} from "../../services/auth.service";
import {UserService} from "../../services/user.service";

@Component({
  selector: 'app-home-page',
  standalone: true,
  imports: [],
  templateUrl: './home-page.component.html'
})
export class HomePageComponent {

  constructor(private authService: AuthService, private userService: UserService) {
  }

  login() {
    this.authService.authorize();
  }

  logout() {
    this.authService.logoff();
  }

  getMe() {
    this.userService.getMe().subscribe((data) => {
      console.log("data: ", data);
    });
  }

  getUsers() {
    this.userService.getUsers().subscribe((data) => {
      console.log("data: ", data);
    });
  }
}
