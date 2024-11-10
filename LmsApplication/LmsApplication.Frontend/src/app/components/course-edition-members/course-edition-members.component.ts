import { Component, OnInit } from '@angular/core';
import { UserListComponent } from '../user-list/user-list.component';
import { NgIf } from '@angular/common';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-course-edition-members',
  standalone: true,
  imports: [UserListComponent, NgIf],
  templateUrl: './course-edition-members.component.html',
})
export class CourseEditionMembersComponent implements OnInit {
  constructor(private router: Router) {}

  sub = new Subscription();
  courseEditionId: string | null = null;

  ngOnInit() {
    this.courseEditionId = this.router.url.split('/')[2];
  }
}
