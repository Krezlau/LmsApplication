import { UserModel } from "../users/user-model";
import { ReactionModel } from "./reaction-model";
import { ReactionType } from "./reaction-type";

export interface CommentModel {
  id: string;
  content: string;
  postId: string;
  author: UserModel;
  reactions: ReactionModel;
  currentUserReaction: ReactionType | null;
  createdAt: Date;
  updatedAt: Date | null;
}
