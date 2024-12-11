import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { CollectionResource } from '../types/collection-resource';
import { CommentModel } from '../types/course-board/comment-model';
import { ApiResponse } from '../types/api-response';
import { CommentCreateModel } from '../types/course-board/comment-create-model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class CommentService {
  constructor(
    private http: HttpClient,
    private authService: AuthService,
  ) {}

  public getComments(
    editionId: string,
    postId: string,
    page: number,
    pageSize: number,
  ) {
    return this.http.get<ApiResponse<CollectionResource<CommentModel>>>(
      `${environment.apiUrl}/api/editions/${editionId}/posts/${postId}/comments?page=${page}&pageSize=${pageSize}`,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  public deleteComment(editionId: string, postId: string, commentId: string) {
    return this.http.delete<ApiResponse<null>>(
      `${environment.apiUrl}/api/editions/${editionId}/posts/${postId}/comments/${commentId}`,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  public createComment(
    editionId: string,
    postId: string,
    model: CommentCreateModel,
  ) {
    return this.http.post<ApiResponse<CommentModel>>(
      `${environment.apiUrl}/api/editions/${editionId}/posts/${postId}/comments`,
      model,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  public updateComment(
    editionId: string,
    postId: string,
    commentId: string,
    model: CommentCreateModel,
  ) {
    return this.http.put<ApiResponse<CommentModel>>(
      `${environment.apiUrl}/api/editions/${editionId}/posts/${postId}/comments/${commentId}`,
      model,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }
}
