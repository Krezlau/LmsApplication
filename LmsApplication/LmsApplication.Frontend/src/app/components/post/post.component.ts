import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnDestroy,
  OnInit,
  Output,
  SimpleChanges,
  signal,
} from '@angular/core';
import { PostModel } from '../../types/course-board/post-model';
import { Subscription } from 'rxjs';
import { AuthService } from '../../services/auth.service';
import { PostService } from '../../services/post.service';
import { Router } from '@angular/router';
import { ReactionType } from '../../types/course-board/reaction-type';
import { DeletePostComponent } from '../delete-post/delete-post.component';
import { EditPostComponent } from '../edit-post/edit-post.component';
import { NgIf } from '@angular/common';
import { PostBottomBarComponent } from '../post-bottom-bar/post-bottom-bar.component';
import { UserAvatarComponent } from '../user-avatar/user-avatar.component';
import { DateFormatting } from '../../helpers/date-formatter';

@Component({
  selector: 'app-post',
  standalone: true,
  imports: [
    NgIf,
    DeletePostComponent,
    EditPostComponent,
    PostBottomBarComponent,
    UserAvatarComponent,
  ],
  templateUrl: './post.component.html',
})
export class PostComponent implements OnChanges, OnInit, OnDestroy {
  @Input() post: PostModel = {} as PostModel;
  @Output() deleted: EventEmitter<string> = new EventEmitter<string>();
  @Output() contentUpdated: EventEmitter<PostModel> =
    new EventEmitter<PostModel>();

  formattedDate: string = '';
  userId: string = '';
  isEdited: boolean = false;

  imageSub = new Subscription();
  imageLoading = signal(false);
  imageUrl: string | null = null;

  constructor(
    private authService: AuthService,
    //private imageService: ImageService,
    private postService: PostService,
    private router: Router,
  ) {}

  enableEditing(): void {
    this.isEdited = !this.isEdited;
  }

  updateContent(updatedPost?: PostModel): void {
    this.isEdited = false;
    if (updatedPost) {
      this.post = updatedPost;
    }
  }

  handleDeletion(postId?: string): void {
    if (postId) {
      this.deleted.emit(postId);
    }
  }

  changeReaction(newReactionType: ReactionType | null): void {
    const oldReaction =
      this.post.currentUserReaction === null
        ? null
        : +this.post.currentUserReaction;
    this.post.currentUserReaction = newReactionType;
    console.log(newReactionType);

    // update reaction type count
    if (oldReaction !== null) {
      this.post.reactions.sumOfReactionsByType.set(
        +oldReaction,
        this.post.reactions.sumOfReactionsByType.get(oldReaction)! - 1,
      );
    }
    if (newReactionType !== null) {
      const currentCount =
        this.post.reactions.sumOfReactionsByType.get(newReactionType);
      if (currentCount) {
        this.post.reactions.sumOfReactionsByType.set(
          +newReactionType,
          currentCount + 1,
        );
      } else {
        this.post.reactions.sumOfReactionsByType.set(newReactionType, 1);
      }
    }

    // update overall count
    if (oldReaction === null && newReactionType !== null) {
      this.post.reactions.sumOfReactions++;
    } else if (oldReaction !== null && newReactionType === null) {
      this.post.reactions.sumOfReactions--;
    }

    // rerender component
    this.post = { ...this.post };
    console.log(this.post);
  }

  handleCommentAdding() {
    this.post.commentsCount++;
  }

  handleCommentDeletion() {
    this.post.commentsCount--;
  }

  removeImage() {
    /* TODO
    this.post.mediaId = null;
    this.imageUrl = null;
    this.imageSub.add(this.postService.updatePost(this.post.postId, {content: this.post.content, mediaId: null}).subscribe(
      (updatedPost) => {
        this.updateContent(updatedPost);
        this.contentUpdated.emit(updatedPost);
        this.post = updatedPost;
      }
    ));
  */
  }

  async redirectToProfile() {
    await this.router.navigate(['/users', this.post.author.email]);
  }

  ngOnInit() {
    this.userId = this.authService.authState().userData!.id;
    /*
    if (this.post.mediaId) {
      this.imageLoading.set(true);
      this.imageSub.add(this.imageService.downloadImage(this.post.mediaId).subscribe(
        (blob) => {
          this.imageUrl = URL.createObjectURL(blob);
          this.imageLoading.set(false);
        }
      ));
    }
    */
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['post'] && this.post && this.post.createdAt) {
      this.formattedDate = DateFormatting.formatDateTime(this.post.createdAt);
    }
  }

  ngOnDestroy() {
    this.imageSub.unsubscribe();
  }
}
