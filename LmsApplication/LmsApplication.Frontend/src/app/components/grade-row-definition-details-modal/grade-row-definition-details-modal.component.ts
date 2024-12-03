import { Component, ElementRef, Input, ViewChild } from '@angular/core';
import { GradesTableRowModel } from '../../types/course-board/grades-table-row-model';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-grade-row-definition-details-modal',
  standalone: true,
  imports: [NgIf],
  templateUrl: './grade-row-definition-details-modal.component.html',
})
export class GradeRowDefinitionDetailsModalComponent {
  @Input() row: GradesTableRowModel | undefined;
  @ViewChild('detailsModal') detailsModal: ElementRef | undefined;

  onShowDetailsClicked() {
    this.detailsModal?.nativeElement.showModal();
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
}
