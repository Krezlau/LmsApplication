import { Component, Input } from '@angular/core';
import { ReactionType } from '../../types/course-board/reaction-type';
import { NgClass, NgIf } from '@angular/common';

@Component({
  selector: 'app-reactions-display',
  standalone: true,
  imports: [NgClass, NgIf],
  templateUrl: './reactions-display.component.html',
  styleUrl: './reactions-display.component.css',
})
export class ReactionsDisplayComponent {
  @Input() sortedReactions: [ReactionType, number][] = [];
  @Input() sumOfReactions: number = 0;
  @Input() isComment?: boolean = false;

  // musze tak bo nie działają enumy w htmlu
  like = ReactionType.Like;
  love = ReactionType.Love;
  haha = ReactionType.Haha;
  wow = ReactionType.Wow;
  sad = ReactionType.Sad;
  wrr = ReactionType.Angry;
}
