import { Component, effect } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { NgClass, NgIf } from '@angular/common';
import { UserRole } from '../../types/users/user-role';
import { Router, RouterLink } from '@angular/router';
import { SearchBarComponent } from '../search-bar/search-bar.component';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [NgClass, NgIf, RouterLink, SearchBarComponent],
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
        (this.authState().userData?.name[0].toUpperCase() ?? '') +
        (this.authState().userData?.surname[0].toUpperCase() ?? '');
    });
  }

  logoff() {
    this.router.navigate(['/login']);
    this.authService.logOut();
  }

  login() {
    this.router.navigate(['/login']);
  }

  protected readonly UserRole = UserRole;
}
