import { UserModel } from '../users/user-model';
import { ReactionModel } from './reaction-model';
import { ReactionType } from './reaction-type';

export interface PostModel {
  id: string;
  content: string;
  editionId: string;
  author: UserModel;
  reactions: ReactionModel;
  currentUserReaction: ReactionType | null;
  commentsCount: number;
  createdAt: Date;
  updatedAt: Date;
}
