import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  OnDestroy,
  Output,
} from '@angular/core';
import { Subscription } from 'rxjs';
import { Router } from '@angular/router';
import { CommentModel } from '../../types/course-board/comment-model';
import { ReactionModel } from '../../types/course-board/reaction-model';
import { ReactionType } from '../../types/course-board/reaction-type';
import { ReactionService } from '../../services/reaction.service';
import { AuthService } from '../../services/auth.service';
import { DateFormatting } from '../../helpers/date-formatter';
import { NgIf } from '@angular/common';
import { UserAvatarComponent } from '../user-avatar/user-avatar.component';
import { CommentDeleteComponent } from '../comment-delete/comment-delete.component';
import { LikeButtonComponent } from '../like-button/like-button.component';
import { ReactionsDisplayComponent } from '../reactions-display/reactions-display.component';
import { EditCommentComponent } from '../edit-comment/edit-comment.component';

@Component({
  selector: 'app-comment',
  standalone: true,
  imports: [
    NgIf,
    UserAvatarComponent,
    CommentDeleteComponent,
    LikeButtonComponent,
    ReactionsDisplayComponent,
    EditCommentComponent,
  ],
  templateUrl: './comment.component.html',
})
export class CommentComponent implements OnInit, OnDestroy {
  @Input() editionId: string = '';
  @Input() comment: CommentModel = {} as CommentModel;
  @Output() deleted: EventEmitter<string> = new EventEmitter<string>();

  formattedDate: string = '';
  reactions: ReactionModel = {} as ReactionModel;
  sortedReactions: [ReactionType, number][] = [];
  postReactionTypesCount = 0;
  currentUserReactionType: ReactionType | null = null;
  subscription = new Subscription();
  isEdited: boolean = false;
  userId: string = '';
  formattedDateOfEditing: string = '';

  constructor(
    private reactionService: ReactionService,
    private authService: AuthService,
    private router: Router,
  ) {}

  upsertReaction(reactionType: ReactionType) {
    if (this.reactionService.isLoading()) return;
    this.subscription = this.reactionService
      .upsertReaction(
        this.editionId,
        this.comment.id,
        reactionType,
        this.comment.currentUserReaction,
        'comment',
      )
      .subscribe(() => {
        this.changeReaction(
          reactionType === this.comment.currentUserReaction
            ? null
            : reactionType,
        );
        this.ngOnInit();
      });
  }

  changeReaction(newReactionType: ReactionType | null): void {
    const oldReaction = this.comment.currentUserReaction;
    this.comment.currentUserReaction = newReactionType;

    // update reaction type count
    if (oldReaction !== null) {
      this.comment.reactions.sumOfReactionsByType.set(
        oldReaction,
        this.comment.reactions.sumOfReactionsByType.get(oldReaction)! - 1,
      );
    }
    if (newReactionType !== null) {
      const currentCount =
        this.comment.reactions.sumOfReactionsByType.get(newReactionType);
      if (currentCount) {
        this.comment.reactions.sumOfReactionsByType.set(
          newReactionType,
          currentCount + 1,
        );
      } else {
        this.comment.reactions.sumOfReactionsByType.set(newReactionType, 1);
      }
    }

    // update overall count
    if (oldReaction === null && newReactionType !== null) {
      this.comment.reactions.sumOfReactions++;
    } else if (oldReaction !== null && newReactionType === null) {
      this.comment.reactions.sumOfReactions--;
    }

    // rerender component
    this.comment = { ...this.comment };
  }

  handleDeletion(commentId?: string): void {
    if (commentId) {
      this.deleted.emit(commentId);
    }
  }

  enableEditing(): void {
    this.isEdited = !this.isEdited;
  }

  updateComment(updatedComment?: CommentModel): void {
    this.isEdited = false;
    if (updatedComment) {
      this.comment = updatedComment;
      if (this.comment.updatedAt)
        this.formattedDateOfEditing = DateFormatting.formatDateTime(
          this.comment.updatedAt,
        );
    }
  }

  async redirectToProfile() {
    await this.router.navigate(['/profiles', this.comment.author.id, 'info']);
  }

  ngOnInit(): void {
    this.formattedDate = DateFormatting.formatDateTime(this.comment.createdAt);
    if (this.comment.updatedAt)
      this.formattedDateOfEditing = DateFormatting.formatDateTime(
        this.comment.updatedAt,
      );
    this.reactions = this.comment.reactions;
    this.currentUserReactionType = this.comment.currentUserReaction;
    this.postReactionTypesCount =
      this.comment.reactions.sumOfReactionsByType.size;

    if (
      this.reactions.sumOfReactionsByType &&
      this.reactions.sumOfReactionsByType.size > 0
    ) {
      this.sortedReactions = [...this.reactions.sumOfReactionsByType.entries()]
        .filter((reaction) => reaction[1] > 0)
        .sort((a, b) => b[1] - a[1]);
    } else {
      this.sortedReactions = [];
    }
    this.userId = this.authService.authState().userData!.id;
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
