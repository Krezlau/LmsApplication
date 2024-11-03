import { Component, Input } from '@angular/core';
import { UserModel } from '../../types/users/user-model';
import { NgIf } from '@angular/common';
import { UserRole, toHumanReadable } from '../../types/users/user-role';
import { AuthService } from '../../services/auth.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-user-profile',
  standalone: true,
  imports: [NgIf, RouterLink],
  templateUrl: './user-profile.component.html',
})
export class UserProfileComponent {
  @Input() user: UserModel | null = null;

  constructor(private authService: AuthService) {}

  authState = this.authService.authState;

  toHumanReadable(role: UserRole) {
    return toHumanReadable(role);
  }

  isLoggedInUser() {
    return this.user?.id == (this.authState().userData?.id ?? '');
  }
}
