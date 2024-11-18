import {
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
} from '@angular/core';
import { Subscription } from 'rxjs';
import { CommentModel } from '../../types/course-board/comment-model';
import { CommentService } from '../../services/comment.service';
import { ReactionType } from '../../types/course-board/reaction-type';
import { CollectionResource } from '../../types/collection-resource';
import { NgFor, NgIf, NgClass } from '@angular/common';
import { CommentComponent } from '../comment/comment.component';
import { ApiResponse } from '../../types/api-response';
import { AddCommentComponent } from '../add-comment/add-comment.component';

@Component({
  selector: 'app-comment-list',
  standalone: true,
  imports: [NgIf, NgFor, NgClass, CommentComponent, AddCommentComponent],
  templateUrl: './comment-list.component.html',
})
export class CommentListComponent implements OnInit, OnDestroy {
  @Input() editionId: string = '';
  @Input() postId: string = '';
  @Output() contentAdded: EventEmitter<any> = new EventEmitter<any>();
  @Output() contentDeleted: EventEmitter<any> = new EventEmitter<any>();

  comments: CommentModel[] = [];
  totalPages: number = 0;
  currentPage: number = 0;

  isLoading: boolean = false;
  subscription = new Subscription();

  constructor(private commentService: CommentService) {}

  private parseComments(comments: CommentModel[]) {
    comments.forEach((c) => this.parseComment(c));
  }

  private parseComment(comment: CommentModel) {
    const keys = Object.keys(comment.reactions.sumOfReactionsByType);
    const value = Object.values(comment.reactions.sumOfReactionsByType);

    comment.reactions.sumOfReactionsByType = new Map();

    for (let i = 0; i < keys.length; i++) {
      const reactionType: ReactionType =
        ReactionType[keys[i] as keyof typeof ReactionType];
      comment.reactions.sumOfReactionsByType.set(reactionType, value[i]);
    }
  }

  handleDeletion(commentToDelete: string) {
    if (commentToDelete) {
      this.comments = this.comments.filter(
        (comment) => comment.id !== commentToDelete,
      );
      this.contentDeleted.emit();
    }
  }

  getComments(page: number, pageSize: number): void {
    this.isLoading = true;
    this.subscription = this.commentService
      .getComments(this.editionId, this.postId, page, pageSize)
      .subscribe({
        next: (response: ApiResponse<CollectionResource<CommentModel>>) => {
          if (response.data?.items) {
            this.comments = [...this.comments, ...response.data.items];
            this.parseComments(this.comments);
            this.totalPages = Math.ceil(response.data.totalCount / 5);
            this.isLoading = false;
            this.currentPage++;
          }
        },
        error: (error: any) => {
          this.isLoading = false;
          console.log(error);
        },
      });
  }

  addComment(newComment?: CommentModel): void {
    if (newComment) {
      this.parseComment(newComment);
      this.comments.unshift(newComment);
      this.contentAdded.emit();
    }
  }

  ngOnInit(): void {
    this.getComments(1, 5);
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
