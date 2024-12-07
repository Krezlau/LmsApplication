import {
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { GradesTableRowModel } from '../../types/course-board/grades-table-row-model';
import { DatePipe, NgIf } from '@angular/common';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { Subscription, tap } from 'rxjs';
import { AlertService } from '../../services/alert.service';
import { GradeService } from '../../services/grade.service';
import { Router } from '@angular/router';
import { ApiResponse } from '../../types/api-response';

@Component({
  selector: 'app-grade-row-definition-edit-modal',
  standalone: true,
  imports: [NgIf, ReactiveFormsModule],
  templateUrl: './grade-row-definition-edit-modal.component.html',
})
export class GradeRowDefinitionEditModalComponent implements OnInit, OnDestroy {
  @Output() edit = new EventEmitter<GradesTableRowModel>();
  @Input() row: GradesTableRowModel | null = null;
  @ViewChild('editModal') editModal: ElementRef | undefined;

  constructor(
    private alertService: AlertService,
    private gradeService: GradeService,
    private router: Router,
  ) {}

  onEditClicked() {
    this.editModal?.nativeElement.showModal();
  }

  datePipe = new DatePipe('en-US');

  titleControl = new FormControl('', [Validators.required]);
  descriptionControl = new FormControl('', []);
  dateControl = new FormControl<Date | null>(null, []);
  isSummedControl = new FormControl(false, []);

  sub = new Subscription();

  ngOnInit(): void {
    this.titleControl.setValue(this.row?.title ?? '');
    this.descriptionControl.setValue(this.row?.description ?? '');
    this.dateControl.setValue(this.row?.date ? this.row.date : null);
    this.isSummedControl.setValue(this.row?.isSummed ?? false);
  }

  onSubmit(event: Event) {
    event.preventDefault();

    if (!this.row) {
      return;
    }

    if (this.titleControl.invalid || !this.titleControl.value) {
      this.alertService.show('Title is required', 'error');
      return;
    }

    this.sub.add(
      this.gradeService
        .editRowDefinition(
          this.router.url.split('/')[2],
          this.row.id,
          this.titleControl.value,
          this.descriptionControl.value,
          this.dateControl.value ?? null,
          this.isSummedControl.value ?? false,
        )
        .pipe(
          tap({
            next: (response: ApiResponse<GradesTableRowModel>) => {
              this.edit.emit(response.data!);
              this.editModal?.nativeElement.close();
              this.titleControl.reset();
              this.descriptionControl.reset();
              this.dateControl.reset();
              this.isSummedControl.reset();
            },
            error: (error) => {
              if (error.error.message) {
                this.alertService.show(error.error.message, 'error');
              } else {
                this.alertService.show('Could not create row', 'error');
              }
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
