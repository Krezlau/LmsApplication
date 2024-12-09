import {
  Component,
  ElementRef,
  Input,
  OnDestroy,
  ViewChild,
} from '@angular/core';
import { UserModel } from '../../types/users/user-model';
import { GradeService } from '../../services/grade.service';
import { AlertService } from '../../services/alert.service';
import { Router } from '@angular/router';
import { Subscription, tap } from 'rxjs';
import {
  FinalGradeModel,
  GradeModel,
  UserGradeModel,
} from '../../types/course-board/grade-model';
import { ApiResponse } from '../../types/api-response';
import { NgClass, NgIf } from '@angular/common';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-add-final-grade-modal',
  standalone: true,
  imports: [NgIf, NgClass, ReactiveFormsModule],
  templateUrl: './add-final-grade-modal.component.html',
})
export class AddFinalGradeModalComponent implements OnDestroy {
  @Input() user: UserModel | null = null;
  @ViewChild('gradesModal') gradesModal: ElementRef | undefined;

  constructor(
    private gradeService: GradeService,
    private alertService: AlertService,
    private router: Router,
  ) {}

  sub = new Subscription();

  grades: GradeModel[] = [];
  finalGrade: FinalGradeModel | null = null;
  gradesLoading = false;

  finalGradeControl = new FormControl(2.0, Validators.required);

  getSum(): number {
    return this.grades.reduce(
      (acc, grade) =>
        acc + (grade.row.isSummed ? (grade.value?.value ?? 0) : 0),
      0,
    );
  }

  deleteFinalGrade(): void {
    if (!this.user || !this.finalGrade) return;

    this.sub.add(
      this.gradeService
        .deleteFinalGrade(
          this.router.url.split('/')[2],
          this.user.id,
        )
        .pipe(
          tap({
            next: (_) => {
              this.alertService.show('Final grade deleted', 'success');
              this.gradesModal?.nativeElement.close();
            },
            error: (_) => {
              this.alertService.show('Failed to delete final grade', 'error');
            },
          }),
        )
        .subscribe(),
    );
  }

  submitFinalGrade(event: Event): void {
    event.preventDefault();
    if (!this.user) return;

    if (this.finalGradeControl.invalid) {
      this.alertService.show('Please provide a final grade', 'error');
      return;
    }

    this.sub.add(
      this.gradeService
        .addFinalGrade(
          this.router.url.split('/')[2],
          this.user!.id,
          this.finalGradeControl.value!,
        )
        .pipe(
          tap({
            next: (_) => {
              this.alertService.show('Final grade added', 'success');
              this.gradesModal?.nativeElement.close();
            },
            error: (_) => {
              this.alertService.show('Failed to add final grade', 'error');
            },
          }),
        )
        .subscribe(),
    );
  }

  showGradesModal(): void {
    this.gradesModal?.nativeElement.showModal();
    this.gradesLoading = true;
    this.sub.add(
      this.gradeService
        .getUserGradesTable(this.router.url.split('/')[2], this.user!.id)
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
