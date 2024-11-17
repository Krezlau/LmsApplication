import { ReactionType } from "./reaction-type";

export interface ReactionModel {
  sumOfReactions: number;
  sumOfReactionsByType: Map<ReactionType, number>;
}
