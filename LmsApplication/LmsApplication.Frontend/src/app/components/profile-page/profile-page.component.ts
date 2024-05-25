import {Component, OnDestroy, OnInit} from '@angular/core';
import {UserService} from "../../services/user.service";
import {Observable, Subscription} from "rxjs";
import {ActivatedRoute} from "@angular/router";
import {UserModel} from "../../types/users/user-model";
import {UserProfileComponent} from "../user-profile/user-profile.component";
import {AsyncPipe} from "@angular/common";

@Component({
  selector: 'app-profile-page',
  standalone: true,
  imports: [
    UserProfileComponent,
    AsyncPipe
  ],
  templateUrl: './profile-page.component.html'
})
export class ProfilePageComponent implements OnInit, OnDestroy {

  sub = new Subscription();
  user$: Observable<UserModel> | null  = null;

  constructor(private userService: UserService, private route: ActivatedRoute) { }

  async ngOnInit() {
    this.sub.add(this.route.params.subscribe(params => {
      this.user$ = this.userService.getUser(params['email']);
    }));
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }
}
