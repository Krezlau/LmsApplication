import {
  Component,
  ElementRef,
  Input,
  OnDestroy,
  ViewChild,
} from '@angular/core';
import { GradesTableRowModel } from '../../types/course-board/grades-table-row-model';
import { NgFor, NgIf } from '@angular/common';
import { Router } from '@angular/router';
import { GradeService } from '../../services/grade.service';
import { Subscription, tap } from 'rxjs';
import { UserGradesModel } from '../../types/course-board/user-grades-model';
import { UserGradeComponent } from '../user-grade/user-grade.component';

@Component({
  selector: 'app-grade-row-definition-details-modal',
  standalone: true,
  imports: [NgIf, NgFor, UserGradeComponent],
  templateUrl: './grade-row-definition-details-modal.component.html',
})
export class GradeRowDefinitionDetailsModalComponent implements OnDestroy {
  @Input() row: GradesTableRowModel | undefined;
  @ViewChild('detailsModal') detailsModal: ElementRef | undefined;

  constructor(
    private router: Router,
    private gradeService: GradeService,
  ) {}

  sub = new Subscription();
  userGrades: UserGradesModel | null = null;
  gradesLoading = false;

  onShowDetailsClicked() {
    this.detailsModal?.nativeElement.showModal();
    console.log('lmao');
    this.gradesLoading = true;
    this.sub.add(
      this.gradeService
        .getUserGrades(this.router.url.split('/')[2], this.row!.id)
        .pipe(
          tap({
            next: (data) => {
              this.userGrades = data.data;
              this.gradesLoading = false;
            },
            error: () => {
              this.gradesLoading = false;
            },
          }),
        )
        .subscribe(),
    );
  }

  getRowDate(row: GradesTableRowModel) {
    return row.date ? new Date(row.date).toLocaleDateString() : '';
  }

  getRowType(row: GradesTableRowModel) {
    switch (row.rowType) {
      case 1:
        return 'Text';
      case 2:
        return 'Number';
      case 3:
        return 'Yes/No';
      default:
        return 'None';
    }
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
