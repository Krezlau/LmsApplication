import {Component, Input} from '@angular/core';
import {UserModel} from "../../types/users/user-model";
import {NgForOf, NgIf} from "@angular/common";
import {UserCardComponent} from "../user-card/user-card.component";

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [
    NgForOf,
    NgIf,
    UserCardComponent
  ],
  templateUrl: './user-list.component.html'
})
export class UserListComponent {
  @Input() users: UserModel[] | null = null;
}
