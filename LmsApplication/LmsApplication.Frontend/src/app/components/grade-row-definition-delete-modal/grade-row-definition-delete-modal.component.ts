import { NgClass } from '@angular/common';
import {
  Component,
  ElementRef,
  EventEmitter,
  Input,
  Output,
  ViewChild,
} from '@angular/core';

@Component({
  selector: 'app-grade-row-definition-delete-modal',
  standalone: true,
  imports: [NgClass],
  templateUrl: './grade-row-definition-delete-modal.component.html',
})
export class GradeRowDefinitionDeleteModalComponent {
  @Output() delete = new EventEmitter();
  @ViewChild('deleteModal') deleteModal: ElementRef | undefined;
  @Input() size: 'sm' | 'md' = 'sm';

  openModal() {
    this.deleteModal?.nativeElement.showModal();
  }

  closeModal() {
    this.deleteModal?.nativeElement.close();
  }

  deleteRow() {
    this.delete.emit();
    this.closeModal();
  }
}
