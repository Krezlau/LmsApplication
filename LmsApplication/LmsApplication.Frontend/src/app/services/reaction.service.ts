import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { ReactionType } from '../types/course-board/reaction-type';
import { finalize } from 'rxjs/operators';
import { env } from '../../env';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root',
})
export class ReactionService {
  constructor(
    private http: HttpClient,
    private authService: AuthService,
  ) {}

  isLoading = signal(false);

  public upsertReaction(
    editionId: string,
    contentId: string,
    reactionType: ReactionType,
    currentReactionType: ReactionType | null,
    contentType: string = 'post',
  ) {
    this.isLoading.set(true);

    const reactionToSend =
      reactionType === currentReactionType ? null : reactionType;

    console.log(reactionToSend)

    return this.http
      .put<ReactionType | null>(
        `${env.apiUrl}/api/editions/${editionId}/${contentType}s/${contentId}/reactions?${
          reactionToSend !== null ? 'type=' + reactionToSend : ''
        }`,
        {},
        {
          headers: {
            Authorization: `Bearer ${this.authService.authState().accessToken}`,
          },
        },
      )
      .pipe(
        finalize(() => {
          this.isLoading.set(false);
        }),
      );
  }
}
