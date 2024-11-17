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
import { PostService } from '../../services/post.service';
import { AuthService } from '../../services/auth.service';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-delete-post',
  standalone: true,
  imports: [NgIf],
  templateUrl: './delete-post.component.html',
})
export class DeletePostComponent implements OnDestroy, OnInit {
  @Input() postId: string = '';
  @Input() authorId: string = '';
  @Input() editionId: string = '';
  @Output() deleted: EventEmitter<string> = new EventEmitter<string>();

  visible: boolean = false;
  private addPostSubscription?: Subscription;

  isLoading: boolean = false;
  constructor(
    private alertService: AlertService,
    private postService: PostService,
    private authService: AuthService,
  ) {}

  deletePost(): void {
    this.isLoading = true;
    this.addPostSubscription = this.postService
      .deletePost(this.editionId, this.postId)
      .subscribe(
        () => {
          this.deleted.emit(this.postId);
          this.isLoading = false;
          this.alertService.show('Post deleted successfully', 'success');
        },
        (error) => {
          this.isLoading = false;
        },
      );
  }

  ngOnInit() {
    if (this.authService.authState().userData?.id === this.authorId) {
      this.visible = true;
    }
  }
  ngOnDestroy(): void {
    this.addPostSubscription?.unsubscribe();
  }
}
