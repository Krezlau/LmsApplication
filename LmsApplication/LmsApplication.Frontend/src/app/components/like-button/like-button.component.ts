import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ReactionType } from '../../types/course-board/reaction-type';
import { NgClass, NgIf } from '@angular/common';

@Component({
  selector: 'app-like-button',
  standalone: true,
  imports: [NgClass, NgIf],
  templateUrl: './like-button.component.html',
  styleUrl: './like-button.component.css',
})
export class LikeButtonComponent implements OnInit {
  @Input() currentUserReaction: ReactionType | null = null;
  @Input() isComment?: boolean = false;

  @Output() changeReaction: EventEmitter<ReactionType> =
    new EventEmitter<ReactionType>();

  // musze tak bo nie działają enumy w htmlu
  like = ReactionType.Like;
  love = ReactionType.Love;
  haha = ReactionType.Haha;
  wow = ReactionType.Wow;
  sad = ReactionType.Sad;
  wrr = ReactionType.Angry;

  ngOnInit() {
    console.log(this.currentUserReaction);
  }
}
