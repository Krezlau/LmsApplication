import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription, tap } from 'rxjs';
import { PostService } from '../../services/post.service';
import { PostModel } from '../../types/course-board/post-model';
import { NgFor, NgIf } from '@angular/common';
import { AddPostComponent } from '../add-post/add-post.component';
import { PostComponent } from '../post/post.component';
import { Router } from '@angular/router';
import { ApiResponse } from '../../types/api-response';
import { CollectionResource } from '../../types/collection-resource';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { ReactionType } from '../../types/course-board/reaction-type';

@Component({
  selector: 'app-post-list',
  standalone: true,
  imports: [NgIf, NgFor, AddPostComponent, PostComponent, InfiniteScrollModule],
  templateUrl: './post-list.component.html',
})
export class PostListComponent implements OnInit, OnDestroy {
  posts: PostModel[] = [];
  isLoading = false;
  pageNumber: number = 1;

  totalPages: number = 0;
  courseEditionId: string = '';

  subscription = new Subscription();

  constructor(
    private postService: PostService,
    private router: Router,
  ) {}

  onScroll() {
    this.loadPosts();
  }

  private parsePosts(posts: PostModel[]) {
    posts.forEach((post) => {
      const keys = Object.keys(post.reactions.sumOfReactionsByType);
      const value = Object.values(post.reactions.sumOfReactionsByType);

      post.reactions.sumOfReactionsByType = new Map();

      for (let i = 0; i < keys.length; i++) {
        const reactionType: ReactionType =
          ReactionType[keys[i] as keyof typeof ReactionType];
        post.reactions.sumOfReactionsByType.set(reactionType, value[i]);
      }
    });
  }

  loadPosts() {
    if (this.totalPages === this.pageNumber && this.pageNumber !== 0) return;
    this.isLoading = true;
    this.subscription = this.postService
      .getPosts(this.courseEditionId, this.pageNumber, 10)
      .pipe(
        tap({
          next: (response: ApiResponse<CollectionResource<PostModel>>) => {
            this.parsePosts(response.data!.items);
            this.posts = [...this.posts, ...response.data!.items];
            this.pageNumber = this.pageNumber + 1;
            this.isLoading = false;
            this.totalPages = response.data?.totalCount! / 10;
            console.log(this.posts);
          },
          error: (error) => {
            this.isLoading = false;
          },
        }),
      )
      .subscribe();
  }

  addContent(newPost?: PostModel): void {
    if (newPost) {
      this.parsePosts([newPost]);
      this.posts.unshift(newPost);
    }
  }

  handleDeletion(postToDelete: string) {
    if (postToDelete) {
      this.posts = this.posts.filter((post) => post.id !== postToDelete);
    }
  }

  ngOnInit() {
    this.courseEditionId = this.router.url.split('/')[2];
    this.loadPosts();
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }
}
