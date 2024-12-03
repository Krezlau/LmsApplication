import { Component, OnInit, OnDestroy } from '@angular/core';
import { NgFor, NgIf } from '@angular/common';
import { GradeService } from '../../services/grade.service';
import { Subscription, tap } from 'rxjs';
import { UserModel } from '../../types/users/user-model';
import { GradesTableRowModel } from '../../types/course-board/grades-table-row-model';
import { Router } from '@angular/router';
import { ApiResponse } from '../../types/api-response';
import { AlertService } from '../../services/alert.service';
import { GradeRowDefinitionAddFormComponent } from '../grade-row-definition-add-form/grade-row-definition-add-form.component';
import { GradeRowDefinitionDetailsModalComponent } from '../grade-row-definition-details-modal/grade-row-definition-details-modal.component';
import { GradeRowDefinitionDeleteModalComponent } from '../grade-row-definition-delete-modal/grade-row-definition-delete-modal.component';

@Component({
  selector: 'app-course-edition-teacher-grades',
  standalone: true,
  imports: [
    NgFor,
    NgIf,
    GradeRowDefinitionAddFormComponent,
    GradeRowDefinitionDetailsModalComponent,
    GradeRowDefinitionDeleteModalComponent,
  ],
  templateUrl: './course-edition-teacher-grades.component.html',
})
export class CourseEditionTeacherGradesComponent implements OnInit, OnDestroy {
  constructor(
    private gradeService: GradeService,
    private router: Router,
    private alertService: AlertService,
  ) {}

  courseEditionId = '';

  rowDefinitions: GradesTableRowModel[] = [];
  rowDefinitionsLoading = false;

  students: UserModel[] = [];
  studentsLoading = false;

  sub = new Subscription();

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

  rowAdded(row: GradesTableRowModel) {
    this.rowDefinitions.push(row);
  }

  deleteRow(row: GradesTableRowModel) {
    this.sub.add(
      this.gradeService
        .deleteRowDefinition(this.courseEditionId, row.id)
        .pipe(
          tap({
            next: () => {
              this.rowDefinitions = this.rowDefinitions.filter(
                (r) => r.id !== row.id,
              );
            },
            error: (error) => {
              if (error.error.message) {
                this.alertService.show(error.error.message, 'error');
              } else {
                this.alertService.show('Could not delete row', 'error');
              }
            },
          }),
        )
        .subscribe(),
    );
  }

  ngOnInit(): void {
    this.courseEditionId = this.router.url.split('/')[2];
    this.rowDefinitionsLoading = true;
    this.sub.add(
      this.gradeService
        .getRowDefinitions(this.courseEditionId)
        .pipe(
          tap({
            next: (response: ApiResponse<GradesTableRowModel[]>) => {
              this.rowDefinitions = response.data!;
              this.rowDefinitionsLoading = false;
            },
            error: (error) => {
              if (error.error.message) {
                this.alertService.show(error.error.message, 'error');
              } else {
                this.alertService.show('Could not get rows', 'error');
              }
              this.rowDefinitionsLoading = false;
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
