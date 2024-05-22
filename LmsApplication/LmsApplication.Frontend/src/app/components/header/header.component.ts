import {Component, effect} from '@angular/core';
import {AuthService} from "../../services/auth.service";
import {NgClass, NgIf} from "@angular/common";

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [
    NgClass,
    NgIf
  ],
  templateUrl: './header.component.html'
})
export class HeaderComponent {

  authState = this.authService.authState;

  letterAvatar = '';

  constructor(private authService: AuthService) {
    effect(() => {
      this.letterAvatar = this.authState().userData?.name[0].toUpperCase() ?? '';
    });
  }

  logoff() {
    this.authService.logoff();
  }

  login() {
    this.authService.authorize();
  }
}
