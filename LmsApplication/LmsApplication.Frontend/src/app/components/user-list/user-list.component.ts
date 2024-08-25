import {Component, Input, OnDestroy, OnInit} from '@angular/core';
import {UserModel} from "../../types/users/user-model";
import {NgForOf, NgIf} from "@angular/common";
import {UserCardComponent} from "../user-card/user-card.component";
import {UserService} from "../../services/user.service";
import {Subscription} from "rxjs";

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
export class UserListComponent implements OnInit, OnDestroy{
  users: UserModel[] = [];

  sub = new Subscription();

  constructor(private userService: UserService) {
  }

  ngOnInit(): void {
    this.sub.add(this.userService.getUsers().subscribe(users => {
      this.users = users;
    }));
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
