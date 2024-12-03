import { NgIf } from '@angular/common';
import { Router } from '@angular/router';
import {
  Component,
  ElementRef,
  EventEmitter,
  OnDestroy,
  Output,
  ViewChild,
} from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { Subscription, tap } from 'rxjs';
import { GradeService } from '../../services/grade.service';
import { AlertService } from '../../services/alert.service';
import { GradesTableRowModel } from '../../types/course-board/grades-table-row-model';
import { ApiResponse } from '../../types/api-response';

@Component({
  selector: 'app-grade-row-definition-add-form',
  standalone: true,
  imports: [NgIf, ReactiveFormsModule],
  templateUrl: './grade-row-definition-add-form.component.html',
})
export class GradeRowDefinitionAddFormComponent implements OnDestroy {
  constructor(
    private gradeService: GradeService,
    private alertService: AlertService,
    private router: Router,
  ) {}

  @Output() added = new EventEmitter<GradesTableRowModel>();
  @ViewChild('addRowModal') addRowModal: ElementRef | undefined;

  titleControl = new FormControl('', [Validators.required]);
  descriptionControl = new FormControl('', []);
  dateControl = new FormControl<Date | null>(null, []);
  rowTypeControl = new FormControl(0, []);
  isSummedControl = new FormControl(false, []);

  sub = new Subscription();

  onSubmit(event: Event) {
    event.preventDefault();

    if (this.titleControl.invalid || !this.titleControl.value) {
      this.alertService.show('Title is required', 'error');
      return;
    }

    if (this.isSummedControl.value === true && this.rowTypeControl.value != 2) {
      this.alertService.show('Can only sum rows of type Number', 'error');
      return;
    }

    this.sub.add(
      this.gradeService
        .createRowDefinition(
          this.router.url.split('/')[2],
          this.titleControl.value,
          this.descriptionControl.value,
          this.dateControl.value ?? null,
          this.rowTypeControl.value ?? 0,
          this.isSummedControl.value ?? false,
        )
        .pipe(
          tap({
            next: (response: ApiResponse<GradesTableRowModel>) => {
              this.added.emit(response.data!);
              this.addRowModal?.nativeElement.close();
              this.titleControl.reset();
              this.descriptionControl.reset();
              this.dateControl.reset();
              this.rowTypeControl.reset();
              this.isSummedControl.reset();
            },
            error: (error) => {},
          }),
        )
        .subscribe(),
    );
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
