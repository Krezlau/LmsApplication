import { Component, effect } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { NgClass, NgIf } from '@angular/common';
import { UserRole } from '../../types/users/user-role';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [NgClass, NgIf, RouterLink],
  templateUrl: './header.component.html',
})
export class HeaderComponent {
  authState = this.authService.authState;

  letterAvatar = '';

  constructor(
    private authService: AuthService,
    private router: Router,
  ) {
    effect(() => {
      this.letterAvatar =
        this.authState().userData?.name[0].toUpperCase() ?? '';
    });
    console.log(this.authState().userData)
  }

  logoff() {
    this.authService.logOut();
  }

  login() {
    this.router.navigate(['/login']);
  }

  protected readonly UserRole = UserRole;
}
