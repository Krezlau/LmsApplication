import { Component } from '@angular/core';

@Component({
  selector: 'app-delete-modal',
  standalone: true,
  imports: [],
  templateUrl: './delete-modal.component.html',
})
export class DeleteModalComponent {
  @Output() delete = new EventEmitter<void>();
}
