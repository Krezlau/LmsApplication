import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnDestroy,
  OnInit,
  Output,
  SimpleChanges,
} from '@angular/core';
import { PostModel } from '../../types/course-board/post-model';
import { ReactionType } from '../../types/course-board/reaction-type';
import { Subscription } from 'rxjs';
import { ReactionModel } from '../../types/course-board/reaction-model';
import { PostService } from '../../services/post.service';
import { ReactionService } from '../../services/reaction.service';
import { DateFormatting } from '../../helpers/date-formatter';
import { NgClass, NgIf } from '@angular/common';
import { ReactionsDisplayComponent } from '../reactions-display/reactions-display.component';
import { LikeButtonComponent } from '../like-button/like-button.component';

@Component({
  selector: 'app-post-bottom-bar',
  standalone: true,
  imports: [NgIf, NgClass, ReactionsDisplayComponent, LikeButtonComponent],
  templateUrl: './post-bottom-bar.component.html',
})
export class PostBottomBarComponent implements OnInit, OnDestroy, OnChanges {
  @Input() post: PostModel = {} as PostModel;
  @Output() changeReaction: EventEmitter<ReactionType | null> =
    new EventEmitter<ReactionType | null>();
  @Output() toggleComments: EventEmitter<void> = new EventEmitter<void>();

  formattedDate: string = '';
  isVisible: boolean = true;

  subscription: Subscription = new Subscription();

  reactions: ReactionModel = {} as ReactionModel;
  sortedReactions: [ReactionType, number][] = [];

  postReactionTypesCount = 0;

  // musze tak bo nie działają enumy w htmlu
  like = ReactionType.Like;
  love = ReactionType.Love;
  haha = ReactionType.Haha;
  wow = ReactionType.Wow;
  sad = ReactionType.Sad;
  wrr = ReactionType.Angry;

  constructor(
    private reactionService: ReactionService,
    private postService: PostService,
  ) {}

  onCommentsClicked() {
    this.toggleComments.emit();
  }

  checkIfPostWasEdited() {
    if (
      this.post.updatedAt == null ||
      this.post.createdAt.valueOf() == this.post.updatedAt.valueOf()
    ) {
      this.isVisible = false;
    } else {
      this.isVisible = true;
      this.formattedDate = DateFormatting.formatDateTime(this.post.updatedAt);
    }
  }

  updateReaction(reactionType: ReactionType | null) {
    const newReactionType =
      reactionType === this.post.currentUserReaction ? null : reactionType;
    this.changeReaction.emit(newReactionType);
  }

  upsertReaction(reactionType: ReactionType) {
    if (this.reactionService.isLoading()) return;
    this.subscription = this.reactionService
      .upsertReaction(this.post.editionId, this.post.id, reactionType, this.post.currentUserReaction)
      .subscribe(() => this.updateReaction(reactionType));
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['post']) {
      this.ngOnInit();
    }
  }

  ngOnInit(): void {
    this.reactions = this.post.reactions;
    this.checkIfPostWasEdited();
    this.postReactionTypesCount = this.post.reactions.sumOfReactionsByType.size;
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
    console.log(this.reactions);
    console.log(this.sortedReactions);
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
