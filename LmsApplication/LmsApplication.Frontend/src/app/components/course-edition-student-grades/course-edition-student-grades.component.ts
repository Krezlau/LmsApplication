import { Component, OnDestroy, OnInit } from '@angular/core';
import { GradeService } from '../../services/grade.service';
import { Router } from '@angular/router';
import { AlertService } from '../../services/alert.service';
import {
    FinalGradeModel,
  GradeModel,
  UserGradeModel,
} from '../../types/course-board/grade-model';
import { Subscription, tap } from 'rxjs';
import { ApiResponse } from '../../types/api-response';
import { UserGradeListComponent } from '../../user-grade-list/user-grade-list.component';
import { NgClass, NgIf } from '@angular/common';

@Component({
  selector: 'app-course-edition-student-grades',
  standalone: true,
  imports: [NgClass, NgIf, UserGradeListComponent],
  templateUrl: './course-edition-student-grades.component.html',
})
export class CourseEditionStudentGradesComponent implements OnInit, OnDestroy {
  constructor(
    private gradeService: GradeService,
    private router: Router,
    private alertService: AlertService,
  ) {}

  sub = new Subscription();

  grades: GradeModel[] = [];
  gradesLoading = false;
  finalGrade: FinalGradeModel | null = null;

  ngOnInit(): void {
    this.gradesLoading = true;
    this.sub.add(
      this.gradeService
        .getUserGradesTable(this.router.url.split('/')[2], null)
        .pipe(
          tap({
            next: (response: ApiResponse<UserGradeModel>) => {
              this.gradesLoading = false;
              this.grades = response.data!.grades;
              this.finalGrade = response.data!.finalGrade;
            },
            error: (_) => {
              this.gradesLoading = false;
              this.alertService.show('Failed to load grades', 'error');
            },
          }),
        )
        .subscribe(),
    );
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
