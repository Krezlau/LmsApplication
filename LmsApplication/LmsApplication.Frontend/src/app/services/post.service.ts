import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { ApiResponse } from '../types/api-response';
import { PostModel } from '../types/course-board/post-model';
import { CollectionResource } from '../types/collection-resource';
import { PostCreateModel } from '../types/course-board/post-create-model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class PostService {
  constructor(
    private http: HttpClient,
    private authService: AuthService,
  ) {}

  public getPosts(editionId: string, page: number, size: number) {
    return this.http.get<ApiResponse<CollectionResource<PostModel>>>(
      `${environment.apiUrl}/api/editions/${editionId}/posts?page=${page}&pageSize=${size}`,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  public createPost(editionId: string, createModel: PostCreateModel) {
    return this.http.post<ApiResponse<PostModel>>(
      `${environment.apiUrl}/api/editions/${editionId}/posts`,
      createModel,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  public updatePost(
    editionId: string,
    postId: string,
    updateModel: PostCreateModel,
  ) {
    return this.http.put<ApiResponse<PostModel>>(
      `${environment.apiUrl}/api/editions/${editionId}/posts/${postId}`,
      updateModel,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  public deletePost(editionId: string, postId: string) {
    return this.http.delete<ApiResponse<null>>(
      `${environment.apiUrl}/api/editions/${editionId}/posts/${postId}`,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }
}
