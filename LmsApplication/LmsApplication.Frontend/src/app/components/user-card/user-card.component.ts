import {Component, Input} from '@angular/core';
import {UserModel} from "../../types/users/user-model";
import {NgClass, NgIf} from "@angular/common";
import {toHumanReadable} from "../../types/users/user-role";
import {Router, RouterLink} from "@angular/router";
import {PrivateExportAliasingHost} from "@angular/compiler-cli/src/ngtsc/imports";
import {AuthService} from "../../services/auth.service";

@Component({
  selector: 'app-user-card',
  standalone: true,
  imports: [
    NgIf,
    NgClass,
    RouterLink
  ],
  templateUrl: './user-card.component.html'
})
export class UserCardComponent {
  @Input() user: UserModel | null = null;
  protected readonly toHumanReadable = toHumanReadable;

  authState = this.authService.authState;
  constructor(private authService: AuthService, private router: Router) {
  }

  async onUserClick() {
    if (this.user) {
      await this.router.navigate([this.authState().tenantId, 'users', this.user.email]);
    }
  }

  protected readonly Option = Option;
}
