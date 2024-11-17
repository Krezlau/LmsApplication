import { Component } from '@angular/core';
import { PostListComponent } from '../post-list/post-list.component';

@Component({
  selector: 'app-course-edition-board',
  standalone: true,
  imports: [PostListComponent],
  templateUrl: './course-edition-board.component.html'
})
export class CourseEditionBoardComponent {

}
