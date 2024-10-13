import {Component, Input} from '@angular/core';
import {UserModel} from "../../types/users/user-model";
import {ApiResponse} from "../../types/api-response";

@Component({
  selector: 'app-user-profile',
  standalone: true,
  imports: [],
  templateUrl: './user-profile.component.html'
})
export class UserProfileComponent {
  @Input() user: ApiResponse<UserModel> | null = null;
}
