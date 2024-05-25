import {Component, Input} from '@angular/core';
import {UserModel} from "../../types/users/user-model";
import {NgClass, NgIf} from "@angular/common";
import {toHumanReadable} from "../../types/users/user-role";

@Component({
  selector: 'app-user-card',
  standalone: true,
  imports: [
    NgIf,
    NgClass
  ],
  templateUrl: './user-card.component.html'
})
export class UserCardComponent {

  @Input() user: UserModel | null = null;
  protected readonly toHumanReadable = toHumanReadable;
}
