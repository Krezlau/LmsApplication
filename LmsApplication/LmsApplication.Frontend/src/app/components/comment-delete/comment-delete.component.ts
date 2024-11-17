import {
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
} from '@angular/core';
import { Subscription } from 'rxjs';
import { AlertService } from '../../services/alert.service';
import { AuthService } from '../../services/auth.service';
import { CommentService } from '../../services/comment.service';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-comment-delete',
  standalone: true,
  imports: [NgIf],
  templateUrl: './comment-delete.component.html',
})
export class CommentDeleteComponent implements OnDestroy, OnInit {
  @Input() authorId: string = '';
  @Input() editionId: string = '';
  @Input() commentId: string = '';
  @Input() postId: string = '';
  @Output() deleted: EventEmitter<string> = new EventEmitter<string>();

  visible: boolean = false;
  isLoading: boolean = false;
  private deleteCommentSubscription?: Subscription;

  constructor(
    private alertService: AlertService,
    private authService: AuthService,
    private commentService: CommentService,
  ) {}

  deleteComment(): void {
    this.isLoading = true;
    this.deleteCommentSubscription = this.commentService
      .deleteComment(this.editionId, this.postId, this.commentId)
      .subscribe(
        () => {
          this.deleted.emit(this.commentId);
          this.alertService.show(
            'Comment deleted successfully',
            'success',
          );
          this.isLoading = false;
        },
        (error) => {
          this.isLoading = false;
        },
      );
  }
  ngOnDestroy(): void {
    this.deleteCommentSubscription?.unsubscribe();
  }

  ngOnInit(): void {
    if (this.authService.authState().userData?.id === this.authorId) {
      this.visible = true;
    }
  }
}
