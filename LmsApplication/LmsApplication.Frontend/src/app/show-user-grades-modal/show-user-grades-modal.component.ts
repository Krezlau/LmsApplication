import { NgIf, NgClass } from '@angular/common';
import {
  Component,
  ElementRef,
  Input,
  OnDestroy,
  ViewChild,
} from '@angular/core';
import { UserGradeListComponent } from '../user-grade-list/user-grade-list.component';
import { FinalGradeModel, GradeModel, UserGradeModel } from '../types/course-board/grade-model';
import { GradeService } from '../services/grade.service';
import { AlertService } from '../services/alert.service';
import { ApiResponse } from '../types/api-response';
import { Router } from '@angular/router';
import { Subscription, tap } from 'rxjs';
import { UserModel } from '../types/users/user-model';

@Component({
  selector: 'app-show-user-grades-modal',
  standalone: true,
  imports: [NgClass, NgIf, UserGradeListComponent],
  templateUrl: './show-user-grades-modal.component.html',
})
export class ShowUserGradesModalComponent implements OnDestroy {
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
