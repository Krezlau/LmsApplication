import {
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  Output,
} from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { CommentModel } from '../../types/course-board/comment-model';
import { CommentService } from '../../services/comment.service';
import { AuthService } from '../../services/auth.service';
import { CommentCreateModel } from '../../types/course-board/comment-create-model';
import { ApiResponse } from '../../types/api-response';
import { NgClass, NgIf } from '@angular/common';
import { UserAvatarComponent } from '../user-avatar/user-avatar.component';

@Component({
  selector: 'app-add-comment',
  standalone: true,
  imports: [NgIf, NgClass, ReactiveFormsModule, UserAvatarComponent],
  templateUrl: './add-comment.component.html',
})
export class AddCommentComponent implements OnDestroy {
  @Input() editionId: string = '';
  @Input() postId: string = '';

  @Output() contentAdded: EventEmitter<CommentModel> =
    new EventEmitter<CommentModel>();

  contentControl = new FormControl('', [
    Validators.required,
    Validators.minLength(1),
  ]);
  isLoading: boolean = false;
  model: CommentCreateModel;
  private addCommentSubscription?: Subscription;
  authState = this.authService.authState;

  constructor(
    private commentService: CommentService,
    private authService: AuthService,
  ) {
    this.model = {
      content: '',
    };
  }

  onFormSubmit(): void {
    if (this.contentControl.valid && this.contentControl.value != null) {
      this.isLoading = true;
      this.model.content = this.contentControl.value;
      this.addCommentSubscription = this.commentService
        .createComment(this.editionId, this.postId, this.model)
        .subscribe(
          (response: ApiResponse<CommentModel>) => {
            this.contentAdded.emit(response.data!);
            this.contentControl.reset();
            this.isLoading = false;
          },
          (error) => {
            this.isLoading = false;
          },
        );
    }
  }

  ngOnDestroy(): void {
    this.addCommentSubscription?.unsubscribe();
  }
}
