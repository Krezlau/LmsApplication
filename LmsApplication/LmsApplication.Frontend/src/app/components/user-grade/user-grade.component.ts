import { NgClass, NgIf } from '@angular/common';
import { Component, Input, OnDestroy } from '@angular/core';
import { UserGradesTableRowValueModel } from '../../types/course-board/user-grades-model';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { GradeService } from '../../services/grade.service';
import { AlertService } from '../../services/alert.service';
import { Router } from '@angular/router';
import { Subscription, tap } from 'rxjs';
import { GradesTableRowModel } from '../../types/course-board/grades-table-row-model';
import { ApiResponse } from '../../types/api-response';

@Component({
  selector: 'app-user-grade',
  standalone: true,
  imports: [NgIf, ReactiveFormsModule, NgClass],
  templateUrl: './user-grade.component.html',
})
export class UserGradeComponent implements OnDestroy {
  @Input() grade: UserGradesTableRowValueModel | null = null;
  @Input() row: GradesTableRowModel | null = null;

  constructor(
    private gradeService: GradeService,
    private alertService: AlertService,
    private router: Router,
  ) {}

  editingEnabled = false;
  saveLoading = false;
  deleteLoading = false;
  commentVisible = false;

  gradeControl = new FormControl('', [Validators.required]);
  commentControl = new FormControl('');

  sub = new Subscription();

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

  getGradeValue() {
    if (this.row?.rowType === 3) {
      return this.grade?.value === true
        ? 'Yes'
        : this.grade?.value === false
          ? 'No'
          : '';
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
          this.grade!.student.id,
        )
        .pipe(tap({
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
          }
        }))
        .subscribe(),
    );
  }

  saveGrade() {
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
          this.grade.student.id,
          this.gradeControl.value,
          this.commentControl.value?.length === 0
            ? null
            : this.commentControl.value,
        )
        .pipe(
          tap({
            next: (_: ApiResponse<null>) => {
              this.alertService.show('Grade saved', 'success');
              this.saveLoading = false;
              this.editingEnabled = false;
              this.grade!.value = this.gradeControl.value;
              this.grade!.teacherComment = this.commentControl.value;
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
