import { NgClass, NgIf } from '@angular/common';
import { Component, Input, OnDestroy } from '@angular/core';
import {
  GradesTableRowValueModel,
  UserGradesTableRowValueModel,
} from '../../types/course-board/user-grades-model';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { GradeService } from '../../services/grade.service';
import { AlertService } from '../../services/alert.service';
import { Router } from '@angular/router';
import { Subscription, tap } from 'rxjs';
import { GradesTableRowModel } from '../../types/course-board/grades-table-row-model';
import { ApiResponse } from '../../types/api-response';
import { UserModel } from '../../types/users/user-model';

@Component({
  selector: 'app-user-grade',
  standalone: true,
  imports: [NgIf, ReactiveFormsModule, NgClass],
  templateUrl: './user-grade.component.html',
})
export class UserGradeComponent implements OnDestroy {
  @Input() grade:
    | UserGradesTableRowValueModel
    | GradesTableRowValueModel
    | null = null;
  @Input() row: GradesTableRowModel | null = null;
  @Input() student: UserModel | null = null;
  @Input() type: 'student' | 'row' = 'row';

  constructor(
    private gradeService: GradeService,
    private alertService: AlertService,
    private router: Router,
  ) {}

  editingEnabled = false;
  saveLoading = false;
  deleteLoading = false;
  commentVisible = false;
  descriptionVisible = false;

  gradeControl = new FormControl('', [Validators.required]);
  commentControl = new FormControl('');

  sub = new Subscription();

  toggleDescriptionVisible() {
    this.descriptionVisible = !this.descriptionVisible;
  }

  toggleCommentVisible() {
    this.commentVisible = !this.commentVisible;
  }

  enableEditing() {
    this.editingEnabled = true;
    this.gradeControl.setValue(this.grade?.value);
    this.commentControl.setValue(this.grade?.teacherComment ?? '');
  }

  disableEditing() {
    this.editingEnabled = false;
    this.gradeControl.reset();
    this.commentControl.reset();
  }

  getFirstColumnValue() {
    if (this.type === 'student') {
      return this.student?.name + ' ' + this.student?.surname;
    } else {
      return this.row?.title;
    }
  }

  getGradeValue() {
    if (this.row?.rowType === 3) {
      return this.grade?.value === null || this.grade?.value === undefined
        ? ''
        : this.grade?.value
          ? 'Yes'
          : 'No';
    }

    return this.grade?.value;
  }

  deleteGrade() {
    if (!this.row || !this.grade) return;

    this.deleteLoading = true;
    this.sub.add(
      this.gradeService
        .deleteGrade(
          this.router.url.split('/')[2],
          this.row!.id,
          this.student!.id,
        )
        .pipe(
          tap({
            next: (_: ApiResponse<null>) => {
              this.deleteLoading = false;
              this.grade!.id = null;
              this.grade!.value = null;
              this.grade!.teacher = null;
              this.grade!.teacherComment = null;
            },
            error: (err) => {
              if (err.error.message) {
                this.alertService.show(err.error.message, 'error');
              } else {
                this.alertService.show('Something went wrong', 'error');
              }
              this.deleteLoading = false;
            },
          }),
        )
        .subscribe(),
    );
  }

  saveGrade() {
    console.log(this.grade);
    if (!this.grade || !this.row) return;

    if (this.gradeControl.invalid || !this.gradeControl.value) {
      this.alertService.show('Grade is required', 'error');
      return;
    }
    this.saveLoading = true;

    this.sub.add(
      this.gradeService
        .updateGrade(
          this.router.url.split('/')[2],
          this.row.id,
          this.student!.id,
          this.gradeControl.value,
          this.commentControl.value?.length === 0
            ? null
            : this.commentControl.value,
        )
        .pipe(
          tap({
            next: (response: ApiResponse<GradesTableRowValueModel>) => {
              this.alertService.show('Grade saved', 'success');
              this.saveLoading = false;
              this.editingEnabled = false;
              this.grade!.id = response.data!.id;
              this.grade!.value = response.data!.value;
              this.grade!.teacher = response.data!.teacher;
              this.grade!.teacherComment = response.data!.teacherComment;
            },
            error: (err) => {
              if (err.error.message) {
                this.alertService.show(err.error.message, 'error');
              } else {
                this.alertService.show('Something went wrong', 'error');
              }
              this.saveLoading = false;
            },
          }),
        )
        .subscribe(),
    );
  }

  ngOnDestroy() {
    this.gradeControl.reset();
    this.commentControl.reset();
    this.sub.unsubscribe();
  }
}
