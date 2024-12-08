import { Component, Input } from '@angular/core';
import { GradeModel } from '../types/course-board/grade-model';
import { NgIf, NgFor } from '@angular/common';
import { UserGradeComponent } from '../components/user-grade/user-grade.component';
import { UserModel } from '../types/users/user-model';

@Component({
  selector: 'app-user-grade-list',
  standalone: true,
  imports: [NgIf, NgFor, UserGradeComponent],
  templateUrl: './user-grade-list.component.html',
})
export class UserGradeListComponent {
  @Input() grades: GradeModel[] = [];
  @Input() gradesLoading = false;
  @Input() user: UserModel | null = null;
  @Input() showSum = false;

  constructor() {}

  getSum(): number {
    return this.grades.reduce(
      (acc, grade) =>
        acc + (grade.row.isSummed ? (grade.value?.value ?? 0) : 0),
      0,
    );
  }
}
